using Claims.Application.Repositories;
using Moq;

namespace Claims.Application.Tests.Commands;

// TODO: Decide what to do about this base tests class and the "RandomClaim()" method.
public abstract class ClaimsCommandTests
{
    protected readonly Mock<IClaimsRepository> _claimsRepositoryMock;

    protected ClaimsCommandTests()
    {
        _claimsRepositoryMock = new Mock<IClaimsRepository>();
    }
}
