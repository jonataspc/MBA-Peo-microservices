using Peo.GestaoConteudo.Application.Dtos;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.ObterTodos;

public sealed record Response(IEnumerable<AulaResponse> Aulas);