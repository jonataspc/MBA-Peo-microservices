using Mapster;
using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.Dtos;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.ObterTodos;

public class Handler(IRepository<Domain.Entities.Curso> repository) : IRequestHandler<Query, Result<Response>>
{
    public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        var curso = await repository.GetAsync(request.CursoId);

        if (curso is null)
        {
            return Result.Failure<Response>(new Error(null!, "Curso não existe"));
        }

        return Result.Success(new Response(curso.Aulas.Adapt<IEnumerable<AulaResponse>>()));
    }
}