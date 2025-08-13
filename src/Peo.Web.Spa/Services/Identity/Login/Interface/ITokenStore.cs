namespace Peo.Web.Spa.Services.Identity.Login.Interface
{
    public interface ITokenStore
    {
        Task SetTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task ClearAsync();
    }
}
