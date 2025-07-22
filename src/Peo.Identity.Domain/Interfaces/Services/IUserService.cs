using Peo.Core.Entities;

namespace Peo.Identity.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task AddAsync(Usuario user);
    }
}