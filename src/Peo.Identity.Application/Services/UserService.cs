using Peo.Core.Entities;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Services
{
    public class UserService(IUserRepository repository) : IUserService, IDetalhesUsuarioService
    {
        public async Task AddAsync(Usuario user)
        {
            repository.Insert(user);
            await repository.UnitOfWork.CommitAsync(CancellationToken.None);
        }

        public async Task<Usuario?> ObterUsuarioPorIdAsync(Guid userId)
        {
            return await repository.GetByIdAsync(userId);
        }
    }
}