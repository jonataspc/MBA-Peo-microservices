using Newtonsoft.Json;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public class ObterTodosCursosResponse
    {
        [JsonProperty("cursos")]
        public required Curso[] Cursos { get; set; }
    }
}