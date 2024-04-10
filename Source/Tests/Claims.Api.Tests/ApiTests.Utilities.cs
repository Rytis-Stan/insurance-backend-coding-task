using Claims.Api.Contracts.Dto;
using Claims.Api.Contracts.Messages;
using System.Net;
using static BuildingBlocks.Testing.TestData;
using static BuildingBlocks.Testing.TestValueBuilder;

namespace Claims.Api.Tests;

// This partial class segment is dedicated to various utility/helper methods that
// might be needed by various tests.
//
// ReSharper disable once UnusedMember.Global
public partial class ApiTests
{
    private static DateOnly OneYearPlus1DayCoverPeriodEnd(DateOnly startDate)
    {
        // NOTE: Please see the "CoverPricing" class for an explanation about the 1-year insurance duration.
        return startDate.AddYears(1);
    }

    private static CreateClaimRequest RandomCreateClaimRequest(Guid coverId, DateTime created)
    {
        return new CreateClaimRequest(
            coverId,
            RandomString("name"),
            RandomEnum<ClaimDtoType>(),
            RandomInt(10_000),
            created
        );
    }

    private static CreateCoverRequest RandomCreateCoverRequest(DateTime utcNow)
    {
        return RandomCreateCoverRequest(
            utcNow,
            RandomInt(1, 90),
            RandomEnum<CoverDtoType>()
        );
    }

    private static CreateCoverRequest RandomCreateCoverRequest(DateTime utcNow, int periodDurationInDays, CoverDtoType coverType)
    {
        // NOTE: Start and end date should start at least 1 day after UTC Now to avoid the
        // current date changing while the endpoint is being called (can happen if the test
        // starts running just before a day's end).
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(RandomInt(1, 100));
        var endDate = startDate.AddDays(periodDurationInDays - 1);
        return new CreateCoverRequest(startDate, endDate, coverType);
    }

    private async Task<CoverDto> CreateRandomCoverAsync()
    {
        var request = RandomCreateCoverRequest(DateTime.UtcNow);
        var httpResponse = await CoversPostAsync(request);
        var response = await httpResponse.ReadContentAsync<CreateCoverResponse>(HttpStatusCode.Created);
        return response.Cover;
    }

    private async Task<ClaimDto> CreateRandomClaimAsync(CoverDto cover)
    {
        var createClaimRequest = RandomCreateClaimRequest(cover.Id, UtcDateTime(cover.StartDate));
        var createClaimResponse = await ClaimsPostAsync(createClaimRequest);
        var response = await createClaimResponse.ReadContentAsync<CreateClaimResponse>(HttpStatusCode.Created);
        return response.Claim;
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

    private async Task DeleteMultipleClaimsAsync(IEnumerable<Guid> claimIds)
    {
        var tasks = claimIds.Select(ClaimsDeleteAsync);
        await Task.WhenAll(tasks);
    }

    private async Task DeleteMultipleCoversAsync(IEnumerable<Guid> coverIds)
    {
        var tasks = coverIds.Select(CoversDeleteAsync);
        await Task.WhenAll(tasks);
    }

    private async Task<IEnumerable<CoverDto>> CreateRandomCoversAsync(int coverCount)
    {
        var tasks = Enumerable
            .Range(0, coverCount)
            .Select(_ => CreateRandomCoverAsync());
        return await Task.WhenAll(tasks);
    }
}
