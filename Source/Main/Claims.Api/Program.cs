using System.Text.Json.Serialization;
using Auditing.Auditors;
using Auditing.Auditors.MessageQueueBased;
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
using Claims.Domain;
using Claims.Persistence;
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
        var claimsDatabase = MigrateClaimsDatabase(configuration.CosmosDb);
        var auditQueue = InitializeAuditQueue(configuration.RabbitMq);
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
        services
            // NOTE: Suffix suppression is required for "CreatedAtAction" method to work properly.
            .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }

    private static void AddInfrastructure(IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();
    }

    private static void AddRepositories(IServiceCollection services, IClaimsDatabase database)
    {
        services.AddTransient(_ => database.ClaimsRepository);
        services.AddTransient(_ => database.CoversRepository);
    }

    private static void AddDomain(IServiceCollection services)
    {
        services.AddTransient<ICoverPricing, CoverPricing>();
    }

    private static void AddApplicationCommands(IServiceCollection services)
    {
        services.AddTransient<ICommand<CreateCoverArgs, CreateCoverResult>, CreateCoverCommand>();
        services.AddTransient<ICommand<GetCoverArgs, GetCoverResult>, GetCoverCommand>();
        services.AddTransient<ICommandWithNoArgs<GetAllCoversResult>, GetAllCoversCommand>();
        services.AddTransient<ICommandWithNoResult<DeleteCoverArgs>, DeleteCoverCommand>();
        
        services.AddTransient<ICommand<GetCoverPremiumArgs, GetCoverPremiumResult>, GetCoverPremiumCommand>();

        services.AddTransient<ICommand<CreateClaimArgs, CreateClaimResult>, CreateClaimCommand>();
        services.AddTransient<ICommand<GetClaimByIdArgs, GetClaimByIdResult>, GetClaimByIdCommand>();
        services.AddTransient<ICommandWithNoArgs<GetAllClaimsResult>, GetAllClaimsCommand>();
        services.AddTransient<ICommandWithNoResult<DeleteClaimArgs>, DeleteClaimCommand>();
    }

    private static void AddAuditing(IServiceCollection services, IConnectedSendingQueue<AuditMessage> auditQueue)
    {
        services.AddTransient<IClaimAuditor>(_ => new MessageQueueClaimAuditor(auditQueue));
        services.AddTransient<ICoverAuditor>(_ => new MessageQueueCoverAuditor(auditQueue));
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
