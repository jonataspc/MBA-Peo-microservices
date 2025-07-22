using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Api;

namespace Peo.Core.Web.Extensions
{
    public static class EndpointsExtensions
    {
        public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}