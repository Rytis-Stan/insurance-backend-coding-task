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
        var app = BuildApp(args);
        ConfigureApp(app);
        MigrateDatabase(app);

        app.Run();
    }

    private static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        AddServices(builder);
        var app = builder.Build();
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

        var cosmosClient = InitializeCosmosClientInstanceAsync(configuration.CosmosDb).GetAwaiter().GetResult();
        var claimsRepository = new CosmosDbClaimsRepository(cosmosClient, configuration.CosmosDb.DatabaseName, configuration.CosmosDb.ContainerName);
        var coversRepository = new CosmosDbCoversRepository(cosmosClient, configuration.CosmosDb.DatabaseName, "Cover");

        services.AddSingleton(claimsRepository);
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(configuration.ConnectionString));
        services.AddTransient<IClaimsService>(x => new ClaimsService(claimsRepository, new Auditor(x.GetRequiredService<AuditContext>(), new Clock())));
        services.AddTransient<ICoversService>(x => new CoversService(coversRepository, new Auditor(x.GetRequiredService<AuditContext>(), new Clock())));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
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
        await database.Database.CreateContainerIfNotExistsAsync(configuration.ContainerName, "/id");
        return client;
    }

    private static void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
    }
}
