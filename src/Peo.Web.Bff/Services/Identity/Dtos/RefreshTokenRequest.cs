using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.Identity.Dtos
{
    public record RefreshTokenRequest(
        [Required]
        string Token);
}