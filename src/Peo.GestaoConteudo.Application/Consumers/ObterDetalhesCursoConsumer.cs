using MassTransit;
using Peo.Core.Interfaces.Data;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.GestaoConteudo.Domain.Entities;

namespace Peo.GestaoConteudo.Application.Consumers
{
    public class ObterDetalhesCursoConsumer(IRepository<Curso> cursoRepository) : IConsumer<ObterDetalhesCursoRequest>
    {
        public async Task Consume(ConsumeContext<ObterDetalhesCursoRequest> context)
        {
            var curso = await cursoRepository.GetAsync(context.Message.CursoId);

            await context.RespondAsync<ObterDetalhesCursoResponse>(
                new ObterDetalhesCursoResponse(curso?.Id, curso?.Aulas.Count, curso?.Titulo, curso?.Preco)
            );
        }
    }
}