using Newtonsoft.Json;

namespace Peo.Web.Bff.Services.Identity.Dtos
{
    public class AlunoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}