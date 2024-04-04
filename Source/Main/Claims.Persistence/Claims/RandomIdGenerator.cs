namespace Claims.Persistence.Claims;

public class RandomIdGenerator : IIdSource
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}
