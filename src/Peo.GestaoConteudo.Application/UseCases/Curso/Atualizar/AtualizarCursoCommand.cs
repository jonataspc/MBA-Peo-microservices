
using MediatR;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.Atualizar
{
    public class AtualizarCursoCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool EstaPublicado { get; set; }
        public List<string> Tags { get; set; }

        public AtualizarCursoCommand()
        {
            Titulo = string.Empty;
            Tags = new List<string>();
        }
    }
}
