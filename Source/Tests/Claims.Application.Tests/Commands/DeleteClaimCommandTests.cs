using Claims.Application.Commands;
using Xunit;

namespace Claims.Application.Tests.Commands;

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

        await _command.ExecuteAsync(new DeleteClaimRequest(id));

        _claimsRepositoryMock.Verify(x => x.DeleteByIdAsync(id));
    }
}