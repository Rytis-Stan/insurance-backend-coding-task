using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Claims.Api.Auditing;
using Claims.Api.Configuration;
using Claims.Infrastructure;
using Claims.Persistence.CosmosDb;
using Claims.Repositories;
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
        var services = builder.Services;
        var configuration = new AppConfiguration(builder.Configuration);

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        var cosmosDbConfiguration = configuration.CosmosDb;
        var cosmosClient = InitializeCosmosClientInstanceAsync(cosmosDbConfiguration).GetAwaiter().GetResult();
        AddRepositories(services, cosmosClient, cosmosDbConfiguration);
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(configuration.ConnectionString));

        services.AddSingleton<IClock, Clock>();
        services.AddTransient<IClaimAuditor, Auditor>();
        services.AddTransient<ICoverAuditor, Auditor>();
        services.AddTransient<IClaimsService, ClaimsService>();
        services.AddTransient<ICoversService, CoversService>();
        services.AddTransient<IPricingService, PricingService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddRepositories(IServiceCollection services, CosmosClient cosmosClient, CosmosDbConfiguration configuration)
    {
        Trace.WriteLine("DB Name: " + configuration.DatabaseName);

        // throw new Exception("The database name is: " + configuration.DatabaseName);

        var idGenerator = new IdGenerator();
        services.AddSingleton<IClaimsRepository>(new CosmosDbClaimsRepository(cosmosClient, configuration.DatabaseName, idGenerator));
        services.AddSingleton<ICoversRepository>(new CosmosDbCoversRepository(cosmosClient, configuration.DatabaseName, idGenerator));
    }

    private static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static async Task<CosmosClient> InitializeCosmosClientInstanceAsync(CosmosDbConfiguration configuration)
    {
        var client = new CosmosClient(configuration.Account, configuration.Key);
        var response = await client.CreateDatabaseIfNotExistsAsync(configuration.DatabaseName);
        foreach (var containerName in ContainerNames.All)
        {
            await response.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
        }
        return client;
    }

    private static void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
    }
}
