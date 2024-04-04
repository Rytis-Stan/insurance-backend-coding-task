using System.Text.Json.Serialization;
using Claims.Api.Configuration;
using Claims.Application;
using Claims.Auditing;
using Claims.Auditing.MessageQueueBased;
using Claims.Auditing.MessageQueues;
using Claims.Auditing.MessageQueues.RabbitMq;
using Claims.Infrastructure;
using Claims.Persistence.Auditing;
using Claims.Persistence.Claims;
using Microsoft.Azure.Cosmos;

namespace Claims.Api;

public class Program
{
    static void Main(string[] args)
    {
        BuildApp(args).Run();
    }

    private static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = new AppConfiguration(builder.Configuration);
        var (claimsDatabase, auditQueue) = MigratedDatabases(configuration);
        AddServices(builder.Services, claimsDatabase, auditQueue);
        var app = builder.Build();
        ConfigureApp(app);
        return app;
    }

    private static (IClaimsDatabase, ISendingQueue<AuditMessage>) MigratedDatabases(AppConfiguration configuration)
    {
        MigrateAuditDatabase(configuration.ConnectionString);
        var auditQueue = InitializeAuditQueue(configuration.RabbitMq);
        var claimsDatabase = MigrateClaimsDatabase(configuration.CosmosDb);
        return (claimsDatabase, auditQueue);
    }

    private static ISendingQueue<AuditMessage> InitializeAuditQueue(RabbitMqConfiguration configuration)
    {
        return new InactiveRabbitMqSendingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Activate();
    }

    private static void MigrateAuditDatabase(string connectionString)
    {
        new EntityFrameworkAuditDatabase(connectionString).Migrate();
    }

    private static IClaimsDatabase MigrateClaimsDatabase(CosmosDbConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        var database = new ClaimsDatabase(cosmosClient, configuration.DatabaseName, new RandomIdGenerator());
        database.InitializeAsync().GetAwaiter().GetResult();
        return database;
    }

    private static void AddServices(IServiceCollection services, IClaimsDatabase claimsDatabase, ISendingQueue<AuditMessage> auditQueue)
    {
        AddControllers(services);
        AddRepositories(services, claimsDatabase);
        AddInfrastructure(services);
        AddDomainServices(services);
        AddAuditing(services, auditQueue);
        AddSwagger(services);
    }

    private static void AddControllers(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void AddInfrastructure(IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();
    }

    private static void AddRepositories(IServiceCollection services, IClaimsDatabase database)
    {
        services.AddScoped(_ => database.ClaimsRepository);
        services.AddScoped(_ => database.CoversRepository);
    }

    private static void AddDomainServices(IServiceCollection services)
    {
        services.AddTransient<IClaimsService, ClaimsService>();
        services.AddTransient<ICoversService, CoversService>();
        services.AddTransient<IPricingService, PricingService>();
    }

    private static void AddAuditing(IServiceCollection services, ISendingQueue<AuditMessage> auditQueue)
    {
        services.AddScoped<IClaimAuditor>(_ => new MessageQueueClaimAuditor(auditQueue));
        services.AddScoped<ICoverAuditor>(_ => new MessageQueueCoverAuditor(auditQueue));
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}