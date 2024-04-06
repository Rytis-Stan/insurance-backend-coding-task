using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests.Commands;

public class GetCoverCommandTests : CoversCommandTests
{
    private readonly GetCoverCommand _command;

    public GetCoverCommandTests()
    {
        _command = new GetCoverCommand(_coversRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsCoverByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var cover = RandomCover();
        StubFindCover(id, cover);

        var returnedCover = await _command.ExecuteAsync(new GetCoverRequest(id));

        Assert.Equal(cover, returnedCover);
    }

    private void StubFindCover(Guid id, Cover? coverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }
}
