using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Peo.Core.Web.Extensions;
using Peo.Identity.Application.Endpoints;

namespace Peo.Identity.Application.Extensions
{
    public static class EndpointsConfig
    {
        public static WebApplication MapIdentityEndpoints(this WebApplication app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoint<RegisterEndpoint>()
            .MapEndpoint<LoginEndpoint>()
            .MapEndpoint<RefreshTokenEndpoint>();

            return app;
        }
    }
}