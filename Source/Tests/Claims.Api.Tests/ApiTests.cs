using System.Net;
using Claims.Api.Configuration;
using Claims.Api.Dto;
using Claims.Domain;
using Claims.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Claims.Api.Tests;

public class ApiTests : IDisposable
{
    private readonly HttpClient _client;

    public ApiTests()
    {
        _client = new TestWebApplicationFactory<Program>().CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        
        // TODO: Rewrite tests so that the databases can be deleted only once for all the API tests?
        DeleteCosmosDbIfExists();
        DeleteSqlServerDatabaseIfExists();
    }

    private void DeleteCosmosDbIfExists()
    {
        var configuration = CosmosDbConfiguration();
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        try
        {
            cosmosClient.GetDatabase(configuration.DatabaseName).DeleteAsync().GetAwaiter().GetResult();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Database was not found. Do nothing.
        }
    }

    private void DeleteSqlServerDatabaseIfExists()
    {
        // TODO: Implement database deletion logic.
    }

    private CosmosDbConfiguration CosmosDbConfiguration()
    {
        return AppConfiguration.FromConfiguration(
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
        ).CosmosDb;
    }

    [Theory]
    [InlineData(1, CoverType.BulkCarrier, 1625.00)]
    [InlineData(179, CoverType.Tanker, 330037.50)]
    [InlineData(182, CoverType.Yacht, 239717.50)]
    public async Task CoversPostReturnsNewlyCreatedCover(int periodDurationInDays, CoverType coverType, decimal expectedPremium)
    {
        var utcNow = DateTime.UtcNow;
        // NOTE: Start and end date should start at least 1 day after UTC Now to avoid the
        // current date changing while the endpoint is being called (can happen if the
        // test starts running just before a day's end).
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var endDate = startDate.AddDays(periodDurationInDays - 1);
        
        var response = await _client.PostAsync("/Covers", new CreateCoverRequestDto(startDate, endDate, coverType));

        response.EnsureSuccessStatusCode();
        var cover = await response.ReadContentAsync<CoverDto>();

        Assert.NotNull(cover);
        Assert.NotEqual(Guid.Empty, cover.Id);
        Assert.Equal(startDate, cover.StartDate);
        Assert.Equal(endDate, cover.EndDate);
        Assert.Equal(coverType, cover.Type);
        Assert.Equal(expectedPremium, cover.Premium);
    }

    [Fact]
    public async Task CoversGetReturnEmptyCoverCollectionWhenNoClaimsAdded()
    {
        var response = await _client.GetAsync("/Covers");
        
        response.EnsureSuccessStatusCode();
        var covers = await response.ReadContentAsync<CoverDto[]>();
        Assert.NotNull(covers);
        Assert.Empty(covers);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/Covers/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CoversDeleteWithIdReturnsNoContentWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/Covers/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Theory]
    [InlineData("2001-01-01", "2001-01-01", CoverType.Yacht, 1375.00)]
    [InlineData("2024-03-02", "2024-03-31", CoverType.ContainerShip, 48750)]
    [InlineData("1995-06-05", "1995-12-04", CoverType.Tanker, 337331.25)]
    public async Task CoversPremiumGetReturnsCalculatedPremiumForGivenPeriodBasedOnCoverType(
        string startDate, string endDate, CoverType coverType, decimal expectedPremium)
    {
        var response = await _client.GetAsync($"/Covers/Premium/?startDate={startDate}&endDate={endDate}&coverType={coverType}");

        response.EnsureSuccessStatusCode();
        var premium = await response.ReadContentAsync<decimal>();
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaim()
    {
        var utcNow = DateTime.UtcNow;
        var coverStartDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var coverPeriodDurationInDays = 200;
        var coverEndDate = coverStartDate.AddDays(TestData.RandomInt(coverPeriodDurationInDays - 1));
        var coverType = TestData.RandomEnum<CoverType>();
        var coversResponse = await _client.PostAsync("/Covers", new CreateCoverRequestDto(coverStartDate, coverEndDate, coverType));
        coversResponse.EnsureSuccessStatusCode();
        var cover = await coversResponse.ReadContentAsync<CoverDto>();

        var coverId = cover!.Id;
        var name = TestData.RandomString("name");
        var claimType = TestData.RandomEnum<ClaimType>();
        var damageCost = TestData.RandomInt(10_000);
        var created = TestValueBuilder.UtcDateTime(coverStartDate);

        var claimsResponse = await _client.PostAsync("/Claims", new CreateClaimRequestDto(coverId, name, claimType, damageCost, created));

        claimsResponse.EnsureSuccessStatusCode();
        var claim = await claimsResponse.ReadContentAsync<ClaimDto>();
        Assert.NotNull(claim);
        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal(coverId, claim.CoverId);
        Assert.Equal(created, claim.Created);
        Assert.Equal(name, claim.Name);
        Assert.Equal(claimType, claim.Type);
        Assert.Equal(damageCost, claim.DamageCost);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await _client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();
        var claims = await response.ReadContentAsync<ClaimDto[]>();
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/Claims/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ClaimsDeleteWithIdReturnsNoContentWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/Claims/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
