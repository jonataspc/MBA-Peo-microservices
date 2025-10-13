using Peo.Core.Web.Extensions;
using Peo.Identity.WebApi.Endpoints;

namespace Peo.Identity.WebApi.Extensions
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