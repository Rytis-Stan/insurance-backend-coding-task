﻿using Claims.Domain;
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
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, 100, created);
        var service = new ClaimsService(new ClaimsRepositoryStub(), new CoversRepositoryStub(coverId, null));

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => service.CreateClaimAsync(request),
            "Cover references a non-existing claim via the claim ID."
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