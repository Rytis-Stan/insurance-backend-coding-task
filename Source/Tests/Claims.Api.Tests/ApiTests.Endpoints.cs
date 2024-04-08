using Claims.Api.Dto;

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
    private async Task<HttpResponseMessage> CoversPostAsync(CreateCoverRequest request)
    {
        return await _client.PostAsync("/Covers", request);
    }

    private async Task<HttpResponseMessage> CoversGetAsync()
    {
        return await _client.GetAsync("/Covers");
    }

    private async Task<HttpResponseMessage> CoversGetAsync(Guid id)
    {
        return await _client.GetAsync($"/Covers/{id}");
    }

    private async Task<HttpResponseMessage> CoversDeleteAsync(Guid id)
    {
        return await _client.DeleteAsync($"/Covers/{id}");
    }

    private async Task<HttpResponseMessage> CoversPremiumGetAsync(DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        return await CoversPremiumGetAsync($"{startDate:yyyy-MM-dd}", $"{endDate:yyyy-MM-dd}", coverType);
    }

    private async Task<HttpResponseMessage> CoversPremiumGetAsync(string startDate, string endDate, CoverTypeDto coverType)
    {
        return await _client.GetAsync($"/Covers/Premium/?startDate={startDate}&endDate={endDate}&coverType={coverType}");
    }

    private async Task<HttpResponseMessage> ClaimsPostAsync(CreateClaimRequest request)
    {
        return await _client.PostAsync("/Claims", request);
    }

    private async Task<HttpResponseMessage> ClaimsGetAsync()
    {
        return await _client.GetAsync("/Claims");
    }

    private async Task<HttpResponseMessage> ClaimsGetAsync(Guid id)
    {
        return await _client.GetAsync($"/Claims/{id}");
    }

    private async Task<HttpResponseMessage> ClaimsDeleteAsync(Guid id)
    {
        return await _client.DeleteAsync($"/Claims/{id}");
    }
}