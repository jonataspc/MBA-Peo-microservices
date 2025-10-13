using MassTransit;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Repositories;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Application.Consumers
{
    public class ObterMatriculaConsumer(IAlunoRepository alunoRepository) : IConsumer<ObterMatriculaRequest>
    {
        public async Task Consume(ConsumeContext<ObterMatriculaRequest> context)
        {
            var cancellationToken = context.CancellationToken;
            Matricula? matricula = await alunoRepository.GetMatriculaByIdAsync(context.Message.MatriculaId, cancellationToken);

            await context.RespondAsync(
                new ObterMatriculaResponse(matricula?.Id, matricula?.CursoId, matricula?.Status == StatusMatricula.PendentePagamento)
            );
        }
    }
}