using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.Faturamento.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.Faturamento
{
    public class FaturamentoService(HttpClient httpClient)
    {
        public async Task<Results<Ok<EfetuarPagamentoResponse>, ValidationProblem, UnauthorizedHttpResult, BadRequest<object>>> EfetuarPagamentoAsync(EfetuarPagamentoRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/faturamento/matricula/pagamento", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<EfetuarPagamentoResponse>(cancellationToken: ct);
            if (loginResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize login response");
            }

            return TypedResults.Ok(loginResponse);
        }
    }
}