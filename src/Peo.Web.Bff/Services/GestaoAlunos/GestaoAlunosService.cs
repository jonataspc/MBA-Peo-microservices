using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.GestaoAlunos.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.GestaoAlunos
{
    public class GestaoAlunosService(HttpClient httpClient)
    {        
        public async Task<Results<Ok<MatriculaCursoResponse>, ValidationProblem, UnauthorizedHttpResult, BadRequest, BadRequest<object>>> MatricularCursoAsync(MatriculaCursoRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/aluno/matricula/", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var matriculaResponse = await response.Content.ReadFromJsonAsync<MatriculaCursoResponse>(cancellationToken: ct);
            if (matriculaResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize matricula response");
            }

            return TypedResults.Ok(matriculaResponse);
        }


        public async Task<Results<Ok<IEnumerable<MatriculaResponse>>, ValidationProblem, UnauthorizedHttpResult, BadRequest, BadRequest<object>>> ConsultarMatriculasAlunoAsync(CancellationToken ct)
        {
            var response = await httpClient.GetAsync("/v1/aluno/matricula/", ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var matriculaResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MatriculaResponse>>(cancellationToken: ct);
            if (matriculaResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize matricula response");
            }

            return TypedResults.Ok(matriculaResponse);
        }


        public async Task<Results<Ok<IEnumerable<AulaMatriculaResponse>>, ValidationProblem, UnauthorizedHttpResult, BadRequest, BadRequest<object>>> ObterAulasMatriculaAsync(Guid matriculaId, CancellationToken ct)
        {
            var response = await httpClient.GetAsync($"/v1/aluno/matricula/{matriculaId}/aulas", ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var aulasResponse = await response.Content.ReadFromJsonAsync<IEnumerable<AulaMatriculaResponse>>(cancellationToken: ct);
            if (aulasResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize aulas matricula response");
            }

            return TypedResults.Ok(aulasResponse);
        }

        public async Task<Results<Ok<ConcluirMatriculaResponse>, ValidationProblem, BadRequest, UnauthorizedHttpResult, BadRequest<object>>> ConcluirMatriculaAsync(ConcluirMatriculaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/aluno/matricula/concluir", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var concluirResponse = await response.Content.ReadFromJsonAsync<ConcluirMatriculaResponse>(cancellationToken: ct);
            if (concluirResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize concluir matricula response");
            }

            return TypedResults.Ok(concluirResponse);
        }

        // Aula endpoints
        public async Task<Results<Ok<ProgressoAulaResponse>, ValidationProblem, UnauthorizedHttpResult, BadRequest, BadRequest<object>>> IniciarAulaAsync(IniciarAulaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/aluno/matricula/aula/iniciar", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var progressoResponse = await response.Content.ReadFromJsonAsync<ProgressoAulaResponse>(cancellationToken: ct);
            if (progressoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize progresso aula response");
            }

            return TypedResults.Ok(progressoResponse);
        }

        public async Task<Results<Ok<ProgressoAulaResponse>, ValidationProblem, UnauthorizedHttpResult, BadRequest, BadRequest<object>>> ConcluirAulaAsync(ConcluirAulaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/aluno/matricula/aula/concluir", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var progressoResponse = await response.Content.ReadFromJsonAsync<ProgressoAulaResponse>(cancellationToken: ct);
            if (progressoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize progresso aula response");
            }

            return TypedResults.Ok(progressoResponse);
        }

        // Certificados endpoint
        public async Task<Results<Ok<IEnumerable<CertificadoAlunoResponse>>, BadRequest, UnauthorizedHttpResult, BadRequest<object>>> ObterCertificadosAsync(CancellationToken ct)
        {
            var response = await httpClient.GetAsync("/v1/aluno/certificados", ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var certificadosResponse = await response.Content.ReadFromJsonAsync<IEnumerable<CertificadoAlunoResponse>>(cancellationToken: ct);
            if (certificadosResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize certificados response");
            }

            return TypedResults.Ok(certificadosResponse);
        }

        // historico endpoint
        public async Task<Results<Ok<IEnumerable<HistoricoAlunoProgressoResponse>>, BadRequest, UnauthorizedHttpResult, BadRequest<object>>> ObterHistoricoAsync(CancellationToken ct)
        {
            var response = await httpClient.GetAsync("/v1/aluno/progresso-matriculas", ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Unauthorized();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var historicoAlunoResponse = await response.Content.ReadFromJsonAsync<IEnumerable<HistoricoAlunoProgressoResponse>>(cancellationToken: ct);
            if (historicoAlunoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize historico response");
            }

            return TypedResults.Ok(historicoAlunoResponse);
        }
    }
}