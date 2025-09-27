using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Dtos;
using Peo.Core.Messages.IntegrationCommands;
using Peo.Faturamento.Domain.Interfaces.Services;

namespace Peo.Faturamento.Application.Handlers
{
    public class ProcessarPagamentoMatriculaCommandHandler
        (
        IPagamentoService pagamentoService
        ) : IRequestHandler<ProcessarPagamentoMatriculaCommand, Result<ProcessarPagamentoMatriculaResponse>>
    {
        public async Task<Result<ProcessarPagamentoMatriculaResponse>> Handle(ProcessarPagamentoMatriculaCommand request, CancellationToken cancellationToken)
        {
            var pagamento = await pagamentoService.ProcessarPagamentoMatriculaAsync(request.MatriculaId, request.Valor, request.DadosCartao, cancellationToken);

            if (pagamento.Status == Domain.ValueObjects.StatusPagamento.Falha)
            {
                return Result.Failure<ProcessarPagamentoMatriculaResponse>(new Error($"Pagamento falhou: {pagamento.Detalhes}"));
            }

            return Result.Success(new ProcessarPagamentoMatriculaResponse(true, pagamento.Status.ToString()));
        }
    }
}