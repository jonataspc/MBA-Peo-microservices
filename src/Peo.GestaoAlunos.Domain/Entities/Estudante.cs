using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;

namespace Peo.GestaoAlunos.Domain.Entities;

public class Estudante : EntityBase, IAggregateRoot
{
    public Guid UsuarioId { get; private set; }

    public bool EstaAtivo { get; private set; }

    public virtual ICollection<Matricula> Matriculas { get; private set; } = [];

    public Estudante()
    {
    }

    public Estudante(Guid usuarioId)
    {
        UsuarioId = usuarioId;
        EstaAtivo = true;
    }

    public void Deactivate()
    {
        EstaAtivo = false;
    }

    public void Activate()
    {
        EstaAtivo = true;
    }
}