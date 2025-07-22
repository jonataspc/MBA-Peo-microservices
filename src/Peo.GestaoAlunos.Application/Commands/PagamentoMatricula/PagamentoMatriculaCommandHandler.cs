using Peo.Core.Interfaces.Services.Acls;
using Peo.Core.Messages.IntegrationCommands;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Application.Commands.PagamentoMatricula;

public class PagamentoMatriculaCommandHandler : IRequestHandler<PagamentoMatriculaCommand, Result<PagamentoMatriculaResponse>>
{
    private readonly IEstudanteRepository _estudanteRepository;
    private readonly IMediator _mediator;
    private readonly ICursoAulaService _aulaCursoService;

    public PagamentoMatriculaCommandHandler(
        IEstudanteRepository estudanteRepository,
        IMediator mediator,
        ICursoAulaService aulaCursoService)
    {
        _estudanteRepository = estudanteRepository;
        _mediator = mediator;
        _aulaCursoService = aulaCursoService;
    }

    public async Task<Result<PagamentoMatriculaResponse>> Handle(PagamentoMatriculaCommand request, CancellationToken cancellationToken)
    {
        var matricula = await _estudanteRepository.GetMatriculaByIdAsync(request.Request.MatriculaId);

        if (matricula is null)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error("Matrícula não encontrada"));
        }

        if (matricula.Status != StatusMatricula.PendentePagamento)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error($"Não é possível processar pagamento para matrícula com status {matricula.Status}"));
        }

        var preco = await _aulaCursoService.ObterPrecoCursoAsync(matricula.CursoId);

        var resultPagamento = await _mediator.Send(new ProcessarPagamentoMatriculaCommand(matricula.Id, preco, request.Request.DadosCartao), cancellationToken);

        if (resultPagamento.IsFailure)
        {
            return Result.Failure<PagamentoMatriculaResponse>(resultPagamento.Error);
        }

        var response = new PagamentoMatriculaResponse(matricula.Id, resultPagamento.Value?.StatusPagamento, preco);

        return Result.Success(response);
    }
}