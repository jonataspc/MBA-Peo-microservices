using Peo.Core.Web.Extensions;
using Peo.GestaoAlunos.WebApi.Endpoints.Aluno;
using Peo.GestaoAlunos.WebApi.Endpoints.Aula;
using Peo.GestaoAlunos.WebApi.Endpoints.Matricula;

namespace Peo.GestaoAlunos.WebApi.Endpoints
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
            .MapEndpoint<EndpointObterAulasMatricula>()
            .MapEndpoint<EndpointConcluirMatricula>()
            .MapEndpoint<EndpointObterHistorico>()
            .MapEndpoint<EndpointCertificadosAluno>()
            .MapEndpoint<EndpointsAula>()
            ;
        }
    }
}