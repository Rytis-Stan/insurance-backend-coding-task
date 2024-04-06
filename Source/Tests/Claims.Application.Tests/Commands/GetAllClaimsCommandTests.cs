using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests.Commands;

public class GetAllClaimsCommandTests : ClaimsCommandTests
{
    private readonly GetAllClaimsCommand _command;

    public GetAllClaimsCommandTests()
    {
        _command = new GetAllClaimsCommand(_claimsRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsAllClaimsFromRepository()
    {
        var claims = new[] { TestDomainData.RandomClaim(), TestDomainData.RandomClaim() };
        StubGetAllClaims(claims);

        var returnedClaims = await _command.ExecuteAsync();

        Assert.Equal(claims, returnedClaims);
    }

    private void StubGetAllClaims(IEnumerable<Claim> claimsToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(claimsToReturn);
    }
}