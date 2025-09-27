using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.Cadastrar;

public class Handler(IRepository<Domain.Entities.Curso> repository) : IRequestHandler<Command, Result<Response>>
{
    public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
    {
        var curso = new Domain.Entities.Curso(

            titulo: request.Titulo,
            descricao: request.Descricao,
            instrutorNome: request.InstrutorNome,
            conteudoProgramatico: new ConteudoProgramatico(request.ConteudoProgramatico),
            preco: request.Preco,
            estaPublicado: true,
            dataPublicacao: DateTime.Now,
            tags: request.Tags ?? [],
            aulas: []
        );

        repository.Insert(curso, cancellationToken);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new Response(curso.Id));
    }
}