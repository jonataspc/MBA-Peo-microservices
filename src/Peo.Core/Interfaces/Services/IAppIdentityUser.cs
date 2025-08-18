using Microsoft.AspNetCore.Http;

namespace Peo.Core.Interfaces.Services
{
    public interface IAppIdentityUser
    {
        string GetUsername();

        Guid GetUserId();

        bool IsAuthenticated();

        bool IsInRole(string role);

        string? GetRemoteIpAddress();

        string? GetLocalIpAddress();

        bool IsAdmin();

        HttpContext GetHttpContext();

        string? GetUserToken();

        string GetUserRefreshToken();
    }
}