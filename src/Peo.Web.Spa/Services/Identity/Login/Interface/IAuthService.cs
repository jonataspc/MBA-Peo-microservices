namespace Peo.Web.Spa.Services.Identity.Login.Interface
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string user, string pass);
        Task LogoutAsync();
    }
}
