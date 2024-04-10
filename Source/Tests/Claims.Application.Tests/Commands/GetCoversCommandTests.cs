using Claims.Application.Commands.GetCovers;
using Claims.Domain;
using Moq;
using Xunit;
using static Claims.Application.Tests.TestDomainData;

namespace Claims.Application.Tests.Commands;

public class GetCoversCommandTests : CoversCommandTests
{
    private readonly GetCoversCommand _command;

    public GetCoversCommandTests()
    {
        _command = new GetCoversCommand(CoversRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsAllCoversFromRepository()
    {
        var covers = new[] { RandomCover(), RandomCover() };
        StubGetAllCovers(covers);

        var result = await _command.ExecuteAsync();

        Assert.Equal(covers, result.Covers);
    }

    private void StubGetAllCovers(IEnumerable<Cover> coversToReturn)
    {
        CoversRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(coversToReturn);
    }
}
