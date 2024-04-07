using Claims.Application.Commands;
using Xunit;

namespace Claims.Application.Tests.Commands;

public class DeleteCoverCommandTests : CoversCommandTests
{
    private readonly DeleteCoverCommand _command;

    public DeleteCoverCommandTests()
    {
        _command = new DeleteCoverCommand(CoversRepositoryMock.Object);
    }

    [Fact]
    public async Task DeletesCoverByIdInRepository()
    {
        var id = Guid.NewGuid();

        await _command.ExecuteAsync(new DeleteCoverRequest(id));

        CoversRepositoryMock.Verify(x => x.DeleteByIdAsync(id));
    }
}
