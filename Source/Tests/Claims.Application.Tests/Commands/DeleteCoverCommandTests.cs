﻿using Claims.Application.Commands;
using Claims.Domain;
using Moq;
using Xunit;

namespace Claims.Application.Tests.Commands;

public class DeleteCoverCommandTests : CoversCommandTests
{
    private readonly DeleteCoverCommand _command;

    public DeleteCoverCommandTests()
    {
        _command = new DeleteCoverCommand(_coversRepositoryMock.Object);
    }

    [Fact]
    public async Task DeletesCoverByIdInRepository()
    {
        var id = Guid.NewGuid();

        await _command.ExecuteAsync(new DeleteCoverRequest(id));

        _coversRepositoryMock.Verify(x => x.DeleteByIdAsync(id));
    }

    [Fact]
    public async Task ReturnsCoverReturnedByRepositoryDelete()
    {
        var id = Guid.NewGuid();
        var cover = TestDomainData.RandomCover();
        StubDeleteCover(id, cover);

        var returnedCover = await _command.ExecuteAsync(new DeleteCoverRequest(id));

        Assert.Equal(cover, returnedCover);
    }

    private void StubDeleteCover(Guid id, Cover? deletedCoverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.DeleteByIdAsync(id))
            .ReturnsAsync(deletedCoverToReturn);
    }
}