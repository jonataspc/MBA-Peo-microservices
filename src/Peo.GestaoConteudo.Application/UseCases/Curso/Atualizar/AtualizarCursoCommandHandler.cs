
using MediatR;
using Peo.Core.Interfaces.Data;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.Atualizar
{
    public class AtualizarCursoCommandHandler : IRequestHandler<AtualizarCursoCommand, Unit>
    {
        private readonly IRepository<Domain.Entities.Curso> _cursoRepository;

        public AtualizarCursoCommandHandler(IRepository<Domain.Entities.Curso> cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<Unit> Handle(AtualizarCursoCommand request, CancellationToken cancellationToken)
        {
            var curso = await _cursoRepository.WithTracking().GetAsync(request.Id);

            if (curso == null)
            {
                // Lançar exceção ou retornar um resultado de falha
                return Unit.Value;
            }

            curso.AtualizarTituloDescricao(request.Titulo, request.Descricao);

            _cursoRepository.Update(curso);
            await _cursoRepository.UnitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
