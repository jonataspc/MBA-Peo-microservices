using Microsoft.AspNetCore.Components.Authorization;
using Peo.Web.Spa.Services.Identity.Login.Interface;
using System.Security.Claims;
using System.Text.Json;

namespace Peo.Web.Spa.Services.Identity.Login
{
    public sealed class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStore _tokens;
        private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

        public JwtAuthStateProvider(ITokenStore tokens) => _tokens = tokens;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokens.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token) || IsExpired(token))
                return new AuthenticationState(Anonymous);

            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaims(token), "jwt"));
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(ParseClaims(token), "jwt"));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
        }

        private static bool IsExpired(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2) return true;

                var payload = Convert.FromBase64String(Pad(parts[1].Replace('-', '+').Replace('_', '/')));
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("exp", out var exp))
                {
                    var seconds = exp.GetInt64();
                    return DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow;
                }
            }
            catch { /* se der erro, trate como expirado */ }
            return true;
        }

        private static IEnumerable<Claim> ParseClaims(string token)
        {
            var claims = new List<Claim>();
            string[] userRolesValue = null!;

            try
            {
                var parts = token.Split('.');
                var payload = Convert.FromBase64String(Pad(parts[1].Replace('-', '+').Replace('_', '/')));
                using var doc = JsonDocument.Parse(payload);
                foreach (var p in doc.RootElement.EnumerateObject())
                {
                    if (p.Name == ClaimTypes.Role && p.Value.ToString().Contains(','))
                    {
                        userRolesValue = JsonSerializer.Deserialize<string[]>(p.Value)!;
                    }
                    else
                    {
                        claims.Add(new Claim(p.Name, p.Value.ToString()));
                    }
                }
            }
            catch { /* ignore parsing errors */ }

            // Extract and normalize role claims
            if (userRolesValue is not null)
            {
                var roleClaims = userRolesValue
                .Select(role => new Claim(ClaimTypes.Role, role.Trim()));

                claims.AddRange(roleClaims);
            }

            return claims;
        }

        private static string Pad(string s) => s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
    }
}