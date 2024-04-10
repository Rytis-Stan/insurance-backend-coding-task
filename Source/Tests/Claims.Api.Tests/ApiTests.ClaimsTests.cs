using System.Net;
using Claims.Api.Contracts.Messages;
using Xunit;
using static BuildingBlocks.Testing.Temporals;

namespace Claims.Api.Tests;

public partial class ApiTests : IDisposable
{
    [Fact]
    public async Task ClaimsPostReturnsBadRequestWhenCoverWithGivenIdDoesNotExist()
    {
        var coverId = Guid.NewGuid();
        var request = RandomCreateClaimRequest(coverId, DateTime.UtcNow);

        var httpResponse = await ClaimsPostAsync(request);

        await AssertApi.BadRequestAsync(httpResponse, "Claim references a non-existing cover via the cover ID.");
    }

    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaimWhenRequestValid()
    {
        var cover = await CreateRandomCoverAsync();
        var request = RandomCreateClaimRequest(cover.Id, UtcDateTime(cover.StartDate));
        
        var httpResponse = await ClaimsPostAsync(request);

        var response = await httpResponse.ReadContentAsync<CreateClaimResponse>(HttpStatusCode.Created);
        var claim = response.Claim;
        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal(cover.Id, claim.CoverId);
        Assert.Equal(request.Name, claim.Name);
        Assert.Equal(request.Type, claim.Type);
        Assert.Equal(request.DamageCost, claim.DamageCost);
        Assert.Equal(request.Created, claim.Created);
    }

    [Fact]
    public async Task ClaimsPostReturnsLocationOfNewClaimThatCanBeUsedToRetrieveItLaterWhenRequestIsValid()
    {
        var cover = await CreateRandomCoverAsync();
        var request = RandomCreateClaimRequest(cover.Id, UtcDateTime(cover.StartDate));

        var httpCreateResponse = await ClaimsPostAsync(request);

        var httpGetResponse = await _client.GetAsync(httpCreateResponse.Headers.Location);
        var createResponse = await httpCreateResponse.ReadContentAsync<CreateClaimResponse>(HttpStatusCode.Created);
        var getResponse = await httpGetResponse.OkReadContentAsync<GetClaimResponse>();
        Assert.Equal(createResponse.Claim, getResponse.Claim);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var httpResponse = await ClaimsGetAsync();

        var response = await httpResponse.OkReadContentAsync<GetClaimsResponse>();
        Assert.Empty(response.Claims);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task ClaimsGetReturnsAllClaimsWhenClaimsAdded(int claimCount)
    {
        var createdClaims = await CreateRandomCoverWithClaimsAsync(claimCount);
        
        var httpResponse = await ClaimsGetAsync();

        var response = await httpResponse.OkReadContentAsync<GetClaimsResponse>();
        AssertApi.EqualIgnoreOrder(createdClaims, response.Claims);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(4, 1)]
    [InlineData(4, 2)]
    public async Task ClaimsGetReturnsNonDeletedClaimsWhenClaimsAddedAndSomeDeleted(int claimAddCount, int claimDeleteCount)
    {
        var cover = await CreateRandomCoverAsync();
        var createdClaims = (await CreateRandomClaimsAsync(cover, claimAddCount)).ToList();
        var createdClaimsToKeep = createdClaims.Skip(claimDeleteCount);
        var createdClaimsToDelete = createdClaims.Take(claimDeleteCount);
        await DeleteMultipleClaimsAsync(createdClaimsToDelete.Select(x => x.Id));

        var httpResponse = await ClaimsGetAsync();

        var response = await httpResponse.OkReadContentAsync<GetClaimsResponse>();
        AssertApi.EqualIgnoreOrder(createdClaimsToKeep, response.Claims);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenNoClaimCreatedWithGivenId()
    {
        var id = Guid.NewGuid();

        var httpResponse = await ClaimsGetAsync(id);

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsClaimWhenItWasAlreadyCreatedPreviously()
    {
        var createdCover = await CreateRandomCoverAsync();
        var createdClaim = await CreateRandomClaimAsync(createdCover);

        var httpResponse = await ClaimsGetAsync(createdClaim.Id);

        var response = await httpResponse.OkReadContentAsync<GetClaimResponse>();
        Assert.Equal(createdClaim, response.Claim);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenClaimCreatedButLaterDeleted()
    {
        var createdCover = await CreateRandomCoverAsync();
        var createdClaim = await CreateRandomClaimAsync(createdCover);
        await ClaimsDeleteAsync(createdClaim.Id);

        var httpResponse = await ClaimsGetAsync(createdClaim.Id);

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task ClaimsDeleteWithIdReturnsNoContentWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var httpResponse = await ClaimsDeleteAsync(id);

        Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
    }
}
