using Claims.Application.Commands.GetClaimById;
using Claims.Domain;
using Moq;
using Xunit;
using static Claims.Application.Tests.TestDomainData;

namespace Claims.Application.Tests.Commands;

public class GetClaimByIdCommandTests : ClaimsCommandTests
{
    private readonly GetClaimByIdCommand _command;

    public GetClaimByIdCommandTests()
    {
        _command = new GetClaimByIdCommand(ClaimsRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsClaimByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var claim = RandomClaim();
        StubFindClaim(id, claim);

        var result = await _command.ExecuteAsync(new GetClaimByIdArgs(id));

        Assert.Equal(claim, result.Claim);
    }

    private void StubFindClaim(Guid id, Claim claimToReturn)
    {
        ClaimsRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(claimToReturn);
    }
}