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
        AddServices(builder);
        var app = builder.Build();
        ConfigureApp(app);

        // TODO: Finish implementing queues for auditing.
        //InitializeMessageQueues();

        return app;
    }

    private static void InitializeMessageQueues(RabbitMqConfiguration configuration)
    {
        var queue = new InactiveRabbitMqSendingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).Activate();
        // Send some experimental messages!
        for (int i = 0; i < 10; i++)
        {
            queue.Send(new AuditMessage(AuditEntityKind.Claim, Guid.NewGuid(), HttpRequestType.Post));
        }
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        AddServices(builder.Services, AppConfiguration.FromConfiguration(builder.Configuration));
    }

    private static void AddServices(IServiceCollection services, AppConfiguration configuration)
    {
        MigrateAuditDatabase(configuration.ConnectionString);

        AddControllers(services);
        AddRepositories(services, InitializeCosmosDb(configuration.CosmosDb));
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

    private static ClaimsDatabase InitializeCosmosDb(CosmosDbConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        var database = new ClaimsDatabase(cosmosClient, configuration.DatabaseName, new IdGenerator());
        database.InitializeAsync().GetAwaiter().GetResult();
        return database;
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
    
    private static void MigrateAuditDatabase(string connectionString)
    {
        var auditDatabase = new EntityFrameworkAuditDatabase(connectionString);
        auditDatabase.Migrate();
    }
}