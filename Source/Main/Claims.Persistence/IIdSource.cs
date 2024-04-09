namespace Claims.Persistence;

public interface IIdSource
{
    Guid NewId();
}
