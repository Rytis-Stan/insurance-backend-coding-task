using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;

namespace Claims.Application.Tests;

// TODO: Decide where to put the "RandomCover()" method.
public abstract class CoversCommandTests
{
    protected readonly Mock<ICoversRepository> _coversRepositoryMock;

    protected CoversCommandTests()
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
