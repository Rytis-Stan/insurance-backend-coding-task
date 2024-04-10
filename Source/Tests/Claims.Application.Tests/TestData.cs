using Claims.Domain;
using static BuildingBlocks.Testing.TestData;

namespace Claims.Application.Tests;

public static class TestDomainData
{
    public static Cover RandomCover()
    {
        var startDate = RandomDate();
        var periodDurationInDays = 200;
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = startDate.AddDays(RandomInt(periodDurationInDays - 1)),
            Type = RandomEnum<CoverType>(),
            Premium = RandomInt(100)
        };
    }

    public static Claim RandomClaim()
    {
        return RandomClaim(RandomUtcDateTime());
    }

    public static Claim RandomClaim(DateTime created)
    {
        return new Claim
        {
            Id = Guid.NewGuid(),
            CoverId = Guid.NewGuid(),
            Name = RandomString("name"),
            Type = RandomEnum<ClaimType>(),
            DamageCost = RandomInt(100),
            Created = created
        };
    }
}
