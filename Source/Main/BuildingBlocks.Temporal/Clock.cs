namespace BuildingBlocks.Temporal;

public class Clock : IClock
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}
