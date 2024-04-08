using System.Text.Json.Serialization;
using BuildingBlocks.MessageQueues;
using BuildingBlocks.MessageQueues.RabbitMq;
using BuildingBlocks.Temporal;
using Claims.Api.Configuration;
using Claims.Application.Commands;
using Claims.Application.Commands.CreateClaim;
using Claims.Application.Commands.CreateCover;
using Claims.Application.Commands.DeleteClaim;
using Claims.Application.Commands.DeleteCover;
using Claims.Application.Commands.GetAllClaims;
using Claims.Application.Commands.GetAllCovers;
using Claims.Application.Commands.GetClaimById;
using Claims.Application.Commands.GetCover;
using Claims.Application.Commands.GetCoverPremium;
using Claims.Auditing;
using Claims.Auditing.MessageQueueBased;
using Claims.Domain;
using Claims.Persistence.Claims;
using Microsoft.Azure.Cosmos;

namespace Claims.Api;

// NOTE: Please read the updated "README.md" file for discussions about various task implementation details.
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
        var auditQueue = InitializeAuditQueue(configuration.RabbitMq);
        var claimsDatabase = MigrateClaimsDatabase(configuration.CosmosDb);
        AddServices(builder.Services, claimsDatabase, auditQueue);
        var app = builder.Build();
        ConfigureApp(app);
        return app;
    }

    private static IConnectedSendingQueue<AuditMessage> InitializeAuditQueue(RabbitMqConfiguration configuration)
    {
        return new RabbitMqSendingQueue<AuditMessage>(configuration.HostName, configuration.QueueName).StartSending();
    }

    private static IClaimsDatabase MigrateClaimsDatabase(CosmosDbConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        var database = new CosmosDbClaimsDatabase(cosmosClient, configuration.DatabaseName, new RandomIdGenerator());
        database.InitializeAsync().GetAwaiter().GetResult();
        return database;
    }

    private static void AddServices(IServiceCollection services, IClaimsDatabase claimsDatabase, IConnectedSendingQueue<AuditMessage> auditQueue)
    {
        AddControllers(services);
        AddRepositories(services, claimsDatabase);
        AddInfrastructure(services);
        AddDomain(services);
        AddApplicationCommands(services);
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

    private static void AddDomain(IServiceCollection services)
    {
        services.AddTransient<ICoverPricing, CoverPricing>();
    }

    private static void AddApplicationCommands(IServiceCollection services)
    {
        services.AddTransient<ICommand<CreateCoverRequest, CreateCoverResponse>, CreateCoverCommand>();
        services.AddTransient<ICommand<GetCoverRequest, GetCoverResponse>, GetCoverCommand>();
        services.AddTransient<ICommandWithNoParameters<GetAllCoversResponse>, GetAllCoversCommand>();
        services.AddTransient<ICommandWithNoResults<DeleteCoverRequest>, DeleteCoverCommand>();
        
        services.AddTransient<ICommand<GetCoverPremiumRequest, GetCoverPremiumResponse>, GetCoverPremiumCommand>();

        services.AddTransient<ICommand<CreateClaimRequest, CreateClaimResponse>, CreateClaimCommand>();
        services.AddTransient<ICommand<GetClaimByIdRequest, GetClaimByIdResponse>, GetClaimByIdCommand>();
        services.AddTransient<ICommandWithNoParameters<GetAllClaimsResponse>, GetAllClaimsCommand>();
        services.AddTransient<ICommandWithNoResults<DeleteClaimRequest>, DeleteClaimCommand>();
    }

    private static void AddAuditing(IServiceCollection services, IConnectedSendingQueue<AuditMessage> auditQueue)
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
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.MapControllers();
    }
}