using BuildingBlocks.Testing;
using Claims.Domain;

namespace Claims.Application.Tests;

public static class TestDomainData
{
    public static Cover RandomCover()
    {
        var startDate = TestData.RandomDate();
        var periodDurationInDays = 200;
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = startDate.AddDays(TestData.RandomInt(periodDurationInDays - 1)),
            Type = TestData.RandomEnum<CoverType>(),
            Premium = TestData.RandomInt(100)
        };
    }

    public static Claim RandomClaim()
    {
        return RandomClaim(TestData.RandomUtcDateTime());
    }

    public static Claim RandomClaim(DateTime created)
    {
        return new Claim
        {
            Id = Guid.NewGuid(),
            CoverId = Guid.NewGuid(),
            Name = TestData.RandomString("name"),
            Type = TestData.RandomEnum<ClaimType>(),
            DamageCost = TestData.RandomInt(100),
            Created = created
        };
    }
}
