namespace Claims.Infrastructure;

public class IdGenerator : IIdGenerator
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}
