namespace Claims.Infrastructure;

public class RandomIdGenerator : IIdGenerator
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}
