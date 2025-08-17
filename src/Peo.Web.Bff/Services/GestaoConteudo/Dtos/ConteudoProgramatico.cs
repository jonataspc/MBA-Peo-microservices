using Newtonsoft.Json;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public class ConteudoProgramatico
    {
        [JsonProperty("conteudo")]
        public required string Conteudo { get; set; }
    }
}