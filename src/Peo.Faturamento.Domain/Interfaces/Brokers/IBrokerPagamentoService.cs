using Peo.Core.Dtos;
using Peo.Faturamento.Domain.Dtos;

namespace Peo.Faturamento.Domain.Interfaces.Brokers
{
    public interface IBrokerPagamentoService
    {
        Task<PaymentBrokerResult> ProcessarPagamentoAsync(CartaoCredito cartaoCredito);
    }
}