using Newtonsoft.Json;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public class ObterTodosCursosResponse
    {
        
        public required Curso[] Cursos { get; set; }
    }
}