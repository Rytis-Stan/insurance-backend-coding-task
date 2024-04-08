using static Claims.Testing.TestValueBuilder;

namespace Claims.Testing;

public class TestData
{
    private static readonly Random Random = new();

    public static int RandomInt(int inclusiveMinValue, int inclusiveMaxValue)
    {
        return Random.Next(inclusiveMinValue, inclusiveMaxValue + 1);
    }

    public static int RandomInt(int inclusiveMaxValue)
    {
        return Random.Next(inclusiveMaxValue + 1);
    }

    public static string RandomString(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid()}";
    }

    public static TEnum RandomEnum<TEnum>()
        where TEnum : struct, Enum
    {
        return RandomChoice(Enum.GetValues<TEnum>());
    }

    private static T RandomChoice<T>(T[] values)
    {
        var index = Random.Next(values.Length);
        return values[index];
    }

    public static DateTime RandomUtcDateTime()
    {
        return UtcDateTime(2000, 01, 01).AddDays(Random.Next(10_000));
    }

    public static DateOnly RandomDate()
    {
        return new DateOnly(2000, 01, 01).AddDays(Random.Next(10_000));
    }

    public static (DateOnly Start, DateOnly End) RandomFixedLengthPeriod(int durationInDays)
    {
        var startDate = RandomDate();
        var endDate = startDate.AddDays(durationInDays - 1);
        return (startDate, endDate);
    }
}
