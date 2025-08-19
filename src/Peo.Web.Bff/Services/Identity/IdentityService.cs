using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.Identity.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.Identity
{
    public class IdentityService(HttpClient httpClient)
    {
        public async Task<Results<Ok, UnauthorizedHttpResult, BadRequest<object>>> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/identity/register", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            return TypedResults.Ok();
        }

        public async Task<Results<Ok<LoginResponse>, ValidationProblem, UnauthorizedHttpResult, BadRequest<object>>> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/identity/login", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
            if (loginResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize login response");
            }

            return TypedResults.Ok(loginResponse);
        }

        public async Task<Results<Ok<RefreshTokenResponse>, ValidationProblem, UnauthorizedHttpResult, BadRequest<object>>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/identity/refresh-token", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var refreshTokenResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: ct);
            if (refreshTokenResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize refresh token response");
            }

            return TypedResults.Ok(refreshTokenResponse);
        }
    }
}