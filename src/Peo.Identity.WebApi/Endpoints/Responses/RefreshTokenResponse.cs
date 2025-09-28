namespace Peo.Identity.WebApi.Endpoints.Responses
{
    public record RefreshTokenResponse(string Token, Guid UserId);
}