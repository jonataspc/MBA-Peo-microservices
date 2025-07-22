using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.Communication.Mediator;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Data;
using Peo.Faturamento.Application.Services;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Domain.ValueObjects;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using System.Linq.Expressions;

namespace Peo.Tests.UnitTests.Faturamento;

public class PagamentoServiceTests
{
    private readonly Mock<IRepository<Pagamento>> _pagamentoRepositoryMock;
    private readonly Mock<IEstudanteRepository> _estudanteRepositoryMock;
    private readonly Mock<IBrokerPagamentoService> _paymentBrokerServiceMock;
    private readonly PagamentoService _pagamentoService;
    private readonly Mock<ILogger<PagamentoService>> _loggerMock;
    private readonly Mock<IMediatorHandler> _mediatorHandler;

    public PagamentoServiceTests()
    {
        _pagamentoRepositoryMock = new Mock<IRepository<Pagamento>>();
        _estudanteRepositoryMock = new Mock<IEstudanteRepository>();
        _paymentBrokerServiceMock = new Mock<IBrokerPagamentoService>();
        _loggerMock = new Mock<ILogger<PagamentoService>>();
        _mediatorHandler = new Mock<IMediatorHandler>();

        _pagamentoService = new PagamentoService(
            _pagamentoRepositoryMock.Object,
            _paymentBrokerServiceMock.Object,
            _mediatorHandler.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessarPagamentoMatriculaAsync_DeveProcessarPagamentoComSucesso()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var valor = 99.99m;
        var cartaoCredito = new CartaoCredito("1234567890123456", "12/25", "123", "Usuário Teste");
        var matricula = new Matricula(Guid.CreateVersion7(), Guid.CreateVersion7());
        var pagamento = new Pagamento(matriculaId, valor);
        var resultadoBroker = new PaymentBrokerResult(true, null, "hash-123");
        var unitOfWorkEstudanteMock = new Mock<IUnitOfWork>();

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork)
            .Returns(unitOfWorkEstudanteMock.Object);
        unitOfWorkEstudanteMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentBrokerServiceMock.Setup(x => x.ProcessarPagamentoAsync(cartaoCredito))
            .ReturnsAsync(resultadoBroker);
        _pagamentoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _pagamentoRepositoryMock.Setup(x => x.Update(It.IsAny<Pagamento>()))
            .Callback<Pagamento>(p => { });
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(pagamento);

        // Act
        var resultado = await _pagamentoService.ProcessarPagamentoMatriculaAsync(matriculaId, valor, cartaoCredito);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPagamento.Pago);
        resultado.IdTransacao.Should().NotBeNull();
        resultado.DataPagamento.Should().NotBeNull();
        resultado.DadosCartao.Should().NotBeNull();
        resultado!.DadosCartao!.Hash.Should().Be("hash-123");
        _pagamentoRepositoryMock.Verify(x => x.Insert(It.Is<Pagamento>(p => p.MatriculaId == matriculaId && p.Valor == valor)), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.Update(It.Is<Pagamento>(p => p.Status == StatusPagamento.Pago)), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ProcessarPagamentoMatriculaAsync_DeveLidarComFalhaPagamento()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var valor = 99.99m;
        var cartaoCredito = new CartaoCredito("1234567890123456", "12/25", "123", "Usuário Teste");
        var matricula = new Matricula(Guid.CreateVersion7(), Guid.CreateVersion7());
        var resultadoBroker = new PaymentBrokerResult(false, "Fundos insuficientes", null);
        var pagamento = new Pagamento(matriculaId, valor);
        var unitOfWorkEstudanteMock = new Mock<IUnitOfWork>();

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork)
            .Returns(unitOfWorkEstudanteMock.Object);
        unitOfWorkEstudanteMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentBrokerServiceMock.Setup(x => x.ProcessarPagamentoAsync(cartaoCredito))
            .ReturnsAsync(resultadoBroker);
        _pagamentoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _pagamentoRepositoryMock.Setup(x => x.Update(It.IsAny<Pagamento>()))
            .Callback<Pagamento>(p => { });
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(pagamento);

