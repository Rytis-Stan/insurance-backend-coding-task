using Claims.Domain;
using Claims.Testing;

namespace Claims.Application.Tests;

// TODO: Figure out how to organize both this class and the other "TestData" class and figure out the proper naming.
//       The other class should probably become something like "RandomExtensions", or something similar.
public static class TestDomainData
{
    // TODO: Figure out if this object should be constructed with "real" relationships between its fields (the "EndDate" could go after the "StartDate" for example).
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
