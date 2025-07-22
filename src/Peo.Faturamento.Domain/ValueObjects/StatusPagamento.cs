namespace Peo.Faturamento.Domain.ValueObjects;

public enum StatusPagamento
{
    Pendente,
    Processando,
    Pago,
    Falha,
    Estornado,
    Cancelado
}