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
    protected readonly Mock<ICoverPricing> _coverPricingMock;
    protected readonly Mock<IClock> _clockMock;

    public CoversServiceTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _coverPricingMock = new Mock<ICoverPricing>();
        _clockMock = new Mock<IClock>();
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

    protected void StubUtcNow(DateTime utcNow)
    {
        _clockMock
            .Setup(x => x.UtcNow())
            .Returns(utcNow);
    }
}
