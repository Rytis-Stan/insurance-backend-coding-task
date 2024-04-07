using Claims.Application.Commands.GetClaimById;
using Claims.Domain;
using Moq;
using Xunit;

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
        var claim = TestDomainData.RandomClaim();
        StubFindClaim(id, claim);

        var response = await _command.ExecuteAsync(new GetClaimByIdRequest(id));

        Assert.Equal(claim, response.Claim);
    }

    private void StubFindClaim(Guid id, Claim claimToReturn)
    {
        ClaimsRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(claimToReturn);
    }
}