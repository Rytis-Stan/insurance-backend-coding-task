using System.Net;
using BuildingBlocks.Testing;
using Claims.Api.Contracts.Dto;
using Claims.Api.Contracts.Messages;
using Xunit;

namespace Claims.Api.Tests;

public static class AssertApi
{
    public static async Task BadRequestAsync(HttpResponseMessage httpResponse, string expectedErrorMessage)
    {
        var response = await httpResponse.ReadContentAsync<ValidationErrorResponse>(HttpStatusCode.BadRequest);
        Assert.Equal(
            new ValidationErrorResponse(new ValidationErrorDto(expectedErrorMessage)),
            response
        );
    }

    public static void EqualIgnoreOrder(IEnumerable<CoverDto>? expected, IEnumerable<CoverDto>? actual)
    {
        AssertExtended.EqualIgnoreOrder(expected, actual, x => x.Id);
    }

    public static void EqualIgnoreOrder(IEnumerable<ClaimDto>? expected, IEnumerable<ClaimDto>? actual)
    {
        AssertExtended.EqualIgnoreOrder(expected, actual, x => x.Id);
    }
}
