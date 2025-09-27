using Peo.Core.Dtos;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Interfaces.Brokers;

namespace Peo.Faturamento.Integrations.Paypal.Services
{
    public class PaypalBrokerService : IBrokerPagamentoService
    {
        public async Task<PaymentBrokerResult> ProcessarPagamentoAsync(CartaoCredito cartaoCredito, CancellationToken cancellationToken)
        {
            if (cartaoCredito?.NumeroCartao is null)
            {
                return new PaymentBrokerResult(false, "Credit card is null", Guid.CreateVersion7().ToString());
            }

            if (cartaoCredito.NumeroCartao.Length != 16 && cartaoCredito.NumeroCartao.Length != 15)
            {
                return new PaymentBrokerResult(false, "Credit card is invalid", Guid.CreateVersion7().ToString());
            }

            // Simula chamada à API do Paypal
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(0, 2)));

            var success = Random.Shared.Next(0, 2) == 1 ||
                            cartaoCredito.NumeroCartao.StartsWith("1234");

            if (success)
            {
                return new PaymentBrokerResult(true, default, Guid.CreateVersion7().ToString());
            }

            // Simula falha no pagamento
            var reasons = new[]
                {
                    "Insufficient funds",
                    "Card expired",
                    "Card reported lost or stolen",
                    "Payment gateway error"
                };

            var reason = reasons[Random.Shared.Next(reasons.Length)];

            return new PaymentBrokerResult(false, reason, Guid.CreateVersion7().ToString());
        }
    }
}