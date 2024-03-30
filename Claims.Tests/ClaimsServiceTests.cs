using Claims.Domain;
using Claims.Dto;
using Xunit;

namespace Claims.Tests;

public class ClaimsServiceTests
{
    private const ClaimType AnyClaimType = ClaimType.BadWeather;
    private const decimal AnyDamageCost = 10.00m;
    private const CoverType AnyCoverType = CoverType.BulkCarrier;
    private const decimal AnyPremium = 100.00m;

    [Theory]
    [InlineData(100_001)]
    [InlineData(100_002)]
    [InlineData(100_003)]
    public async Task ThrowsExceptionWhenCreatingAClaimWithDamageCostsExceedingMaxAllowed(int damageCost)
    {
        var coverId = Guid.NewGuid();
        var created = new DateTime(2000, 01, 01); // TODO: Random date-time value within a valid range?
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, damageCost, created);
        var service = new ClaimsService(new ClaimsRepositoryStub(), new CoversRepositoryStub());

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => service.CreateClaimAsync(request),
            "Damage cost cannot exceed 100.000."
        );
    }

    // TODO: Should the thrown exception be an ArgumentException?
    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimWithNonExistingCoverId()
    {
        var coverId = Guid.NewGuid();
        var created = new DateTime(2000, 01, 01); // TODO: Random date-time value within a valid range?
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, AnyDamageCost, created);
        var service = new ClaimsService(new ClaimsRepositoryStub(), new CoversRepositoryStub(coverId, null));

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => service.CreateClaimAsync(request),
            "Claim references a non-existing cover via the cover ID."
        );
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimForADateOutsideOfRelatedCoverPeriod()
    {
        // TODO: Use UTC datetime!!!
        await Test(
            new DateOnly(2000, 10, 20),
            new DateOnly(2000, 10, 20),
            new DateTime(2000, 10, 18)
        );
        await Test(
            new DateOnly(2000, 10, 20),
            new DateOnly(2000, 10, 20),
            new DateTime(2000, 10, 19)
        );

        async Task Test(DateOnly coverStartDate, DateOnly coverEndDate, DateTime claimCreated)
        {
            var coverId = Guid.NewGuid();
            var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, 100, claimCreated);
            var cover = new Cover
            {
                Id = coverId,
                StartDate = coverStartDate,
                EndDate = coverEndDate,
                Type = AnyCoverType,
                Premium = AnyPremium
            };
            var service = new ClaimsService(new ClaimsRepositoryStub(), new CoversRepositoryStub(coverId, cover));

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => service.CreateClaimAsync(request),
                "Claim is outside of the related cover period."
            );
        }
    }

    private class ClaimsRepositoryStub : IClaimsRepository
    {
        public Task<Claim> AddAsync(INewClaimInfo item)
        {
            throw new NotImplementedException();
        }

        public Task<Claim?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Claim>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Claim> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
