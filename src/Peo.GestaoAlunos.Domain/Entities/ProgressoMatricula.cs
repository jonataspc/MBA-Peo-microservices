using Peo.Core.Entities.Base;

namespace Peo.GestaoAlunos.Domain.Entities;

public class ProgressoMatricula : EntityBase
{
    public Guid MatriculaId { get; private set; }
    public Guid AulaId { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime? DataConclusao { get; private set; }

    public bool EstaConcluido => DataConclusao.HasValue;

    protected ProgressoMatricula()
    { }

    public ProgressoMatricula(Guid matriculaId, Guid aulaId)
    {
        MatriculaId = matriculaId;
        AulaId = aulaId;
        DataInicio = DateTime.Now;
    }

    public void MarcarComoConcluido()
    {
        DataConclusao = DateTime.Now;
    }
}