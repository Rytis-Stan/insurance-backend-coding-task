using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Claims.Api.Configuration;
using Claims.Auditing;
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
        var cosmosClient = new CosmosClient(cosmosDbConfiguration.Account, cosmosDbConfiguration.Key);

        Trace.WriteLine("DB Name: " + cosmosDbConfiguration.DatabaseName);

        // throw new Exception("The database name is: " + configuration.DatabaseName);

        var database = new ClaimsDatabase(cosmosClient, cosmosDbConfiguration.DatabaseName, new IdGenerator());
        database.InitializeAsync().GetAwaiter().GetResult();

        AddRepositories(services, database);
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(configuration.ConnectionString));

        services.AddSingleton<IClock, Clock>();
        services.AddTransient<IClaimAuditor, Auditor>();
        services.AddTransient<ICoverAuditor, Auditor>();
        AddServices(services);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddRepositories(IServiceCollection services, IClaimsDatabase database)
    {
        services.AddScoped(_ => database.ClaimsRepository);
        services.AddScoped(_ => database.CoversRepository);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddTransient<IClaimsService, ClaimsService>();
        services.AddTransient<ICoversService, CoversService>();
        services.AddTransient<IPricingService, PricingService>();
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
    
    private static void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
    }
}
