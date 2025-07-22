using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterTodos;

public sealed record Query() : IRequest<Result<Response>>;