using System.ComponentModel.DataAnnotations;

namespace Peo.Identity.Application.Endpoints.Requests
{
    public record LoginRequest(
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        string Email,

        [Required]
        string Password);
}