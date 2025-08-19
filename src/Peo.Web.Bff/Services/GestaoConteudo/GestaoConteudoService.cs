using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.GestaoConteudo.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.GestaoConteudo
{
    public class GestaoConteudoService(HttpClient httpClient)
    {
        // Curso endpoints
        public async Task<Results<Ok<CadastrarCursoResponse>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, BadRequest, BadRequest<object>>> CadastrarCursoAsync(CursoRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/conteudo/curso/", request, ct);
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

            var cursoResponse = await response.Content.ReadFromJsonAsync<CadastrarCursoResponse>(cancellationToken: ct);
            if (cursoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize curso response");
            }

            return TypedResults.Ok(cursoResponse);
        }

        public async Task<Results<Ok<ObterTodosCursosResponse>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, BadRequest, BadRequest<object>>> ObterTodosCursosAsync(CancellationToken ct)
        {
            var response = await httpClient.GetAsync("/v1/conteudo/curso/", ct);
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

            var cursosResponse = await response.Content.ReadFromJsonAsync<ObterTodosCursosResponse>(cancellationToken: ct);
            if (cursosResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize cursos response");
            }

            return TypedResults.Ok(cursosResponse);
        }

        public async Task<Results<Ok<Curso>, NotFound, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<object>>> ObterCursoPorIdAsync(Guid id, CancellationToken ct)
        {
            var response = await httpClient.GetAsync($"/v1/conteudo/curso/{id}", ct);
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

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return TypedResults.NotFound();
                }

                throw new HttpRequestException($"Request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
            }

            var cursoResponse = await response.Content.ReadFromJsonAsync<Curso>(cancellationToken: ct);
            if (cursoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize curso response");
            }

            return TypedResults.Ok(cursoResponse);
        }

        // Aula endpoints
        public async Task<Results<Ok<ObterTodasAulasResponse>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, BadRequest, BadRequest<object>>> ObterAulasDoCursoAsync(Guid cursoId, CancellationToken ct)
        {
            var response = await httpClient.GetAsync($"/v1/conteudo/curso/{cursoId}/aula", ct);
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

            var aulasResponse = await response.Content.ReadFromJsonAsync<ObterTodasAulasResponse>(cancellationToken: ct);
            if (aulasResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize aulas response");
            }

            return TypedResults.Ok(aulasResponse);
        }

        public async Task<Results<Ok<CadastrarAulaResponse>, ValidationProblem, UnauthorizedHttpResult, ForbidHttpResult, BadRequest, BadRequest<object>>> CadastrarAulaAsync(Guid cursoId, AulaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync($"/v1/conteudo/curso/{cursoId}/aula", request, ct);
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

            var aulaResponse = await response.Content.ReadFromJsonAsync<CadastrarAulaResponse>(cancellationToken: ct);
            if (aulaResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize aula response");
            }

            return TypedResults.Ok(aulaResponse);
        }
    }
}