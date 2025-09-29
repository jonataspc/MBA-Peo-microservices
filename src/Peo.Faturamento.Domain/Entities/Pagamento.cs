using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;
using Peo.Faturamento.Domain.ValueObjects;

namespace Peo.Faturamento.Domain.Entities;

public class Pagamento : EntityBase, IAggregateRoot
{
    public Guid MatriculaId { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public StatusPagamento Status { get; private set; }
    public string? IdTransacao { get; private set; }
    public string? Detalhes { get; private set; }
    public CartaoCreditoData? DadosCartao { get; private set; }

    protected Pagamento()
    { }

    public Pagamento(Guid matriculaId, decimal valor)
    {
        MatriculaId = matriculaId;
        Valor = valor;
        Status = StatusPagamento.Pendente;
        Validar();
    }

    public void ProcessarPagamento(string idTransacao)
    {
        if (Status != StatusPagamento.Pendente)
            throw new DomainException("Pagamento só pode ser processado quando está Pendente");

        IdTransacao = idTransacao;
        Status = StatusPagamento.Processando;
    }

    public void ConfirmarPagamento(CartaoCreditoData dadosCartao)
    {
        if (Status != StatusPagamento.Processando)
            throw new DomainException("Pagamento só pode ser confirmado quando está em Processamento");

        DadosCartao = dadosCartao;
        DataPagamento = DateTime.Now;
        Status = StatusPagamento.Pago;
    }

    public void MarcarComoFalha(string? detalhes)
    {
        if (Status != StatusPagamento.Processando)
            throw new DomainException("Pagamento só pode ser marcado como falha quando está em Processamento");

        Detalhes = detalhes;
        Status = StatusPagamento.Falha;
    }

    public void Estornar()
    {
        if (Status != StatusPagamento.Pago)
            throw new DomainException("Pagamento só pode ser estornado quando está Pago");

        Status = StatusPagamento.Estornado;
    }

    public void Cancelar()
    {
        if (Status != StatusPagamento.Pendente)
            throw new DomainException("Pagamento só pode ser cancelado quando está Pendente");

        Status = StatusPagamento.Cancelado;
    }

    private void Validar()
    {
        if (MatriculaId == Guid.Empty)
            throw new DomainException("O campo MatriculaId é obrigatório.");
        if (Valor <= 0)
            throw new DomainException("O campo Valor deve ser maior que zero.");
    }
}