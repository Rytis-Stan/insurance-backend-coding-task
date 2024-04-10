using Claims.Application.Commands.GetClaim;
using Claims.Domain;
using Moq;
using Xunit;
using static Claims.Application.Tests.TestDomainData;

namespace Claims.Application.Tests.Commands;

public class GetClaimCommandTests : ClaimsCommandTests
{
    private readonly GetClaimCommand _command;

    public GetClaimCommandTests()
    {
        _command = new GetClaimCommand(ClaimsRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsClaimByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var claim = RandomClaim();
        StubFindClaim(id, claim);

        var result = await _command.ExecuteAsync(new GetClaimArgs(id));

        Assert.Equal(claim, result.Claim);
    }

    private void StubFindClaim(Guid id, Claim claimToReturn)
    {
        ClaimsRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(claimToReturn);
    }
}