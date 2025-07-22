using Mapster;
using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.Cadastrar;

public class Handler(IRepository<Domain.Entities.Curso> repository) : IRequestHandler<Command, Result<Response>>
{
    public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
    {
        var curso = await repository.WithTracking()
                                     .GetAsync(request.CursoId);

        if (curso is null)
        {
            return Result.Failure<Response>(new Error("Curso não encontrado"));
        }

        var aula = request.Adapt<Domain.Entities.Aula>();

        curso.Aulas.Add(aula);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new Response(aula.Id));
    }
}