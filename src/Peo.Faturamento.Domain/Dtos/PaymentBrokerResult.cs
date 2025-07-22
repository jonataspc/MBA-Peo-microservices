namespace Peo.Faturamento.Domain.Dtos
{
    public record PaymentBrokerResult(bool Success, string? Details, string? Hash);
}