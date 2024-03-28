namespace Claims.Domain;

public class Cover
{
    public required Guid Id { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required CoverType Type { get; init; }
    public required decimal Premium { get; init; }
    public required DateTime Created { get; init; }

    // TODO: Make the "Premium" property call "ComputerPremium".
    public static decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var multiplier = Multiplier(coverType);

        var premiumPerDay = 1250 * multiplier;
        var insuranceDurationInDays = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        for (var day = 0; day < insuranceDurationInDays; day++)
        {
            if (day < 30)
            {
                totalPremium += premiumPerDay;
            }
            if (day < 180 && coverType == CoverType.Yacht)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            }
            else if (day < 180)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            }
            if (day < 365 && coverType != CoverType.Yacht)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            }
            else if (day < 365)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }
        }

        return totalPremium;
    }

    private static decimal Multiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}