using Peo.Core.DomainObjects.Result;
using Peo.Web.Bff.Services.Identity.Dtos;

namespace Peo.Web.Bff.Services.Identity
{
    public class IdentityService(HttpClient httpClient)
    {
        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/identity/register", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure(await response.Content.ReadAsStringAsync(ct));
            }

            return Result.Success();
        }
    }
}