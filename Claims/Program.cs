using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Configuration;
using Claims.DataAccess;
using Claims.Domain;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Claims;

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
        return app;
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
        var clock = AddRepositories(cosmosClient, cosmosDbConfiguration, services);
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(configuration.ConnectionString));

        services.AddSingleton<IClock>(clock);
        services.AddTransient<IClaimAuditor, Auditor>();
        services.AddTransient<ICoverAuditor, Auditor>();
        services.AddTransient<IClaimsService, ClaimsService>();
        services.AddTransient<ICoversService, CoversService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static Clock AddRepositories(CosmosClient cosmosClient, CosmosDbConfiguration cosmosDbConfiguration,
        IServiceCollection services)
    {
        var clock = new Clock();
        var idGenerator = new IdGenerator();
        services.AddSingleton<IClaimsRepository>(new CosmosDbClaimsRepository(cosmosClient, cosmosDbConfiguration.DatabaseName, cosmosDbConfiguration.ContainerNames.Claim, clock, idGenerator));
        services.AddSingleton<ICoversRepository>(new CosmosDbCoversRepository(cosmosClient, cosmosDbConfiguration.DatabaseName, cosmosDbConfiguration.ContainerNames.Cover, clock, idGenerator));
        return clock;
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
        var database = await client.CreateDatabaseIfNotExistsAsync(configuration.DatabaseName);
        foreach (var containerName in configuration.ContainerNames)
        {
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
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
