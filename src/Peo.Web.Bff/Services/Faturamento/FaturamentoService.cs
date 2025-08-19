using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.Faturamento.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.Faturamento
{
    public class FaturamentoService(HttpClient httpClient)
    {
        public async Task<Results<Ok<EfetuarPagamentoResponse>, ValidationProblem, ForbidHttpResult, UnauthorizedHttpResult, BadRequest<object>>> EfetuarPagamentoAsync(EfetuarPagamentoRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/faturamento/matricula/pagamento", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return TypedResults.Forbid();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<EfetuarPagamentoResponse>(cancellationToken: ct);
            if (loginResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize payment response");
            }

            return TypedResults.Ok(loginResponse);
        }
    }
}