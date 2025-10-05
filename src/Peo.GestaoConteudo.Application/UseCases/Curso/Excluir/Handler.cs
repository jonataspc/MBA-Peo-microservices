using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using System.Linq;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.Excluir;

public class Handler(IRepository<Domain.Entities.Curso> repository)
    : IRequestHandler<Command, Result<Response>>
{
    public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
    {
        var cursos = await repository.GetAsync(c => c.Id == request.CursoId);
        var curso = cursos.FirstOrDefault();

        if (curso is null)
            return Result.Failure<Response>(new Error("NotFound", "Curso não encontrado"));

        var aula = curso.Aulas?.FirstOrDefault(a => a.Id == request.AulaId);
        if (aula is null)
            return Result.Failure<Response>(new Error("NotFound", "Aula não encontrada"));

        curso.Aulas.Remove(aula);
        repository.Update(curso);
        await repository.UnitOfWork.CommitAsync(ct);

        return Result.Success(new Response(request.AulaId));
    }
}
