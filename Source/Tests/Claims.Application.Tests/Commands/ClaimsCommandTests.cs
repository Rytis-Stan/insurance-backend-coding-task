using Claims.Application.Repositories;
using Moq;

namespace Claims.Application.Tests.Commands;

public abstract class ClaimsCommandTests
{
    protected readonly Mock<IClaimsRepository> ClaimsRepositoryMock;

    protected ClaimsCommandTests()
    {
        ClaimsRepositoryMock = new Mock<IClaimsRepository>();
    }
}
