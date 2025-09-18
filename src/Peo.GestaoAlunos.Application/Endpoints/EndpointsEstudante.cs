using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;
using Peo.GestaoAlunos.Application.Endpoints.Aula;
using Peo.GestaoAlunos.Application.Endpoints.Aluno;
using Peo.GestaoAlunos.Application.Endpoints.Matricula;

namespace Peo.GestaoAlunos.Application.Endpoints
{
    public static class EndpointsAluno
    {
        public static void MapAlunoEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/aluno")
            .WithTags("Aluno")
            .MapEndpoint<EndpointMatriculaCurso>()
            .MapEndpoint<EndpointObterMatriculas>()
            .MapEndpoint<EndpointConcluirMatricula>()
            .MapEndpoint<EndpointCertificadosAluno>()
            .MapEndpoint<EndpointsAula>()
            ;
        }
    }
}