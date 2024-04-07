using Claims.Application.Repositories;
using Moq;

namespace Claims.Application.Tests.Commands;

public abstract class CoversCommandTests
{
    protected readonly Mock<ICoversRepository> CoversRepositoryMock;

    protected CoversCommandTests()
    {
        CoversRepositoryMock = new Mock<ICoversRepository>();
    }
}
