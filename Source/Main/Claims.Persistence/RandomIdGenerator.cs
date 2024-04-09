namespace Claims.Persistence;

public class RandomIdGenerator : IIdSource
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}
