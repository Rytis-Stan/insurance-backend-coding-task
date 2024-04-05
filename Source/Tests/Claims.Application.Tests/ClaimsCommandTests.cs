using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;

namespace Claims.Application.Tests;

// TODO: Decide what to do about this base tests class and the "RandomClaim()" method.
public abstract class ClaimsCommandTests
{
    protected readonly Mock<IClaimsRepository> _claimsRepositoryMock;
    protected readonly Mock<ICoversRepository> _coversRepositoryMock;

    protected ClaimsCommandTests()
    {
        _claimsRepositoryMock = new Mock<IClaimsRepository>();
        _coversRepositoryMock = new Mock<ICoversRepository>();
    }

    protected static Claim RandomClaim()
    {
        return new Claim
        {
            Id = Guid.NewGuid(),
            CoverId = Guid.NewGuid(),
            Name = TestData.RandomString("name"),
            Type = TestData.RandomEnum<ClaimType>(),
            DamageCost = TestData.RandomInt(100),
            Created = TestData.RandomUtcDateTime()
        };
    }
}
