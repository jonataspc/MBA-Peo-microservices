using Peo.Web.Spa.Services.Identity.Login.Interface;
using System.Net.Http.Headers;

namespace Peo.Web.Spa.Services.Identity.Login
{
    public sealed class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ITokenStore _tokens;
        public AuthHeaderHandler(ITokenStore tokens) => _tokens = tokens;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokens.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
