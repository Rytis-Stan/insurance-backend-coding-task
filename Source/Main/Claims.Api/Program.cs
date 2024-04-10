using System.Text.Json.Serialization;
using Auditing.Auditors;
using Auditing.Auditors.MessageQueueBased;
using BuildingBlocks.MessageQueues.RabbitMq;
using BuildingBlocks.Temporal;
using Claims.Api.Configuration;
using Claims.Api.DependencyInjection;
using Claims.Application.Commands;
using Claims.Application.Commands.CreateClaim;
using Claims.Application.Commands.CreateCover;
using Claims.Application.Commands.DeleteClaim;
using Claims.Application.Commands.DeleteCover;
using Claims.Application.Commands.GetAllClaims;
using Claims.Application.Commands.GetAllCovers;
using Claims.Application.Commands.GetClaim;
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
        AddServices(builder.Services, claimsDatabase, configuration.RabbitMq);
        var app = builder.Build();
        ConfigureApp(app);
        return app;
    }

    private static IClaimsDatabase MigrateClaimsDatabase(CosmosDbConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        var database = new CosmosDbClaimsDatabase(cosmosClient, configuration.DatabaseName, new RandomIdGenerator());
        database.InitializeAsync().GetAwaiter().GetResult();
        return database;
    }

    private static void AddServices(IServiceCollection services, IClaimsDatabase claimsDatabase, RabbitMqConfiguration configuration)
    {
        AddControllers(services);
        AddRepositories(services, claimsDatabase);
        AddInfrastructure(services);
        AddDomain(services);
        AddApplicationCommands(services);
        AddAuditing(services, configuration);
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
        services.AddTransient<ICommand<GetClaimArgs, GetClaimResult>, GetClaimCommand>();
        services.AddTransient<ICommandWithNoArgs<GetAllClaimsResult>, GetAllClaimsCommand>();
        services.AddTransient<ICommandWithNoResult<DeleteClaimArgs>, DeleteClaimCommand>();
    }

    private static void AddAuditing(IServiceCollection services, RabbitMqConfiguration configuration)
    {
        AddQueueAuditor<ClaimAuditorSource>(services, configuration.HostName, configuration.QueueNames.ClaimAudit);
        AddQueueAuditor<CoverAuditorSource>(services, configuration.HostName, configuration.QueueNames.CoverAudit);
    }

    private static void AddQueueAuditor<TSource>(IServiceCollection services, string hostName, string queueName)
        where TSource : SourceOf<IHttpRequestAuditor>, new()
    {
        var queue = new RabbitMqSendingQueue<AuditMessage>(hostName, queueName).StartSending();
        services.AddTransient(_ => new TSource { Obj = new MessageQueueAuditor(queue) });
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
