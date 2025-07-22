using Microsoft.AspNetCore.Identity;

namespace Peo.Identity.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, IEnumerable<string> roles);
    }
}