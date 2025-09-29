using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.GestaoAlunos.Application.Commands.Matricula;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Services;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class ConcluirMatriculaCommandHandlerTests
{
    private readonly Mock<IAlunoService> _alunoService;
    private readonly Mock<ILogger<ConcluirMatriculaCommandHandler>> _loggerMock;
    private readonly ConcluirMatriculaCommandHandler _handler;

    public ConcluirMatriculaCommandHandlerTests()
    {
        _alunoService = new Mock<IAlunoService>();
        _loggerMock = new Mock<ILogger<ConcluirMatriculaCommandHandler>>();
        _handler = new ConcluirMatriculaCommandHandler(
            _alunoService.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarMatricula_QuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoId, cursoId);
        matricula.ConfirmarPagamento();
        matricula.Concluir();

        _alunoService.Setup(x => x.ConcluirMatriculaAsync(matriculaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matricula);

        var request = new ConcluirMatriculaRequest
        {
            MatriculaId = matriculaId
        };
        var command = new ConcluirMatriculaCommand(request);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Status.Should().Be(StatusMatricula.Concluido.ToString());
        resultado.Value.DataConclusao.Should().Be(matricula.DataConclusao);
        resultado.Value.ProgressoGeral.Should().Be(100);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoErroOcorre()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var mensagemErro = "Ocorreu um erro";

        _alunoService.Setup(x => x.ConcluirMatriculaAsync(matriculaId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(mensagemErro));

        var request = new ConcluirMatriculaRequest
        {
            MatriculaId = matriculaId
        };
        var command = new ConcluirMatriculaCommand(request);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be(mensagemErro);
    }
}