﻿using System.Net;
using Claims.Api.Dto;
using Claims.Testing;
using Xunit;
using static Claims.Testing.TestValueBuilder;

namespace Claims.Api.Tests;

public partial class ApiTests : IDisposable
{
    [Fact]
    public async Task ClaimsPostReturnsBadRequestWhenCoverWithGivenIdDoesNotExist()
    {
        var coverId = Guid.NewGuid();
        var request = RandomCreateClaimRequestDto(coverId, DateTime.UtcNow);

        var response = await ClaimsPostAsync(request);

        await AssertReturnedBadRequestAsync(response, "Claim references a non-existing cover via the cover ID.");
    }

    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaimWhenRequestValid()
    {
        var cover = await CreateRandomCoverAsync();
        var request = RandomCreateClaimRequestDto(cover.Id, UtcDateTime(cover.StartDate));
        
        var httpResponse = await ClaimsPostAsync(request);

        var response = await httpResponse.ReadContentAsync<CreateClaimResponse>(HttpStatusCode.OK);
        var claim = response!.Claim;
        Assert.NotNull(claim);
        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal(cover.Id, claim.CoverId);
        Assert.Equal(request.Name, claim.Name);
        Assert.Equal(request.Type, claim.Type);
        Assert.Equal(request.DamageCost, claim.DamageCost);
        Assert.Equal(request.Created, claim.Created);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var httpResponse = await ClaimsGetAsync();

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        var claims = await httpResponse.ReadContentAsync<ClaimDto[]>();
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task ClaimsGetReturnsAllClaimsWhenClaimsAdded(int claimCount)
    {
        var createdClaims = await CreateRandomCoverWithClaimsAsync(claimCount);
        
        var httpResponse = await ClaimsGetAsync();

        var claims = await httpResponse.ReadContentAsync<ClaimDto[]>(HttpStatusCode.OK);
        AssertExtended.EqualIgnoreOrder(createdClaims, claims);
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
        await ClaimsDeleteMultipleAsync(createdClaimsToDelete.Select(x => x.Id));

        var httpResponse = await ClaimsGetAsync();

        var claims = await httpResponse.ReadContentAsync<ClaimDto[]>(HttpStatusCode.OK);
        AssertExtended.EqualIgnoreOrder(createdClaimsToKeep, claims);
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

        var claim = await httpResponse.ReadContentAsync<ClaimDto>(HttpStatusCode.OK);
        Assert.Equal(createdClaim, claim);
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

    // TODO: How should this method be named to make it clear that it does not just construct a DTO but actually calls an endpoint for the Cover creation?
    private async Task<CoverDto> CreateRandomCoverAsync()
    {
        var request = RandomCreateCoverRequestDto(DateTime.UtcNow);
        var httpResponse = await CoversPostAsync(request);
        var response = await httpResponse.ReadContentAsync<CreateCoverResponse>();
        return response!.Cover;
    }

    private async Task<ClaimDto> CreateRandomClaimAsync(CoverDto cover)
    {
        var createClaimRequest = RandomCreateClaimRequestDto(cover.Id, UtcDateTime(cover.StartDate));
        var createClaimResponse = await ClaimsPostAsync(createClaimRequest);
        return (await createClaimResponse.ReadContentAsync<ClaimDto>());
    }

    private async Task<IEnumerable<ClaimDto>> CreateRandomCoverWithClaimsAsync(int claimCount)
    {
        var cover = await CreateRandomCoverAsync();
        return await CreateRandomClaimsAsync(cover, claimCount);
    }

    private async Task<IEnumerable<ClaimDto>> CreateRandomClaimsAsync(CoverDto cover, int claimCount)
    {
        var tasks = Enumerable
            .Range(0, claimCount)
            .Select(_ => CreateRandomClaimAsync(cover));
        return await Task.WhenAll(tasks);
    }

    private static CreateClaimRequest RandomCreateClaimRequestDto(Guid coverId, DateTime created)
    {
        return new CreateClaimRequest(
            coverId,
            TestData.RandomString("name"),
            TestData.RandomEnum<ClaimTypeDto>(),
            TestData.RandomInt(10_000),
            created
        );
    }

    private async Task ClaimsDeleteMultipleAsync(IEnumerable<Guid> claimIds)
    {
        var tasks = claimIds.Select(ClaimsDeleteAsync);
        await Task.WhenAll(tasks);
    }

    // TODO: Move to "ExtendedAssert"?
    private static async Task AssertReturnedBadRequestAsync(HttpResponseMessage httpResponse, string expectedErrorMessage)
    {
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        var response = await httpResponse.ReadContentAsync<ValidationErrorResponse>();
        Assert.Equal(
            new ValidationErrorResponse(new ValidationErrorDto(expectedErrorMessage)),
            response
        );
    }
}
