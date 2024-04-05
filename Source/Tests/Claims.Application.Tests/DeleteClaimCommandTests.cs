using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests;

public class DeleteClaimCommandTests : ClaimsCommandTests
{
    private readonly DeleteClaimCommand _command;

    public DeleteClaimCommandTests()
    {
        _command = new DeleteClaimCommand(_claimsRepositoryMock.Object);
    }

    [Fact]
    public async Task DeletesClaimByIdInRepository()
    {
        var id = Guid.NewGuid();

        await _command.DeleteClaimAsync(id);

        _claimsRepositoryMock.Verify(x => x.DeleteByIdAsync(id));
    }

    [Fact]
    public async Task ReturnsClaimReturnedByRepositoryDelete()
    {
        var id = Guid.NewGuid();
        var claim = RandomClaim();
        StubDeleteClaim(id, claim);

        var returnedClaim = await _command.DeleteClaimAsync(id);

        Assert.Equal(claim, returnedClaim);
    }

    private void StubDeleteClaim(Guid id, Claim? deletedClaimToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.DeleteByIdAsync(id))
            .ReturnsAsync(deletedClaimToReturn);
    }
}