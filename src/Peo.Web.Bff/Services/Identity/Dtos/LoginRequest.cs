using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.Identity.Dtos
{
    public record LoginRequest(
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        string Email,

        [Required]
        string Password);
}