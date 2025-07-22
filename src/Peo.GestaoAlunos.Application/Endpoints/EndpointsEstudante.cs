using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;
using Peo.GestaoAlunos.Application.Endpoints.Aula;
using Peo.GestaoAlunos.Application.Endpoints.Estudante;
using Peo.GestaoAlunos.Application.Endpoints.Matricula;

namespace Peo.GestaoAlunos.Application.Endpoints
{
    public static class EndpointsEstudante
    {
        public static void MapEstudanteEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/estudante")
            .WithTags("Estudante")
            .MapEndpoint<EndpointMatriculaCurso>()
            .MapEndpoint<EndpointPagamentoMatricula>()
            .MapEndpoint<EndpointConcluirMatricula>()
            .MapEndpoint<EndpointCertificadosEstudante>()
            .MapEndpoint<EndpointsAula>()
            ;
        }
    }
}