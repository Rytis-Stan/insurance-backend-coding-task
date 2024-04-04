using Claims.Api.Dto;
using Claims.Domain;

namespace Claims.Api.Tests;

// This partial class segment contains methods for calling all possible API endpoints.
//
// The reasons for having dedicated methods:
// 1. It makes the endpoint calls slightly shorter, especially if some DTO objects need to be passed in;
// 2. It allows easy reuse of same endpoint calls in multiple tests (because some tests require calling
//    not just the endpoint under test, but others as well, for the purpose of setting up the correct
//    API state for the main endpoint to be tested properly).
//
// ReSharper disable once UnusedMember.Global
public partial class ApiTests
{
    private Task<HttpResponseMessage> CoversPostAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return _client.PostAsync("/Covers", new CreateCoverRequestDto(startDate, endDate, coverType));
    }

    private Task<HttpResponseMessage> CoversGetAsync()
    {
        return _client.GetAsync("/Covers");
    }

    private Task<HttpResponseMessage> CoversGetAsync(Guid id)
    {
        return _client.GetAsync($"/Covers/{id}");
    }

    private Task<HttpResponseMessage> CoversDeleteAsync(Guid id)
    {
        return _client.DeleteAsync($"/Covers/{id}");
    }

    private Task<HttpResponseMessage> CoversPremiumGetAsync(string startDate, string endDate, CoverType coverType)
    {
        return _client.GetAsync($"/Covers/Premium/?startDate={startDate}&endDate={endDate}&coverType={coverType}");
    }

    private Task<HttpResponseMessage> ClaimsPostAsync(Guid coverId, string name, ClaimType claimType, int damageCost, DateTime created)
    {
        return _client.PostAsync("/Claims", new CreateClaimRequestDto(coverId, name, claimType, damageCost, created));
    }

    private Task<HttpResponseMessage> ClaimsGetAsync()
    {
        return _client.GetAsync("/Claims");
    }

    private Task<HttpResponseMessage> ClaimsGetAsync(Guid id)
    {
        return _client.GetAsync($"/Claims/{id}");
    }

    private Task<HttpResponseMessage> ClaimsDeleteAsync(Guid id)
    {
        return _client.DeleteAsync($"/Claims/{id}");
    }
}