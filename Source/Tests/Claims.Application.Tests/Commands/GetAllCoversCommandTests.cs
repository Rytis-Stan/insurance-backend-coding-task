using Claims.Application.Commands.GetAllCovers;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests.Commands;

public class GetAllCoversCommandTests : CoversCommandTests
{
    private readonly GetAllCoversCommand _command;

    public GetAllCoversCommandTests()
    {
        _command = new GetAllCoversCommand(CoversRepositoryMock.Object);
    }

    [Fact]
    public async Task ReturnsAllCoversFromRepository()
    {
        var covers = new[] { TestDomainData.RandomCover(), TestDomainData.RandomCover() };
        StubGetAllCovers(covers);

        var response = await _command.ExecuteAsync();

        Assert.Equal(covers, response.Covers);
    }

    private void StubGetAllCovers(IEnumerable<Cover> coversToReturn)
    {
        CoversRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(coversToReturn);
    }
}
