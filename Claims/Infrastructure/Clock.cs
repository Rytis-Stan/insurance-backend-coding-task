namespace Claims.Infrastructure;

public class Clock : IClock
{
    public DateTime Now()
    {
        return DateTime.Now;
    }
}
