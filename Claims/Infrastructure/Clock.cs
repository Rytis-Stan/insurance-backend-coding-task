namespace Claims.Infrastructure;

public class Clock : IClock
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}
