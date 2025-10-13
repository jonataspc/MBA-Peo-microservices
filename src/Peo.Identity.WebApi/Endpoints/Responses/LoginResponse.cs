namespace Peo.Identity.WebApi.Endpoints.Responses
{
    public record LoginResponse(string Token, Guid UserId);
    public record UserDto(Guid Id, string Nome, string Email);
}