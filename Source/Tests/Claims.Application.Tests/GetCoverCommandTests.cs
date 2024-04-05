using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests;

public class GetCoverCommandTests : CoversServiceTests
{
    private readonly IGetCoverCommand _getCoverCommand;

    public GetCoverCommandTests()
    {
        _getCoverCommand = new GetCoverCommand(_coversRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsCoverByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var cover = RandomCover();
        StubFindCover(id, cover);

        var returnedCover = await _getCoverCommand.GetCoverAsync(id);

        Assert.Equal(cover, returnedCover);
    }

    private void StubFindCover(Guid id, Cover? coverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }
}
