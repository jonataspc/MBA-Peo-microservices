using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.GestaoAlunos.Application.Commands.Aula;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class ConcluirAulaCommandHandlerTests
{
    private readonly Mock<IAlunoService> _alunoServiceMock;
    private readonly Mock<ILogger<ConcluirAulaCommandHandler>> _loggerMock;
    private readonly ConcluirAulaCommandHandler _handler;

    public ConcluirAulaCommandHandlerTests()
    {
        _alunoServiceMock = new Mock<IAlunoService>();
        _loggerMock = new Mock<ILogger<ConcluirAulaCommandHandler>>();
        _handler = new ConcluirAulaCommandHandler(_alunoServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handler_DeveRetornarProgresso_QuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var dataInicio = DateTime.Now.AddHours(-1);
        var dataConclusao = DateTime.Now;
        var progresso = new ProgressoMatricula(matriculaId, aulaId);
        progresso.MarcarComoConcluido();

        _alunoServiceMock.Setup(x => x.ConcluirAulaAsync(matriculaId, aulaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progresso);
        _alunoServiceMock.Setup(x => x.ObterProgressoGeralCursoAsync(matriculaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);

        var request = new ConcluirAulaRequest
        {
            MatriculaId = matriculaId,
            AulaId = aulaId
        };
        var command = new ConcluirAulaCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.MatriculaId.Should().Be(matriculaId);
        result.Value.AulaId.Should().Be(aulaId);
        result.Value.EstaConcluida.Should().BeTrue();
        result.Value.DataInicio.Should().Be(progresso.DataInicio);
        result.Value.DataConclusao.Should().Be(progresso.DataConclusao);
        result.Value.ProgressoGeralCurso.Should().Be(50);
    }

    [Fact]
    public async Task Handler_DeveRetornarFalha_QuandoErroOcorre()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var errorMessage = "An error occurred";

        _alunoServiceMock.Setup(x => x.ConcluirAulaAsync(matriculaId, aulaId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        var request = new ConcluirAulaRequest
        {
            MatriculaId = matriculaId,
            AulaId = aulaId
        };
        var command = new ConcluirAulaCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(errorMessage);
    }
}