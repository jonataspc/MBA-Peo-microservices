using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.ObterTodos;

public sealed record Query(Guid CursoId) : IRequest<Result<Response>>;