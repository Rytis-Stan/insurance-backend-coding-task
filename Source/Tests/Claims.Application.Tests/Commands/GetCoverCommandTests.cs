using Claims.Application.Commands.GetCover;
using Claims.Domain;
using Moq;
using Xunit;
using static Claims.Application.Tests.TestDomainData;

namespace Claims.Application.Tests.Commands;

public class GetCoverCommandTests : CoversCommandTests
{
    private readonly GetCoverCommand _command;

    public GetCoverCommandTests()
    {
        _command = new GetCoverCommand(CoversRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsCoverByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var cover = RandomCover();
        StubFindCover(id, cover);

        var result = await _command.ExecuteAsync(new GetCoverArgs(id));

        Assert.Equal(cover, result.Cover);
    }

    private void StubFindCover(Guid id, Cover? coverToReturn)
    {
        CoversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }
}
