using Peo.Core.Entities;
using Peo.Core.Interfaces.Data;

namespace Peo.Identity.Domain.Interfaces.Data
{
    public interface IUserRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task<Usuario?> GetByIdAsync(Guid userId);

        void Insert(Usuario user);
    }
}