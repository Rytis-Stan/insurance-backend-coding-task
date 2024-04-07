using Claims.Application.Commands.GetAllClaims;
using Claims.Domain;
using Moq;
using Xunit;

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
        var claims = new[] { TestDomainData.RandomClaim(), TestDomainData.RandomClaim() };
        StubGetAllClaims(claims);

        var response = await _command.ExecuteAsync();

        Assert.Equal(claims, response.Claims);
    }

    private void StubGetAllClaims(IEnumerable<Claim> claimsToReturn)
    {
        ClaimsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(claimsToReturn);
    }
}