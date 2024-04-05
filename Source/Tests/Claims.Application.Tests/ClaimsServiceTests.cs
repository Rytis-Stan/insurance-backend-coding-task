using Claims.Application.Commands;
using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;
using Xunit;
using static Claims.Testing.TestValueBuilder;

namespace Claims.Application.Tests;

public class CreateClaimCommandTests : ClaimsServiceTests
{
    private const ClaimType AnyClaimType = ClaimType.BadWeather;
    private const decimal AnyDamageCost = 10.00m;
    private const CoverType AnyCoverType = CoverType.BulkCarrier;
    private const decimal AnyPremium = 100.00m;

    private readonly ICreateClaimCommand _createClaimCommand;

    public CreateClaimCommandTests()
    {
        _createClaimCommand = _claimsService;
    }

    [Theory]
    [InlineData(-2.00)]
    [InlineData(-1.00)]
    [InlineData(-0.01)]
    [InlineData(0.00)]
    public async Task ThrowsExceptionWhenCreatingAClaimWithNonPositiveDamageCost(decimal damageCost)
    {
        var coverId = Guid.NewGuid();
        var created = TestData.RandomUtcDateTime();
        var request = new CreateClaimRequest(coverId, "anyName", AnyClaimType, damageCost, created);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _createClaimCommand.CreateClaimAsync(request),
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
        var created = TestData.RandomUtcDateTime();
        var request = new CreateClaimRequest(coverId, "anyName", AnyClaimType, damageCost, created);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _createClaimCommand.CreateClaimAsync(request),
            "Damage cost cannot exceed 100000."
        );
    }

    // TODO: Should the thrown exception be an ArgumentException?
    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimWithNonExistingCoverId()
    {
        var coverId = Guid.NewGuid();
        var created = TestData.RandomUtcDateTime();
        var request = new CreateClaimRequest(coverId, "anyName", AnyClaimType, AnyDamageCost, created);
        StubFindCover(coverId, null);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _createClaimCommand.CreateClaimAsync(request),
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
            var request = new CreateClaimRequest(cover.Id, "anyName", AnyClaimType, AnyDamageCost, claimCreated);
            StubFindCover(cover.Id, cover);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _createClaimCommand.CreateClaimAsync(request),
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
            var request = new CreateClaimRequest(cover.Id, claimName, claimType, claimDamageCost, claimCreated);
            StubFindCover(cover.Id, cover);

            await _createClaimCommand.CreateClaimAsync(request);

            _claimsRepositoryMock.Verify(x => x.CreateAsync(new NewClaimInfo(cover.Id, claimName, claimType, claimDamageCost, claimCreated)));
        }
    }

    private static Cover CreateCoverForPeriod(DateOnly startDate, DateOnly endDate)
    {
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate,
            Type = AnyCoverType,
            Premium = AnyPremium
        };
    }

    private void StubFindCover(Guid id, Cover? coverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }
}

public class GetClaimByIdCommandTests : ClaimsServiceTests
{
    private readonly IGetClaimByIdCommand _getClaimByIdCommand;

    public GetClaimByIdCommandTests()
    {
        _getClaimByIdCommand = _claimsService;
    }

    [Fact]
    public async Task ReturnsClaimByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var claim = RandomClaim();
        StubFindClaim(id, claim);

        var returnedClaim = await _getClaimByIdCommand.GetClaimByIdAsync(id);

        Assert.Equal(claim, returnedClaim);
    }

    private void StubFindClaim(Guid id, Claim claimToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(claimToReturn);
    }
}

public class GetAllClaimsCommandTests : ClaimsServiceTests
{
    private readonly IGetAllClaimsCommand _getAllClaimsCommand;

    public GetAllClaimsCommandTests()
    {
        _getAllClaimsCommand = _claimsService;
    }

    [Fact]
    public async Task ReturnsAllClaimsFromRepository()
    {
        var claims = new[] { RandomClaim(), RandomClaim() };
        StubGetAllClaims(claims);

        var returnedClaims = await _getAllClaimsCommand.GetAllClaimsAsync();

        Assert.Equal(claims, returnedClaims);
    }

    private void StubGetAllClaims(IEnumerable<Claim> claimsToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(claimsToReturn);
    }
}

public class DeleteClaimCommandTests : ClaimsServiceTests
{
    private readonly IDeleteClaimCommand _deleteClaimCommand;

    public DeleteClaimCommandTests()
    {
        _deleteClaimCommand = _claimsService;
    }

    [Fact]
    public async Task DeletesClaimByIdInRepository()
    {
        var id = Guid.NewGuid();

        await _deleteClaimCommand.DeleteClaimAsync(id);

        _claimsRepositoryMock.Verify(x => x.DeleteByIdAsync(id));
    }

    [Fact]
    public async Task ReturnsClaimReturnedByRepositoryDelete()
    {
        var id = Guid.NewGuid();
        var claim = RandomClaim();
        StubDeleteClaim(id, claim);

        var returnedClaim = await _deleteClaimCommand.DeleteClaimAsync(id);

        Assert.Equal(claim, returnedClaim);
    }

    private void StubDeleteClaim(Guid id, Claim? deletedClaimToReturn)
    {
        _claimsRepositoryMock
            .Setup(x => x.DeleteByIdAsync(id))
            .ReturnsAsync(deletedClaimToReturn);
    }
}

public class ClaimsServiceTests
{
    protected readonly Mock<IClaimsRepository> _claimsRepositoryMock;
    protected readonly Mock<ICoversRepository> _coversRepositoryMock;
    protected readonly ClaimsService _claimsService;

    public ClaimsServiceTests()
    {
        _claimsRepositoryMock = new Mock<IClaimsRepository>();
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _claimsService = new ClaimsService(_claimsRepositoryMock.Object, _coversRepositoryMock.Object);
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
