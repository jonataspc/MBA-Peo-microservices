using Newtonsoft.Json;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public class Curso
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("titulo")]
        public required string Titulo { get; set; }

        [JsonProperty("descricao")]
        public required string Descricao { get; set; }

        [JsonProperty("InstrutorNome")]
        public required string InstrutorNome { get; set; }

        [JsonProperty("conteudoProgramatico")]
        public required ConteudoProgramatico ConteudoProgramatico { get; set; }

        [JsonProperty("preco")]
        public required decimal Preco { get; set; }

        [JsonProperty("estaPublicado")]
        public bool EstaPublicado { get; set; }

        [JsonProperty("dataPublicacao")]
        public required DateTime DataPublicacao { get; set; }

        [JsonProperty("tags")]
        public required string[] Tags { get; set; }
    }
}