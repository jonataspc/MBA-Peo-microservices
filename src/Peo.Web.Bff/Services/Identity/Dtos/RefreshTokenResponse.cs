namespace Peo.Web.Bff.Services.Identity.Dtos
{
    public record RefreshTokenResponse(string Token, Guid UserId);
}