using Mapster;
using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.Dtos;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterTodos;

public class Handler(IRepository<Domain.Entities.Curso> repository) : IRequestHandler<Query, Result<Response>>
{
    public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cursos = await repository.GetAllAsync();
        return Result.Success(new Response(cursos.Adapt<IEnumerable<CursoResponse>>()));
    }
}