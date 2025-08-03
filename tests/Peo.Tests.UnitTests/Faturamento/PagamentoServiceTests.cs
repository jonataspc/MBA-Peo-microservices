using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Data;
using Peo.Core.Interfaces.Services;
using Peo.Faturamento.Application.Services;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Peo.Tests.UnitTests.Faturamento;

public class PagamentoServiceTests
{
    private readonly Mock<IRepository<Pagamento>> _pagamentoRepositoryMock;
    private readonly Mock<IBrokerPagamentoService> _paymentBrokerServiceMock;
    private readonly Mock<ILogger<PagamentoService>> _loggerMock;
    private readonly Mock<IMessageBus> _messageBus;
    private readonly PagamentoService _pagamentoService;

    public PagamentoServiceTests()
    {
        _pagamentoRepositoryMock = new Mock<IRepository<Pagamento>>();
        _paymentBrokerServiceMock = new Mock<IBrokerPagamentoService>();
        _loggerMock = new Mock<ILogger<PagamentoService>>();
        _messageBus = new Mock<IMessageBus>();

        _pagamentoService = new PagamentoService(
            _pagamentoRepositoryMock.Object,
            _paymentBrokerServiceMock.Object,
            _messageBus.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessarPagamentoMatriculaAsync_QuandoPagamentoSucesso_DeveConfirmarPagamento()
    {
        // Arrange
        var matriculaId = Guid.NewGuid();
        var valor = 100.00m;
        var cartaoCredito = new CartaoCredito("1234567890123456", "12/25", "123", "Test User");
        var pagamento = new Pagamento(matriculaId, valor);
        var paymentResult = new PaymentBrokerResult(true, "Success", "hash123");

        _pagamentoRepositoryMock.Setup(x => x.Insert(It.IsAny<Pagamento>()));
        _pagamentoRepositoryMock.Setup(x => x.Update(It.IsAny<Pagamento>()));
        _pagamentoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(pagamento);
        _paymentBrokerServiceMock.Setup(x => x.ProcessarPagamentoAsync(It.IsAny<CartaoCredito>()))
            .ReturnsAsync(paymentResult);
        _messageBus.Setup(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _pagamentoService.ProcessarPagamentoMatriculaAsync(matriculaId, valor, cartaoCredito);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPagamento.Pago);
        _pagamentoRepositoryMock.Verify(x => x.Update(It.Is<Pagamento>(p => p.Status == StatusPagamento.Pago)), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        _messageBus.Verify(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessarPagamentoMatriculaAsync_QuandoPagamentoFalha_DeveMarcarComoFalha()
    {
        // Arrange
        var matriculaId = Guid.NewGuid();
        var valor = 100.00m;
        var cartaoCredito = new CartaoCredito("1234567890123456", "12/25", "123", "Test User");
        var pagamento = new Pagamento(matriculaId, valor);
        var paymentResult = new PaymentBrokerResult(false, "Insufficient funds", null);

        _pagamentoRepositoryMock.Setup(x => x.Insert(It.IsAny<Pagamento>()));
        _pagamentoRepositoryMock.Setup(x => x.Update(It.IsAny<Pagamento>()));
        _pagamentoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(pagamento);
        _paymentBrokerServiceMock.Setup(x => x.ProcessarPagamentoAsync(It.IsAny<CartaoCredito>()))
            .ReturnsAsync(paymentResult);
        _messageBus.Setup(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _pagamentoService.ProcessarPagamentoMatriculaAsync(matriculaId, valor, cartaoCredito);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPagamento.Falha);
        _pagamentoRepositoryMock.Verify(x => x.Update(It.Is<Pagamento>(p => p.Status == StatusPagamento.Falha)), Times.Once);
        _pagamentoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        _messageBus.Verify(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact]
    public async Task ObterPagamentosPorMatriculaIdAsync_QuandoNaoExistemPagamentos_DeveRetornarListaVazia()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        _pagamentoRepositoryMock.Setup(x => x.WithTracking().GetAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
            .ReturnsAsync((IEnumerable<Pagamento>)null);

        // Act
        var resultado = await _pagamentoService.ObterPagamentosPorMatriculaIdAsync(matriculaId);

        // Assert
        resultado.Should().BeEmpty();
    }
}