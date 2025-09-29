using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Domain.Entities;

public class Matricula : EntityBase
{
    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public DateTime? DataConclusao { get; private set; }
    public StatusMatricula Status { get; private set; }
    public int PercentualProgresso { get; private set; }
    public virtual Aluno? Aluno { get; set; }

    protected Matricula()
    { }

    public Matricula(Guid alunoId, Guid cursoId)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        DataMatricula = DateTime.Now;
        Status = StatusMatricula.PendentePagamento;
        PercentualProgresso = 0;
        Validar();
    }

    public void AtualizarProgresso(int percentual)
    {
        if (percentual < 0 || percentual > 100)
            throw new DomainException("O percentual de progresso deve estar entre 0 e 100");

        PercentualProgresso = percentual;
    }

    public void Concluir()
    {
        DataConclusao = DateTime.Now;
        Status = StatusMatricula.Concluido;
        PercentualProgresso = 100;
    }

    public void ConfirmarPagamento()
    {
        Status = StatusMatricula.Ativo;
    }

    public void Cancelar()
    {
        Status = StatusMatricula.Cancelado;
    }

    private void Validar()
    {
        if (AlunoId == Guid.Empty)
            throw new DomainException("O campo AlunoId é obrigatório.");
        if (CursoId == Guid.Empty)
            throw new DomainException("O campo CursoId é obrigatório.");
    }
}