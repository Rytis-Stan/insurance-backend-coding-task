using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests;

public class GetClaimByIdCommandTests : ClaimsCommandTests
{
    private readonly GetClaimByIdCommand _command;

    public GetClaimByIdCommandTests()
    {
        _command = new GetClaimByIdCommand(_claimsRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsClaimByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var claim = RandomClaim();
        StubFindClaim(id, claim);

        var returnedClaim = await _command.GetClaimByIdAsync(id);

        Assert.Equal(claim, returnedClaim);
    }

    private void StubFindClaim(Guid id, Claim claimToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(claimToReturn);
    }
}