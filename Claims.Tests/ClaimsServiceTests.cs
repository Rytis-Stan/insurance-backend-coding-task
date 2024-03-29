using Claims.Domain;
using Claims.Dto;
using Xunit;

namespace Claims.Tests;

public class ClaimsServiceTests
{
    private const ClaimType AnyClaimType = ClaimType.BadWeather;

    [Theory]
    [InlineData(100_001)]
    [InlineData(100_002)]
    [InlineData(100_003)]
    public async Task ThrowsExceptionWhenCreatingAClaimWithDamageCostsExceedingMaxAllowed(int damageCost)
    {
        var coverId = Guid.NewGuid();
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, damageCost);
        var service = new ClaimsService(new ClaimsRepositoryStub());

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => service.CreateClaimAsync(request),
            "Damage cost cannot exceed 100.000."
        );
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
