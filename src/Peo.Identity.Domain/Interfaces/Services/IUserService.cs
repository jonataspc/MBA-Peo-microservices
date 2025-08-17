using Peo.Core.Entities;

namespace Peo.Identity.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task AddAsync(Usuario user);

        Task<Usuario?> ObterUsuarioPorIdAsync(Guid userId);
    }
}