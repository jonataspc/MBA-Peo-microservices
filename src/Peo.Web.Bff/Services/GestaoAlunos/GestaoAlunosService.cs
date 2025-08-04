using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.GestaoAlunos.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.GestaoAlunos
{
    public class GestaoAlunosService(HttpClient httpClient)
    {
        // Matricula endpoints
        public async Task<Results<Ok<MatriculaCursoResponse>, ValidationProblem, BadRequest, BadRequest<object>>> MatricularCursoAsync(MatriculaCursoRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/estudante/matricula/", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var matriculaResponse = await response.Content.ReadFromJsonAsync<MatriculaCursoResponse>(cancellationToken: ct);
            if (matriculaResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize matricula response");
            }

            return TypedResults.Ok(matriculaResponse);
        }

        public async Task<Results<Ok<PagamentoMatriculaResponse>, ValidationProblem, BadRequest, BadRequest<object>>> PagarMatriculaAsync(PagamentoMatriculaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/estudante/matricula/pagamento", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var pagamentoResponse = await response.Content.ReadFromJsonAsync<PagamentoMatriculaResponse>(cancellationToken: ct);
            if (pagamentoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize pagamento response");
            }

            return TypedResults.Ok(pagamentoResponse);
        }

        public async Task<Results<Ok<ConcluirMatriculaResponse>, ValidationProblem, BadRequest, BadRequest<object>>> ConcluirMatriculaAsync(ConcluirMatriculaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/estudante/matricula/concluir", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var concluirResponse = await response.Content.ReadFromJsonAsync<ConcluirMatriculaResponse>(cancellationToken: ct);
            if (concluirResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize concluir matricula response");
            }

            return TypedResults.Ok(concluirResponse);
        }

        // Aula endpoints
        public async Task<Results<Ok<ProgressoAulaResponse>, ValidationProblem, BadRequest, BadRequest<object>>> IniciarAulaAsync(IniciarAulaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/estudante/matricula/aula/iniciar", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var progressoResponse = await response.Content.ReadFromJsonAsync<ProgressoAulaResponse>(cancellationToken: ct);
            if (progressoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize progresso aula response");
            }

            return TypedResults.Ok(progressoResponse);
        }

        public async Task<Results<Ok<ProgressoAulaResponse>, ValidationProblem, BadRequest, BadRequest<object>>> ConcluirAulaAsync(ConcluirAulaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/estudante/matricula/aula/concluir", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var progressoResponse = await response.Content.ReadFromJsonAsync<ProgressoAulaResponse>(cancellationToken: ct);
            if (progressoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize progresso aula response");
            }

            return TypedResults.Ok(progressoResponse);
        }

        // Certificados endpoint
        public async Task<Results<Ok<IEnumerable<CertificadoEstudanteResponse>>, BadRequest, BadRequest<object>>> ObterCertificadosAsync(CancellationToken ct)
        {
            var response = await httpClient.GetAsync("/v1/estudante/certificados", ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var certificadosResponse = await response.Content.ReadFromJsonAsync<IEnumerable<CertificadoEstudanteResponse>>(cancellationToken: ct);
            if (certificadosResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize certificados response");
            }

            return TypedResults.Ok(certificadosResponse);
        }
    }
} 