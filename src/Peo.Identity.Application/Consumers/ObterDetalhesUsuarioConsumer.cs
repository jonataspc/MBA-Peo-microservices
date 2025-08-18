using MassTransit;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Consumers
{
    public class ObterDetalhesUsuarioConsumer(IUserService userService) : IConsumer<ObterDetalhesUsuarioRequest>
    {
        public async Task Consume(ConsumeContext<ObterDetalhesUsuarioRequest> context)
        {
            var usuario = await userService.ObterUsuarioPorIdAsync(context.Message.UsuarioId);

            await context.RespondAsync(
                new ObterDetalhesUsuarioResponse(usuario?.NomeCompleto)
            );
        }
    }
}