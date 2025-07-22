using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;

namespace Peo.Core.Entities
{
    public class Usuario : EntityBase, IAggregateRoot
    {
        public string NomeCompleto { get; private set; }
        public string Email { get; private set; }

        public Usuario(Guid id, string nomeCompleto, string email)
        {
            NomeCompleto = nomeCompleto ?? throw new ArgumentNullException(nameof(nomeCompleto));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Id = id;
        }
    }
}