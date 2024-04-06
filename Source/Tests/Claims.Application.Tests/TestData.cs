using Claims.Domain;

namespace Claims.Application.Tests;

// TODO: Figure out how to organize both this class and the other "TestData" class and figure out the proper naming.
//       The other class should probably become something like "RandomExtensions", or something similar.
public static class TestDomainData
{
    // TODO: Figure out if this object should be constructed with "real" relationships between its fields (the "EndDate" could go after the "StartDate" for example).
    public static Cover RandomCover()
    {
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = Testing.TestData.RandomDate(),
            EndDate = Testing.TestData.RandomDate(),
            Type = Testing.TestData.RandomEnum<CoverType>(),
            Premium = Testing.TestData.RandomInt(100)
        };
    }

    public static Claim RandomClaim()
    {
        return new Claim
        {
            Id = Guid.NewGuid(),
            CoverId = Guid.NewGuid(),
            Name = Testing.TestData.RandomString("name"),
            Type = Testing.TestData.RandomEnum<ClaimType>(),
            DamageCost = Testing.TestData.RandomInt(100),
            Created = Testing.TestData.RandomUtcDateTime()
        };
    }
}
