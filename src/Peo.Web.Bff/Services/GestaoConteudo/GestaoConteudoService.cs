using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.GestaoConteudo.Dtos;
using System.Net;

namespace Peo.Web.Bff.Services.GestaoConteudo
{
    public class GestaoConteudoService(HttpClient httpClient)
    {
        // Curso endpoints
        public async Task<Results<Ok<CursoResponse>, ValidationProblem, BadRequest, BadRequest<object>>> CadastrarCursoAsync(CursoRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync("/v1/conteudo/curso/", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var cursoResponse = await response.Content.ReadFromJsonAsync<CursoResponse>(cancellationToken: ct);
            if (cursoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize curso response");
            }

            return TypedResults.Ok(cursoResponse);
        }

        public async Task<Results<Ok<IEnumerable<CursoResponse>>, ValidationProblem, BadRequest, BadRequest<object>>> ObterTodosCursosAsync(CancellationToken ct)
        {
            var response = await httpClient.GetAsync("/v1/conteudo/curso/", ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var cursosResponse = await response.Content.ReadFromJsonAsync<IEnumerable<CursoResponse>>(cancellationToken: ct);
            if (cursosResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize cursos response");
            }

            return TypedResults.Ok(cursosResponse);
        }

        public async Task<Results<Ok<CursoResponse>, NotFound, ValidationProblem, BadRequest, BadRequest<object>>> ObterCursoPorIdAsync(Guid id, CancellationToken ct)
        {
            var response = await httpClient.GetAsync($"/v1/conteudo/curso/{id}", ct);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var cursoResponse = await response.Content.ReadFromJsonAsync<CursoResponse>(cancellationToken: ct);
            if (cursoResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize curso response");
            }

            return TypedResults.Ok(cursoResponse);
        }

        // Aula endpoints
        public async Task<Results<Ok<IEnumerable<AulaResponse>>, ValidationProblem, BadRequest, BadRequest<object>>> ObterAulasDoCursoAsync(Guid cursoId, CancellationToken ct)
        {
            var response = await httpClient.GetAsync($"/v1/conteudo/curso/{cursoId}/aula", ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var aulasResponse = await response.Content.ReadFromJsonAsync<IEnumerable<AulaResponse>>(cancellationToken: ct);
            if (aulasResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize aulas response");
            }

            return TypedResults.Ok(aulasResponse);
        }

        public async Task<Results<Ok<AulaResponse>, ValidationProblem, BadRequest, BadRequest<object>>> CadastrarAulaAsync(Guid cursoId, AulaRequest request, CancellationToken ct)
        {
            var response = await httpClient.PostAsJsonAsync($"/v1/conteudo/curso/{cursoId}/aula", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return TypedResults.BadRequest(await response.Content.ReadFromJsonAsync<object>(cancellationToken: ct));
            }

            var aulaResponse = await response.Content.ReadFromJsonAsync<AulaResponse>(cancellationToken: ct);
            if (aulaResponse == null)
            {
                return TypedResults.BadRequest<object>("Failed to deserialize aula response");
            }

            return TypedResults.Ok(aulaResponse);
        }
    }
} 