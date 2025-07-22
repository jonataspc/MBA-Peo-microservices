using Peo.Core.Dtos;
using Peo.Faturamento.Domain.Entities;

namespace Peo.Faturamento.Domain.Interfaces.Services;

public interface IPagamentoService
{
    Task<Pagamento> EstornarPagamentoAsync(Guid pagamentoId);

    Task<Pagamento> CancelarPagamentoAsync(Guid pagamentoId);

    Task<Pagamento?> ObterPagamentoPorIdAsync(Guid pagamentoId);

    Task<IEnumerable<Pagamento>> ObterPagamentosPorMatriculaIdAsync(Guid matriculaId);

    Task<Pagamento> ProcessarPagamentoMatriculaAsync(Guid matriculaId, decimal valor, CartaoCredito cartaoCredito);
}