using System.Net.Http.Json;
using System.Text.Json;
using Peo.Web.Spa.Models;

namespace Peo.Web.Spa.Services;

public class AulasService
{
    private readonly HttpClient _http;
    private const string V1 = "v1/";
    public AulasService(IHttpClientFactory f) => _http = f.CreateClient("Api");

    // rotas RELATIVAS ao BaseAddress (sem barra inicial!)
    private const string CursosPath = $"{V1}conteudo/curso";
    private static string ListarAulas(Guid cursoId) => $"{V1}conteudo/curso/{cursoId}/aula";
    private static string CadastrarAula(Guid cursoId) => $"{V1}conteudo/curso/{cursoId}/aula";

    public async Task<List<AulaVm>> ObterAulasDoCurso(Guid cursoId)
    {
        var url = new Uri(_http.BaseAddress!, ListarAulas(cursoId)); // força URL absoluta
        Console.WriteLine($"[SPA] GET {url}");

        var resp = await _http.GetAsync(url);
        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"GET aulas falhou: {(int)resp.StatusCode} {resp.ReasonPhrase}");

        var env = await resp.Content.ReadFromJsonAsync<AulasEnvelope>(
            new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

        return env?.Aulas ?? new();
    }


    public async Task<List<CursoOption>> ObterCursos()
    {
        var url = new Uri(_http.BaseAddress!, $"{V1}conteudo/curso");
        var resp = await _http.GetAsync(url);
        if (!resp.IsSuccessStatusCode)
        {
            var txt = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"GET cursos falhou: {(int)resp.StatusCode} {resp.ReasonPhrase}. Resp: {txt}");
        }

        var opts = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);

        // o BFF devolve { cursos: [...] }
        var envelope = await resp.Content.ReadFromJsonAsync<CursosEnvelope>(opts);

        return envelope?.Cursos?
            .Select(c => new CursoOption { Id = c.Id, Titulo = c.Titulo })
            .ToList() ?? new();
    }

    // tipos internos só para desserializar o envelope vindo do BFF
    private sealed class CursosEnvelope
    {
        public List<CursoDto> Cursos { get; set; } = new();
    }

    private sealed class CursoDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = "";
    }

    public async Task<(bool ok, string? msg, Guid? aulaId)> Cadastrar(AulaVm vm)
    {
        var body = new
        {
            titulo = vm.Titulo,
            descricao = vm.Descricao,
            urlVideo = vm.UrlVideo,
            duracao = vm.DuracaoString,                 // "hh:mm:ss"
            arquivos = vm.Arquivos.Select(a => new { titulo = a.Titulo, url = a.Url })
        };

        // 🔒 monta URL absoluta (evita perder o /v1)
        var url = new Uri(_http.BaseAddress!, CadastrarAula(vm.CursoId));
        var resp = await _http.PostAsJsonAsync(url, body);

        if (!resp.IsSuccessStatusCode)
            return (false, await resp.Content.ReadAsStringAsync(), null);

        var data = await resp.Content.ReadFromJsonAsync<CadastrarAulaResult>(
            new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

        return (true, null, data?.AulaId);
    }

    private sealed class CadastrarAulaResult
    {
        [System.Text.Json.Serialization.JsonPropertyName("aulaId")]
        public Guid AulaId { get; set; }
    }

    public async Task<(bool ok, string? msg)> ExcluirAula(Guid cursoId, Guid aulaId)
    {
        var url = new Uri(_http.BaseAddress!, $"{V1}conteudo/curso/{cursoId}/aula/{aulaId}");
        var resp = await _http.DeleteAsync(url);

        if (resp.IsSuccessStatusCode) return (true, null);
        var body = await resp.Content.ReadAsStringAsync();
        return (false, $"DELETE falhou: {(int)resp.StatusCode} {resp.ReasonPhrase}. Resp: {body}");
    }

}
