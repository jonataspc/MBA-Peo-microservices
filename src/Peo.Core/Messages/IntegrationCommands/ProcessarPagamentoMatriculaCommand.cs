using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Dtos;

namespace Peo.Core.Messages.IntegrationCommands
{
    public class ProcessarPagamentoMatriculaCommand : IRequest<Result<ProcessarPagamentoMatriculaResponse>>
    {
        public Guid MatriculaId { get; private set; }
        public decimal Valor { get; private set; }
        public CartaoCredito DadosCartao { get; private set; }

        public ProcessarPagamentoMatriculaCommand(Guid matriculaId, decimal valor, CartaoCredito dadosCartao)
        {
            MatriculaId = matriculaId;
            Valor = valor;
            DadosCartao = dadosCartao;
        }
    }
}