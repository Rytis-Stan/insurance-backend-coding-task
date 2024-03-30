﻿using Claims.Domain;
using Claims.Dto;
using Moq;
using Xunit;
using static Claims.Tests.TestValueBuilder;

namespace Claims.Tests;

public class ClaimsServiceTests
{
    private const ClaimType AnyClaimType = ClaimType.BadWeather;
    private const decimal AnyDamageCost = 10.00m;
    private const CoverType AnyCoverType = CoverType.BulkCarrier;
    private const decimal AnyPremium = 100.00m;

    private readonly Mock<IClaimsRepository> _claimsRepositoryMock;
    private readonly Mock<ICoversRepository> _coversRepositoryMock;
    private readonly ClaimsService _claimsService;

    public ClaimsServiceTests()
    {
        _claimsRepositoryMock = new Mock<IClaimsRepository>();
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _claimsService = new ClaimsService(_claimsRepositoryMock.Object, _coversRepositoryMock.Object);
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimWithZeroDamageCost()
    {
        var coverId = Guid.NewGuid();
        var created = UtcDateTime(2000, 01, 01); // TODO: Random date-time value within a valid range?
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, 0, created);
        var service = new ClaimsService(_claimsRepositoryMock.Object, _coversRepositoryMock.Object);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => service.CreateClaimAsync(request),
            "Damage cost must be a positive value."
        );
    }

    [Theory]
    [InlineData(100_001.00)]
    [InlineData(100_002.00)]
    [InlineData(100_003.00)]
    public async Task ThrowsExceptionWhenCreatingAClaimWithDamageCostExceedingMaxAllowedValueOfAHundredThousand(decimal damageCost)
    {
        var coverId = Guid.NewGuid();
        var created = UtcDateTime(2000, 01, 01); // TODO: Random date-time value within a valid range?
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, damageCost, created);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _claimsService.CreateClaimAsync(request),
            "Damage cost cannot exceed 100.000."
        );
    }
    
    // TODO: Should the thrown exception be an ArgumentException?
    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimWithNonExistingCoverId()
    {
        var coverId = Guid.NewGuid();
        var created = UtcDateTime(2000, 01, 01); // TODO: Random date-time value within a valid range?
        var request = new CreateClaimRequestDto(coverId, "anyName", AnyClaimType, AnyDamageCost, created);
        StubGetCoverById(coverId, null);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _claimsService.CreateClaimAsync(request),
            "Claim references a non-existing cover via the cover ID."
        );
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimForADateOutsideOfRelatedCoverPeriod()
    {
        await Test(
            Date(2000, 10, 20),
            Date(2000, 10, 20),
            UtcDateTime(2000, 10, 18)
        );
        await Test(
            Date(2000, 10, 20),
            Date(2000, 10, 20),
            UtcDateTime(2000, 10, 19)
        );
        await Test(
            Date(1987, 06, 05),
            Date(1987, 06, 05),
            UtcDateTime(1987, 06, 04)
        );
        await Test(
            Date(1987, 06, 05),
            Date(1987, 06, 05),
            UtcDateTime(1987, 06, 06)
        );
        await Test(
            Date(1987, 06, 05),
            Date(1987, 06, 05),
            UtcDateTime(1987, 06, 07)
        );
        await Test(
            Date(1987, 06, 05),
            Date(1987, 06, 06),
            UtcDateTime(1987, 06, 07)
        );

        async Task Test(DateOnly coverStartDate, DateOnly coverEndDate, DateTime claimCreated)
        {
            var cover = CreateCoverForPeriod(coverStartDate, coverEndDate);
            var request = new CreateClaimRequestDto(cover.Id, "anyName", AnyClaimType, AnyDamageCost, claimCreated);
            StubGetCoverById(cover.Id, cover);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _claimsService.CreateClaimAsync(request),
                "Claim is outside the related cover period."
            );
        }
    }

    [Fact]
    public async Task AddsClaimToRepositoryWhenCreatingAValidClaim()
    {
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "someClaim", ClaimType.BadWeather, 123.45m, UtcDateTime(2000, 10, 10));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.BadWeather, 123.45m, UtcDateTime(2000, 10, 10));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 123.45m, UtcDateTime(2000, 10, 10));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 67.89m, UtcDateTime(2000, 10, 10));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 67.89m, UtcDateTime(2000, 10, 11));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 100000.00m, UtcDateTime(2000, 10, 11));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 100000.00m, UtcDateTime(2000, 10, 14));
        await Test(Date(2000, 10, 10), Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 100000.00m, UtcDateTime(2000, 10, 15));
        await Test(Date(1996, 03, 04), Date(1996, 05, 06), "claimB", ClaimType.Collision, 0.01m, UtcDateTime(1996, 03, 04));
    
        async Task Test(DateOnly coverStartDate, DateOnly coverEndDate, string claimName, ClaimType claimType, decimal claimDamageCost, DateTime claimCreated)
        {
            var cover = CreateCoverForPeriod(coverStartDate, coverEndDate);
            var request = new CreateClaimRequestDto(cover.Id, claimName, claimType, claimDamageCost, claimCreated);
            StubGetCoverById(cover.Id, cover);
    
            await _claimsService.CreateClaimAsync(request);
    
            _claimsRepositoryMock.Verify(x => x.AddAsync(new NewClaimInfo(cover.Id, claimName, claimType, claimDamageCost, claimCreated)));
        }
    }

    private Cover CreateCoverForPeriod(DateOnly coverStartDate, DateOnly coverEndDate)
    {
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = coverStartDate,
            EndDate = coverEndDate,
            Type = AnyCoverType,
            Premium = AnyPremium
        };
    }

    private void StubGetCoverById(Guid coverId, Cover? cover)
    {
        _coversRepositoryMock
            .Setup(x => x.GetByIdAsync(coverId))
            .ReturnsAsync(cover);
    }
}
