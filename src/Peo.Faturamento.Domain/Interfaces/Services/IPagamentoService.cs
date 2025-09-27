using Peo.Core.Dtos;
using Peo.Faturamento.Domain.Entities;

namespace Peo.Faturamento.Domain.Interfaces.Services;

public interface IPagamentoService
{
    Task<Pagamento> EstornarPagamentoAsync(Guid pagamentoId, CancellationToken cancellationToken);

    Task<Pagamento> CancelarPagamentoAsync(Guid pagamentoId, CancellationToken cancellationToken);

    Task<Pagamento?> ObterPagamentoPorIdAsync(Guid pagamentoId, CancellationToken cancellationToken);

    Task<IEnumerable<Pagamento>> ObterPagamentosPorMatriculaIdAsync(Guid matriculaId, CancellationToken cancellationToken);

    Task<Pagamento> ProcessarPagamentoMatriculaAsync(Guid matriculaId, decimal valor, CartaoCredito cartaoCredito, CancellationToken cancellationToken);
}