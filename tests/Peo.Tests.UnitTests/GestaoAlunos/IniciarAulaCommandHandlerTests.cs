using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.GestaoAlunos.Application.Commands.Aula;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class IniciarAulaCommandHandlerTests
{
    private readonly Mock<IEstudanteService> _estudanteServiceMock;
    private readonly Mock<ILogger<IniciarAulaCommandHandler>> _loggerMock;
    private readonly IniciarAulaCommandHandler _handler;

    public IniciarAulaCommandHandlerTests()
    {
        _estudanteServiceMock = new Mock<IEstudanteService>();
        _loggerMock = new Mock<ILogger<IniciarAulaCommandHandler>>();
        _handler = new IniciarAulaCommandHandler(_estudanteServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarProgresso_QuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var progresso = new ProgressoMatricula(matriculaId, aulaId);

        _estudanteServiceMock.Setup(x => x.IniciarAulaAsync(matriculaId, aulaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progresso);
        _estudanteServiceMock.Setup(x => x.ObterProgressoGeralCursoAsync(matriculaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var requisicao = new IniciarAulaRequest
        {
            MatriculaId = matriculaId,
            AulaId = aulaId
        };
        var comando = new IniciarAulaCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.MatriculaId.Should().Be(matriculaId);
        resultado.Value.AulaId.Should().Be(aulaId);
        resultado.Value.EstaConcluida.Should().BeFalse();
        resultado.Value.DataInicio.Should().Be(progresso.DataInicio);
        resultado.Value.DataConclusao.Should().BeNull();
        resultado.Value.ProgressoGeralCurso.Should().Be(0);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoOcorreErro()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var mensagemErro = "Ocorreu um erro";

        _estudanteServiceMock.Setup(x => x.IniciarAulaAsync(matriculaId, aulaId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(mensagemErro));

        var requisicao = new IniciarAulaRequest
        {
            MatriculaId = matriculaId,
            AulaId = aulaId
        };
        var comando = new IniciarAulaCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be(mensagemErro);
    }
}