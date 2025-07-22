using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Commands.MatriculaCurso;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class MatriculaCursoCommandHandlerTests
{
    private readonly Mock<IEstudanteService> _estudanteServiceMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly Mock<ILogger<MatriculaCursoCommandHandler>> _loggerMock;
    private readonly MatriculaCursoCommandHandler _handler;

    public MatriculaCursoCommandHandlerTests()
    {
        _estudanteServiceMock = new Mock<IEstudanteService>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _loggerMock = new Mock<ILogger<MatriculaCursoCommandHandler>>();
        _handler = new MatriculaCursoCommandHandler(
            _estudanteServiceMock.Object,
            _appIdentityUserMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarMatriculaId_QuandoValido()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matriculaId = Guid.CreateVersion7();
        var matricula = new Matricula(Guid.CreateVersion7(), cursoId) { Id = matriculaId };

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);

        _appIdentityUserMock.Setup(x => x.GetUsername())
            .Returns("João da Silva");

        _appIdentityUserMock.Setup(x => x.IsAuthenticated())
            .Returns(true);

        _appIdentityUserMock.Setup(x => x.IsInRole(It.IsAny<string>()))
            .Returns(true);

        _appIdentityUserMock.Setup(x => x.IsAdmin())
            .Returns(true);

        _appIdentityUserMock.Setup(x => x.GetLocalIpAddress())
            .Returns("127.0.0.1");

        _appIdentityUserMock.Setup(x => x.GetRemoteIpAddress())
            .Returns("127.0.0.1");

        _estudanteServiceMock.Setup(x => x.MatricularEstudanteComUserIdAsync(usuarioId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matricula);

        var requisicao = new MatriculaCursoRequest
        {
            CursoId = cursoId
        };
        var comando = new MatriculaCursoCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.MatriculaId.Should().Be(matriculaId);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoOcorreErro()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var mensagemErro = "Ocorreu um erro";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _appIdentityUserMock.Setup(x => x.IsAuthenticated())
            .Returns(true);
        _estudanteServiceMock.Setup(x => x.MatricularEstudanteComUserIdAsync(usuarioId, cursoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(mensagemErro));

        var requisicao = new MatriculaCursoRequest
        {
            CursoId = cursoId
        };
        var comando = new MatriculaCursoCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be(mensagemErro);
    }
}