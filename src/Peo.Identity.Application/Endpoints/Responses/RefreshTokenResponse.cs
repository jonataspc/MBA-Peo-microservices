namespace Peo.Identity.Application.Endpoints.Responses
{
    public record RefreshTokenResponse(string Token, Guid UserId);
}