using Microsoft.Extensions.Logging;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Data;
using Peo.Core.Interfaces.Services;
using Peo.Core.Messages.IntegrationEvents;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.Services;
using Peo.Faturamento.Domain.ValueObjects;

namespace Peo.Faturamento.Application.Services;

public class PagamentoService(
    IRepository<Pagamento> pagamentoRepository,
    IBrokerPagamentoService paymentBrokerService,
    IMessageBus messageBus,
    ILogger<PagamentoService> logger) : IPagamentoService
{
    private async Task<Pagamento> CriarPagamentoAsync(Guid matriculaId, decimal valor, CancellationToken cancellationToken)
    {
        var pagamento = new Pagamento(matriculaId, valor);
        pagamentoRepository.Insert(pagamento, cancellationToken);
        await pagamentoRepository.UnitOfWork.CommitAsync(cancellationToken);
        return pagamento;
    }

    private async Task<Pagamento> ProcessarPagamentoAsync(Guid pagamentoId, string idTransacao, CancellationToken cancellationToken)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId, cancellationToken)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.ProcessarPagamento(idTransacao);
        pagamentoRepository.Update(pagamento, cancellationToken);
        await pagamentoRepository.UnitOfWork.CommitAsync(cancellationToken);
        return pagamento;
    }

    public async Task<Pagamento> EstornarPagamentoAsync(Guid pagamentoId, CancellationToken cancellationToken)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId, cancellationToken)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.Estornar();
        pagamentoRepository.Update(pagamento, cancellationToken);
        await pagamentoRepository.UnitOfWork.CommitAsync(cancellationToken);
        return pagamento;
    }

    public async Task<Pagamento> CancelarPagamentoAsync(Guid pagamentoId, CancellationToken cancellationToken)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId, cancellationToken)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.Cancelar();
        pagamentoRepository.Update(pagamento, cancellationToken);
        await pagamentoRepository.UnitOfWork.CommitAsync(cancellationToken);
        return pagamento;
    }

    public async Task<Pagamento?> ObterPagamentoPorIdAsync(Guid pagamentoId, CancellationToken cancellationToken)
    {
        return await pagamentoRepository.WithTracking()
                                       .GetAsync(pagamentoId, cancellationToken);
    }

    public async Task<IEnumerable<Pagamento>> ObterPagamentosPorMatriculaIdAsync(Guid matriculaId, CancellationToken cancellationToken)
    {
        return await pagamentoRepository.WithTracking().GetAsync(p => p.MatriculaId == matriculaId, cancellationToken)
            ?? [];
    }

    public async Task<Pagamento> ProcessarPagamentoMatriculaAsync(Guid matriculaId, decimal valor, CartaoCredito cartaoCredito, CancellationToken cancellationToken)
    {
        var pagamento = await CriarPagamentoAsync(matriculaId, valor, cancellationToken);
        var idTransacao = Guid.CreateVersion7().ToString();
        pagamento = await ProcessarPagamentoAsync(pagamento.Id, idTransacao, cancellationToken);

        PaymentBrokerResult result;
        try
        {
            result = await paymentBrokerService.ProcessarPagamentoAsync(cartaoCredito, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            pagamento.MarcarComoFalha(e.Message);
            result = new PaymentBrokerResult(false, e.Message, null);
        }

        if (result.Success)
        {
            pagamento.ConfirmarPagamento(new DadosDoCartaoCredito() { Hash = result.Hash });

            await messageBus.PublishAsync(new PagamentoMatriculaConfirmadoEvent(
               pagamento.MatriculaId,
               pagamento.Valor,
               pagamento.DataPagamento!));
        }
        else
        {
            pagamento.MarcarComoFalha(result.Details);

            await messageBus.PublishAsync(new PagamentoComFalhaEvent(
               pagamento.MatriculaId,
               result.Details));
        }

        await pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);

        return pagamento;
    }
}