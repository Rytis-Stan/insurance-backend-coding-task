namespace BuildingBlocks.Temporal;

public interface IClock
{
    DateTime UtcNow();
}
