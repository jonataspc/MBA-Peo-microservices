namespace Peo.Identity.WebApi.Endpoints.Responses
{
    public record LoginResponse(string Token, Guid UserId);
}