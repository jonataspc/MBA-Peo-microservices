using Peo.Core.Entities;

namespace Peo.Core.Interfaces.Services.Acls;

public interface IDetalhesUsuarioService
{
    Task<Usuario?> ObterUsuarioPorIdAsync(Guid userId);
}