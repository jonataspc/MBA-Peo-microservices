namespace Peo.Web.Bff.Services.Identity.Dtos
{
    public record LoginResponse(string Token, Guid UserId);
}