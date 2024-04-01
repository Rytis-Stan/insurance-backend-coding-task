using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Claims.Api.Configuration;
using Claims.Auditing;
using Claims.Domain;
using Claims.Infrastructure;
using Claims.Persistence.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

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
        MigrateDatabase(app);

        // TODO: Uncomment.
        // InitializeMessageQueues();

        return app;
    }

    private static void InitializeMessageQueues()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // TODO: Move the queue name (and some options???) to the configuration file!
        const string queueName = "Claims.AuditQueue";
        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        string message = $"MY_MESSAGE: {DateTime.UtcNow}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            basicProperties: null,
            body: body);
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        AddServices(builder.Services, AppConfigurationFrom(builder));
    }

    private static AppConfiguration AppConfigurationFrom(WebApplicationBuilder builder)
    {
        var configuration = new AppConfiguration();
        builder.Configuration.Bind(configuration);
        return configuration;
    }

    private static void AddServices(IServiceCollection services, AppConfiguration configuration)
    {
        AddControllers(services);
        AddRepositories(services, InitializeCosmosDb(configuration.CosmosDb));
        AddInfrastructure(services);
        AddDomainServices(services);
        AddAuditing(services, configuration.ConnectionString);
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
        Trace.WriteLine("DB Name: " + configuration.DatabaseName);

        // throw new Exception("The database name is: " + configuration.DatabaseName);

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

    private static void AddAuditing(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(connectionString));
        services.AddTransient<IClaimAuditor, Auditor>();
        services.AddTransient<ICoverAuditor, Auditor>();
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
    
    private static void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
    }
}