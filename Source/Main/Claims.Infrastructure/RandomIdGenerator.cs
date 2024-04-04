namespace Claims.Infrastructure;

public class RandomIdGenerator : IIdSource
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}
