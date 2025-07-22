namespace Peo.Identity.Application.Endpoints.Responses
{
    public record LoginResponse(string Token, Guid UserId);
}