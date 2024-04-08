using System.Net;
using Claims.Api.Dto;
using Claims.Testing;
using Xunit;
using static Claims.Testing.TestValueBuilder;

namespace Claims.Api.Tests;

public partial class ApiTests : IDisposable
{
    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaim()
    {
        var cover = await CreateRandomCoverAsync();
        var request = RandomCreateClaimRequestDto(cover.Id, UtcDateTime(cover.StartDate));
        
        var response = await ClaimsPostAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var claim = await response.ReadContentAsync<ClaimDto>();
        Assert.NotNull(claim);
        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal(cover.Id, claim.CoverId);
        Assert.Equal(request.Name, claim.Name);
        Assert.Equal(request.Type, claim.Type);
        Assert.Equal(request.DamageCost, claim.DamageCost);
        Assert.Equal(request.Created, claim.Created);
    }

    private CreateClaimRequestDto RandomCreateClaimRequestDto(Guid coverId, DateTime created)
    {
        return new CreateClaimRequestDto(
            coverId,
            TestData.RandomString("name"),
            TestData.RandomEnum<ClaimTypeDto>(),
            TestData.RandomInt(10_000),
            created
        );
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await ClaimsGetAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var claims = await response.ReadContentAsync<ClaimDto[]>();
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await ClaimsGetAsync(id);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsClaimWhenItWasAlreadyCreatedPreviously()
    {
        var createdCover = await CreateRandomCoverAsync();
        var createdClaim = await CreateRandomClaimAsync(createdCover);

        var response = await ClaimsGetAsync(createdClaim.Id);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var claim = await response.ReadContentAsync<ClaimDto>();
        Assert.Equal(createdClaim, claim);
    }

    private async Task<IEnumerable<ClaimDto>> CreateRandomCoverWithClaimsAsync(int claimCount)
    {
        var cover = await CreateRandomCoverAsync();
        var tasks = Enumerable
            .Range(0, claimCount)
            .Select(_ => CreateRandomClaimAsync(cover));
        return await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task ClaimsDeleteWithIdReturnsNoContentWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await ClaimsDeleteAsync(id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // TODO: How should this method be named to make it clear that it does not just construct a DTO but actually calls an endpoint for the Cover creation?
    private async Task<CoverDto> CreateRandomCoverAsync()
    {
        var request = RandomCreateCoverRequestDto(DateTime.UtcNow);
        var response = await CoversPostAsync(request);
        return (await response.ReadContentAsync<CoverDto>())!;
    }

    private async Task<ClaimDto> CreateRandomClaimAsync(CoverDto cover)
    {
        var createClaimRequest = RandomCreateClaimRequestDto(cover.Id, UtcDateTime(cover.StartDate));
        var createClaimResponse = await ClaimsPostAsync(createClaimRequest);
        return (await createClaimResponse.ReadContentAsync<ClaimDto>())!;
    }
}