        // Act
        var resultado = await _pagamentoService.ProcessarPagamentoMatriculaAsync(matriculaId, valor, cartaoCredito);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPagamento.Falha);
        resultado.Detalhes.Should().Be("Fundos insuficientes");
        _pagamentoRepositoryMock.Verify(x => x.Insert(It.Is<Pagamento>(p => p.MatriculaId == matriculaId && p.Valor == valor)), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.Update(It.Is<Pagamento>(p => p.Status == StatusPagamento.Falha && p.Detalhes == "Fundos insuficientes")), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        _estudanteRepositoryMock.Verify(x => x.AtualizarMatricula(matricula), Times.Never);
    }

    [Fact]
    public async Task EstornarPagamentoAsync_DeveEstornarPagamentoComSucesso()
    {
        // Arrange
        var pagamentoId = Guid.CreateVersion7();
        var pagamento = new Pagamento(Guid.CreateVersion7(), 99.99m);
        pagamento.ProcessarPagamento("transaction-123");
        pagamento.ConfirmarPagamento(new CartaoCreditoData { Hash = "hash-123" });

        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(pagamentoId))
            .ReturnsAsync(pagamento);
        _pagamentoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _pagamentoService.EstornarPagamentoAsync(pagamentoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPagamento.Estornado);
        _pagamentoRepositoryMock.Verify(x => x.Update(pagamento), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EstornarPagamentoAsync_DeveLancarQuandoPagamentoNaoEncontrado()
    {
        // Arrange
        var pagamentoId = Guid.CreateVersion7();
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(pagamentoId))
            .ReturnsAsync((Pagamento?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _pagamentoService.EstornarPagamentoAsync(pagamentoId));

        _pagamentoRepositoryMock.Verify(x => x.Update(It.IsAny<Pagamento>()), Times.Never);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelarPagamentoAsync_DeveCancelarPagamentoComSucesso()
    {
        // Arrange
        var pagamentoId = Guid.CreateVersion7();
        var matriculaId = Guid.CreateVersion7();
        var pagamento = new Pagamento(matriculaId, 99.99m);
        // O pagamento começa com status Pendente, que é o estado correto para cancelamento

        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(pagamentoId))
            .ReturnsAsync(pagamento);
        _pagamentoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _pagamentoService.CancelarPagamentoAsync(pagamentoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPagamento.Cancelado);
        _pagamentoRepositoryMock.Verify(x => x.Update(pagamento), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelarPagamentoAsync_DeveLancarQuandoPagamentoNaoEncontrado()
    {
        // Arrange
        var pagamentoId = Guid.CreateVersion7();
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(pagamentoId))
            .ReturnsAsync((Pagamento?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _pagamentoService.CancelarPagamentoAsync(pagamentoId));

        _pagamentoRepositoryMock.Verify(x => x.Update(It.IsAny<Pagamento>()), Times.Never);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObterPagamentoPorIdAsync_DeveRetornarPagamentoQuandoEncontrado()
    {
        // Arrange
        var pagamentoId = Guid.CreateVersion7();
        var pagamentoEsperado = new Pagamento(Guid.CreateVersion7(), 99.99m);
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(pagamentoId))
            .ReturnsAsync(pagamentoEsperado);

        // Act
        var resultado = await _pagamentoService.ObterPagamentoPorIdAsync(pagamentoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(pagamentoEsperado);
    }

    [Fact]
    public async Task ObterPagamentoPorIdAsync_DeveRetornarNullQuandoPagamentoNaoEncontrado()
    {
        // Arrange
        var pagamentoId = Guid.CreateVersion7();
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(pagamentoId))
            .ReturnsAsync((Pagamento?)null);

        // Act
        var resultado = await _pagamentoService.ObterPagamentoPorIdAsync(pagamentoId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPagamentosPorMatriculaIdAsync_DeveRetornarPagamentosQuandoEncontrado()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var pagamentosEsperados = new List<Pagamento>
        {
            new Pagamento(matriculaId, 99.99m),
            new Pagamento(matriculaId, 199.99m)
        };
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
            .ReturnsAsync(pagamentosEsperados);

        // Act
        var resultado = await _pagamentoService.ObterPagamentosPorMatriculaIdAsync(matriculaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(pagamentosEsperados);
    }

    [Fact]
    public async Task ObterPagamentosPorMatriculaIdAsync_DeveRetornarListaVaziaQuandoNenhumPagamentoEncontrado()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Pagamento>());

        // Act
        var resultado = await _pagamentoService.ObterPagamentosPorMatriculaIdAsync(matriculaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}