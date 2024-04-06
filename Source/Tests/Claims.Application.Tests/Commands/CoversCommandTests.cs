using Claims.Application.Repositories;
using Moq;

namespace Claims.Application.Tests.Commands;

// TODO: Decide where to put the "RandomCover()" method.
public abstract class CoversCommandTests
{
    protected readonly Mock<ICoversRepository> _coversRepositoryMock;

    protected CoversCommandTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
    }
}
