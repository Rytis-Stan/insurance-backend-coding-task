using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests;

public class GetAllCoversCommandTests : CoversCommandTests
{
    private readonly GetAllCoversCommand _command;

    public GetAllCoversCommandTests()
    {
        _command = new GetAllCoversCommand(_coversRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsAllCoversFromRepository()
    {
        var covers = new[] { RandomCover(), RandomCover() };
        StubGetAllCovers(covers);

        var returnedCovers = await _command.ExecuteAsync();

        Assert.Equal(covers, returnedCovers);
    }

    private void StubGetAllCovers(IEnumerable<Cover> coversToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(coversToReturn);
    }
}
