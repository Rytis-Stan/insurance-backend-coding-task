using Claims.Application.Commands.GetCover;
using Claims.Domain;
using Moq;
using Xunit;

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
        var cover = TestDomainData.RandomCover();
        StubFindCover(id, cover);

        var response = await _command.ExecuteAsync(new GetCoverRequest(id));

        Assert.Equal(cover, response.Cover);
    }

    private void StubFindCover(Guid id, Cover? coverToReturn)
    {
        CoversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }
}
