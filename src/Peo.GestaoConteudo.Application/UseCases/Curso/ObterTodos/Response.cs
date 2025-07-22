using Peo.GestaoConteudo.Application.Dtos;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterTodos;

public sealed record Response(IEnumerable<CursoResponse> Cursos);