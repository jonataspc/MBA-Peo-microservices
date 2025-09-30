using Peo.Core.Dtos;
using Peo.Faturamento.Domain.Dtos;

namespace Peo.Faturamento.Domain.Services;

public interface IBrokerPagamentoService
{
    Task<PaymentBrokerResult> ProcessarPagamentoAsync(CartaoCredito cartaoCredito, CancellationToken cancellationToken);
}