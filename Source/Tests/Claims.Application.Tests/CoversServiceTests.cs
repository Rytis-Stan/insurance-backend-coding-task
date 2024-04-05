using BuildingBlocks.Temporal;
using Claims.Application.Commands;
using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;

namespace Claims.Application.Tests;

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
