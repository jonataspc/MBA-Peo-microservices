using Microsoft.JSInterop;
using Peo.Web.Spa.Services.Identity.Login.Interface;

namespace Peo.Web.Spa.Services.Identity.Login
{
    public sealed class TokenStoreLocalStorage : ITokenStore
    {
        private const string Key = "auth_token";
        private readonly IJSRuntime _js;
        public TokenStoreLocalStorage(IJSRuntime js) => _js = js;

        public Task<string?> GetTokenAsync()
            => _js.InvokeAsync<string?>("localStorage.getItem", Key).AsTask();

        public Task SetTokenAsync(string token)
            => _js.InvokeVoidAsync("localStorage.setItem", Key, token).AsTask();

        public Task ClearAsync()
            => _js.InvokeVoidAsync("localStorage.removeItem", Key).AsTask();
    }
}
