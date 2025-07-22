using Microsoft.AspNetCore.Routing;

namespace Peo.Core.Web.Api;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}