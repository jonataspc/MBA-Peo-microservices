using System.ComponentModel.DataAnnotations;

namespace Peo.Identity.WebApi.Endpoints.Requests
{
    public record RefreshTokenRequest(
        [Required]
        string Token);
}