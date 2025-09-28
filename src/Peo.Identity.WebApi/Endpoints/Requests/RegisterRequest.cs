using System.ComponentModel.DataAnnotations;

namespace Peo.Identity.WebApi.Endpoints.Requests
{
    public record RegisterRequest(
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        string Email,

        [Required]
        string Password,

        [Required]
        string Name,

        bool IsAdmin = false

        );
}