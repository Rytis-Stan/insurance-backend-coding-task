namespace Claims.Infrastructure;

public interface IClock
{
    DateTime UtcNow();
}
