using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Dtos;
using Peo.Core.Messages.IntegrationCommands;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.Faturamento.Application.Commands.PagamentoMatricula;
using Peo.Faturamento.Application.Dtos.Requests;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.ValueObjects;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class PagamentoMatriculaCommandHandlerTests
{
    private readonly Mock<IEstudanteRepository> _estudanteRepositoryMock;
    private readonly Mock<IRequestClient<ObterDetalhesCursoRequest>> _obterDetalhesCursoRequestMock;
    private readonly Mock<IRequestClient<ObterMatriculaRequest>> _obterMatriculaRequestMock;
    private readonly PagamentoMatriculaCommandHandler _handler;
    private readonly Mock<IMediator> _mediator;

    public PagamentoMatriculaCommandHandlerTests()
    {
        _estudanteRepositoryMock = new Mock<IEstudanteRepository>();
        _obterDetalhesCursoRequestMock = new Mock<IRequestClient<ObterDetalhesCursoRequest>>();
        _obterMatriculaRequestMock = new Mock<IRequestClient<ObterMatriculaRequest>>();
        _mediator = new Mock<IMediator>();

        _handler = new PagamentoMatriculaCommandHandler(
            _mediator.Object,
            _obterDetalhesCursoRequestMock.Object,
            _obterMatriculaRequestMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagamento_QuandoValido()
    {
        // Arrange
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, cursoId);
        var matriculaId = matricula.Id;
        var valor = 99.99m;
        var cartaoCredito = new CartaoCredito("1234567890123456", "12/25", "123", "Usuário Teste");
        var pagamento = new Pagamento(matriculaId, valor);
        pagamento.ProcessarPagamento(Guid.CreateVersion7().ToString());
        pagamento.ConfirmarPagamento(new CartaoCreditoData { Hash = "hash-123" });

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);

        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", valor));

        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);

        var matriculaMockResponse = new Mock<Response<ObterMatriculaResponse>>();
        matriculaMockResponse.Setup(x => x.Message).Returns(new ObterMatriculaResponse(matriculaId, cursoId, true));

        _obterMatriculaRequestMock.Setup(x => x.GetResponse<ObterMatriculaResponse>(
            It.IsAny<ObterMatriculaRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(matriculaMockResponse.Object);

        _mediator.Setup(x => x.Send(It.IsAny<ProcessarPagamentoMatriculaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new ProcessarPagamentoMatriculaResponse(true, StatusPagamento.Pago.ToString())));

        var requisicao = new PagamentoMatriculaRequest
        {
            MatriculaId = matriculaId,
            DadosCartao = cartaoCredito
        };
        var comando = new PagamentoMatriculaCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.MatriculaId.Should().Be(matriculaId);
        resultado.Value.StatusPagamento.Should().Be(StatusPagamento.Pago.ToString());
        resultado.Value.ValorPago.Should().Be(valor);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoOcorreErro()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, cursoId);
        var valor = 99.99m;

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);

        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", 100.00m));

        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);

        var matriculaMockResponse = new Mock<Response<ObterMatriculaResponse>>();
        matriculaMockResponse.Setup(x => x.Message).Returns(new ObterMatriculaResponse(matriculaId, cursoId, true));

        _obterMatriculaRequestMock.Setup(x => x.GetResponse<ObterMatriculaResponse>(
            It.IsAny<ObterMatriculaRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(matriculaMockResponse.Object);

        _mediator.Setup(x => x.Send(It.IsAny<ProcessarPagamentoMatriculaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProcessarPagamentoMatriculaResponse>(new Error()));

        var requisicao = new PagamentoMatriculaRequest
        {
            MatriculaId = matriculaId,
            DadosCartao = new("1234567890123456", "Usuário Teste", "12/25", "123")
        };
        var comando = new PagamentoMatriculaCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
    }
}