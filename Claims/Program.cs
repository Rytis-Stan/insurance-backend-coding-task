using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Controllers;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

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

        services.AddSingleton(InitializeCosmosClientInstanceAsync(configuration.CosmosDb).GetAwaiter().GetResult());
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(configuration.ConnectionString));

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

    private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(CosmosDbConfiguration configuration)
    {
        var databaseName = configuration.DatabaseName;
        var containerName = configuration.ContainerName;
        var account = configuration.Account;
        var key = configuration.Key;
        var client = new CosmosClient(account, key);
        var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

        return cosmosDbService;
    }

    private static void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
    }
}

public class AppConfiguration
{
    private readonly ConfigurationManager _configuration;

    public AppConfiguration(ConfigurationManager configuration)
    {
        _configuration = configuration;
    }

    public string ConnectionString => _configuration.GetConnectionString("DefaultConnection");
    public CosmosDbConfiguration CosmosDb => new CosmosDbConfiguration(_configuration.GetSection("CosmosDb"));
}

public class CosmosDbConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public string DatabaseName => _configurationSection.GetSection("DatabaseName").Value;
    public string ContainerName => _configurationSection.GetSection("ContainerName").Value;
    public string Account => _configurationSection.GetSection("Account").Value;
    public string Key => _configurationSection.GetSection("Key").Value;

    public CosmosDbConfiguration(IConfigurationSection configurationSection)
    {
        _configurationSection = configurationSection;
    }
}