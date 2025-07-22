using System.ComponentModel.DataAnnotations;

namespace Peo.Identity.Application.Endpoints.Requests
{
    public record RefreshTokenRequest(
        [Required]
        string Token);
}