using FluentAssertions;
using Moq;
using Peo.Core.Entities;
using Peo.Identity.Application.Services;
using Peo.Identity.Domain.Interfaces.Data;

namespace Peo.Tests.UnitTests.Identity;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task AddAsync_DeveAdicionarUsuarioEConfirmarAlteracoes()
    {
        // Arrange
        var usuario = new Usuario(Guid.CreateVersion7(), "Usuário Teste", "teste@exemplo.com");
        _userRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _userService.AddAsync(usuario);

        // Assert
        _userRepositoryMock.Verify(x => x.Insert(usuario), Times.Once);
        _userRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_DeveRetornarUsuarioQuandoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var usuarioEsperado = new Usuario(usuarioId, "Usuário Teste", "teste@exemplo.com");
        _userRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId))
            .ReturnsAsync(usuarioEsperado);

        // Act
        var resultado = await _userService.ObterUsuarioPorIdAsync(usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(usuarioEsperado);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_DeveRetornarNullQuandoUsuarioNaoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId))
            .ReturnsAsync((Usuario?)null);

        // Act
        var resultado = await _userService.ObterUsuarioPorIdAsync(usuarioId);

        // Assert
        resultado.Should().BeNull();
    }
}