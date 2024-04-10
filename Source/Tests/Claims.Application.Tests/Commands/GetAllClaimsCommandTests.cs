using Claims.Application.Commands.GetAllClaims;
using Claims.Domain;
using Moq;
using Xunit;
using static Claims.Application.Tests.TestDomainData;

namespace Claims.Application.Tests.Commands;

public class GetAllClaimsCommandTests : ClaimsCommandTests
{
    private readonly GetAllClaimsCommand _command;

    public GetAllClaimsCommandTests()
    {
        _command = new GetAllClaimsCommand(ClaimsRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsAllClaimsFromRepository()
    {
        var claims = new[] { RandomClaim(), RandomClaim() };
        StubGetAllClaims(claims);

        var result = await _command.ExecuteAsync();

        Assert.Equal(claims, result.Claims);
    }

    private void StubGetAllClaims(IEnumerable<Claim> claimsToReturn)
    {
        ClaimsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(claimsToReturn);
    }
}