using Mapster;
using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.Dtos;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterConteudoProgramatico;

public class Handler(IRepository<Domain.Entities.Curso> repository) : IRequestHandler<Query, Result<Response>>
{
    public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cursos = await repository.GetAsync(request.CursoId, CancellationToken.None);
        return Result.Success(new Response(cursos.Adapt<ConteudoProgramaticoResponse>()));
    }
}