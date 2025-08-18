using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;
using Peo.Faturamento.Application.Endpoints.Pagamento;

namespace Peo.Faturamento.Application.Endpoints
{
    public static class EndpointsFaturamento
    {
        public static void MapFaturamentoEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/faturamento")
            .WithTags("Faturamento")
            .MapEndpoint<EndpointPagamentoMatricula>()
            ;
        }
    }
}