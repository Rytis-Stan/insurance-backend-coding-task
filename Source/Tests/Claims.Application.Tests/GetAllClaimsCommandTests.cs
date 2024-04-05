using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests;

public class GetAllClaimsCommandTests : ClaimsCommandTests
{
    private readonly IGetAllClaimsCommand _getAllClaimsCommand;

    public GetAllClaimsCommandTests()
    {
        _getAllClaimsCommand = new GetAllClaimsCommand(_claimsRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsAllClaimsFromRepository()
    {
        var claims = new[] { RandomClaim(), RandomClaim() };
        StubGetAllClaims(claims);

        var returnedClaims = await _getAllClaimsCommand.GetAllClaimsAsync();

        Assert.Equal(claims, returnedClaims);
    }

    private void StubGetAllClaims(IEnumerable<Claim> claimsToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(claimsToReturn);
    }
}