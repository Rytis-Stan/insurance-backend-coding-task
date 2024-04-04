using System.Text.Json.Serialization;
using Claims.Api.Configuration;
using Claims.Application;
using Claims.Auditing;
using Claims.Auditing.MessageQueues.RabbitMq;
using Claims.Infrastructure;
using Claims.Persistence.CosmosDb;
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
        var configuration = AppConfiguration.FromConfiguration(builder.Configuration);
        var claimsDatabase = MigratedDatabases(configuration);
        AddServices(builder.Services, configuration, claimsDatabase);
        var app = builder.Build();
        ConfigureApp(app);
        return app;
    }

    private static IClaimsDatabase MigratedDatabases(AppConfiguration configuration)
    {
        MigrateAuditDatabase(configuration.ConnectionString);
        return MigrateMainDatabase(configuration.CosmosDb);
    }

    private static void MigrateAuditDatabase(string connectionString)
    {
        new EntityFrameworkAuditDatabase(connectionString).Migrate();
    }

    private static IClaimsDatabase MigrateMainDatabase(CosmosDbConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        var database = new ClaimsDatabase(cosmosClient, configuration.DatabaseName, new IdGenerator());
        database.InitializeAsync().GetAwaiter().GetResult();
        return database;
    }

    private static void AddServices(IServiceCollection services, AppConfiguration configuration, IClaimsDatabase claimsDatabase)
    {
        AddControllers(services);
        AddRepositories(services, claimsDatabase);
        AddInfrastructure(services);
        AddDomainServices(services);
        AddAuditing(services, configuration.RabbitMq);
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

    private static void AddAuditing(IServiceCollection services, RabbitMqConfiguration configuration)
    {
        var queue = new InactiveRabbitMqSendingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Activate();
        services.AddScoped<IClaimAuditor>(_ => new MessageQueueClaimAuditor(queue));
        services.AddScoped<ICoverAuditor>(_ => new MessageQueueCoverAuditor(queue));
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