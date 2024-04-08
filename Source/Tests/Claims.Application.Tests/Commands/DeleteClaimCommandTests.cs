using Claims.Application.Commands.DeleteClaim;
using Xunit;

namespace Claims.Application.Tests.Commands;

public class DeleteClaimCommandTests : ClaimsCommandTests
{
    private readonly DeleteClaimCommand _command;

    public DeleteClaimCommandTests()
    {
        _command = new DeleteClaimCommand(ClaimsRepositoryMock.Object);
    }

    [Fact]
    public async Task DeletesClaimByIdInRepository()
    {
        var id = Guid.NewGuid();

        await _command.ExecuteAsync(new DeleteClaimArgs(id));

        ClaimsRepositoryMock.Verify(x => x.DeleteByIdAsync(id));
    }
}