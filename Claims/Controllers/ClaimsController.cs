using Claims.Auditing;
using Claims.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;
        private readonly IClaimAuditor _auditor;

        public ClaimsController(CosmosDbService cosmosDbService, AuditContext auditContext)
            : this(cosmosDbService, new Auditor(auditContext, new Clock()))
        {
        }

        public ClaimsController(CosmosDbService cosmosDbService, IClaimAuditor auditor)
        {
            _cosmosDbService = cosmosDbService;
            _auditor = auditor;
        }

        [HttpGet]
        public Task<IEnumerable<Claim>> GetAsync()
        {
            return GetAllClaimsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            await CreateClaimAsync(claim);
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
        {
            return DeleteClaimAsync(id);
        }

        [HttpGet("{id}")]
        public Task<Claim> GetAsync(string id)
        {
            return GetClaimByIdAsync(id);
        }

        private async Task CreateClaimAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(claim);
            _auditor.AuditClaim(claim.Id, "POST");
        }

        private Task<IEnumerable<Claim>> GetAllClaimsAsync()
        {
            return _cosmosDbService.GetClaimsAsync();
        }

        private Task DeleteClaimAsync(string id)
        {
            _auditor.AuditClaim(id, "DELETE");
            var deletedClaim = _cosmosDbService.DeleteItemAsync(id);
            return deletedClaim;
        }

        private Task<Claim> GetClaimByIdAsync(string id)
        {
            return _cosmosDbService.GetClaimAsync(id);
        }
    }

    public class CosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(CosmosClient dbClient, string databaseName, string containerName)
        {
            ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await GetAllClaimsAsync();
        }

        private async Task<IEnumerable<Claim>> GetAllClaimsAsync()
        {
            var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<Claim>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Claim> GetClaimAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task AddItemAsync(Claim item)
        {
            return _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public Task DeleteItemAsync(string id)
        {
            return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
        }
    }
}