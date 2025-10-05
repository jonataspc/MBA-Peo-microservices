using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.Excluir;

public sealed record Command(Guid CursoId, Guid AulaId)
    : IRequest<Result<Response>>;
