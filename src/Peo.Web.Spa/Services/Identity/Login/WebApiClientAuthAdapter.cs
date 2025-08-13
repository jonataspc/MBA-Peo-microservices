using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Peo.Web.Spa.Services.Identity.Login.Interface;

namespace Peo.Web.Spa.Services.Identity.Login
{
    public sealed class WebApiClientAuthAdapter : IAuthService
    {
        private readonly WebApiClient _api;
        private readonly ITokenStore _tokens;
        private readonly AuthenticationStateProvider _auth;

        public WebApiClientAuthAdapter(WebApiClient api, ITokenStore tokens, AuthenticationStateProvider auth)
        {
            _api = api;
            _tokens = tokens;
            _auth = auth;
        }

        public async Task<bool> LoginAsync(string user, string pass)
        {
            try
            {
                var res = await _api.PostV1IdentityLoginAsync(new LoginRequest
                {
                    Email = user,
                    Password = pass
                });

                if (string.IsNullOrWhiteSpace(res?.Token))
                    return false;

                await _tokens.SetTokenAsync(res.Token);

                if (_auth is JwtAuthStateProvider jwt)
                    jwt.NotifyUserAuthentication(res.Token);

                return true;
            }
            catch (ApiException)
            {
                // opcional: logar o erro, mostrar snackbar, etc.
                await _tokens.ClearAsync();
                if (_auth is JwtAuthStateProvider jwt)
                    jwt.NotifyUserLogout();
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _tokens.ClearAsync();
            if (_auth is JwtAuthStateProvider jwt)
                jwt.NotifyUserLogout();
        }
    }
}
