using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;

namespace Claims.Application.Tests;

// TODO: Decide what to do with this class. Should this become a base "CoversCommandTests" class?
// TODO: Decide where to put the "RandomCover()" method.
public class CoversServiceTests
{
    protected readonly Mock<ICoversRepository> _coversRepositoryMock;

    public CoversServiceTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
    }

    protected static Cover RandomCover()
    {
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = TestData.RandomDate(),
            EndDate = TestData.RandomDate(),
            Type = TestData.RandomEnum<CoverType>(),
            Premium = TestData.RandomInt(100)
        };
    }
}
