using System.Text.Json.Serialization;

namespace Peo.Web.Spa.Models;

// espelha AulaResponse do BFF + CursoId (usado só no POST)
public class AulaVm
{
    public Guid Id { get; set; }                 // do response
    public Guid CursoId { get; set; }            // usado no POST
    public string Titulo { get; set; } = "";
    public string? Descricao { get; set; }
    public string UrlVideo { get; set; } = "";
    public TimeSpan Duracao { get; set; }        // TimeSpan no BFF
    public List<ArquivoAulaVm> Arquivos { get; set; } = new();

    // helper para enviar TimeSpan no POST como "hh:mm:ss"
    public string DuracaoString => Duracao.ToString(@"hh\:mm\:ss");
}

public class ArquivoAulaVm
{
    public Guid Id { get; set; }                 // só no response
    public string Titulo { get; set; } = "";
    public string Url { get; set; } = "";
}

public class CadastrarAulaResult
{
    public Guid AulaId { get; set; }
}

public class CursoOption
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = "";
}
public class AulasEnvelope
{
    [JsonPropertyName("aulas")]
    public List<AulaVm> Aulas { get; set; } = new();
}