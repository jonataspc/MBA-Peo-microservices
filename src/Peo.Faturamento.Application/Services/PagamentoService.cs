using Microsoft.Extensions.Logging;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Data;
using Peo.Core.Interfaces.Services;
using Peo.Core.Messages.IntegrationEvents;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Domain.Interfaces.Services;
using Peo.Faturamento.Domain.ValueObjects;

namespace Peo.Faturamento.Application.Services;

public class PagamentoService(
    IRepository<Pagamento> pagamentoRepository,
    IBrokerPagamentoService paymentBrokerService,
    IMessageBus messageBus,
    ILogger<PagamentoService> logger) : IPagamentoService
{
    private async Task<Pagamento> CriarPagamentoAsync(Guid matriculaId, decimal valor)
    {
        var pagamento = new Pagamento(matriculaId, valor);
        pagamentoRepository.Insert(pagamento);
        await pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    private async Task<Pagamento> ProcessarPagamentoAsync(Guid pagamentoId, string idTransacao)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.ProcessarPagamento(idTransacao);
        pagamentoRepository.Update(pagamento);
        await pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    public async Task<Pagamento> EstornarPagamentoAsync(Guid pagamentoId)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.Estornar();
        pagamentoRepository.Update(pagamento);
        await pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    public async Task<Pagamento> CancelarPagamentoAsync(Guid pagamentoId)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.Cancelar();
        pagamentoRepository.Update(pagamento);
        await pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    public async Task<Pagamento?> ObterPagamentoPorIdAsync(Guid pagamentoId)
    {
        return await pagamentoRepository.WithTracking()
                                       .GetAsync(pagamentoId);
    }

    public async Task<IEnumerable<Pagamento>> ObterPagamentosPorMatriculaIdAsync(Guid matriculaId)
    {
        return await pagamentoRepository.WithTracking().GetAsync(p => p.MatriculaId == matriculaId)
            ?? [];
    }

    public async Task<Pagamento> ProcessarPagamentoMatriculaAsync(Guid matriculaId, decimal valor, CartaoCredito cartaoCredito)
    {
        var pagamento = await CriarPagamentoAsync(matriculaId, valor);
        var idTransacao = Guid.CreateVersion7().ToString();
        pagamento = await ProcessarPagamentoAsync(pagamento.Id, idTransacao);

        PaymentBrokerResult result;
        try
        {
            result = await paymentBrokerService.ProcessarPagamentoAsync(cartaoCredito);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            pagamento.MarcarComoFalha(e.Message);
            result = new PaymentBrokerResult(false, e.Message, null);
        }

        if (result.Success)
        {
            pagamento.ConfirmarPagamento(new CartaoCreditoData() { Hash = result.Hash });

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