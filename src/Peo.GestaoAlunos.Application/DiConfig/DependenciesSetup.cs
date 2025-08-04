using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Core.Messages.IntegrationEvents;
using Peo.GestaoAlunos.Application.Commands.Aula;
using Peo.GestaoAlunos.Application.Commands.Matricula;
using Peo.GestaoAlunos.Application.Commands.MatriculaCurso;
using Peo.GestaoAlunos.Application.Commands.PagamentoMatricula;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.EventHandlers;
using Peo.GestaoAlunos.Application.Queries.ObterCertificadosEstudante;
using Peo.GestaoAlunos.Application.Services;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.DiConfig
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForGestaoAlunos(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(EstudanteService).Assembly);
            });

            // Handlers
            services.AddScoped<INotificationHandler<PagamentoMatriculaConfirmadoEvent>, PagamentoMatriculaEventHandlers>();
            services.AddScoped<INotificationHandler<PagamentoComFalhaEvent>, PagamentoMatriculaEventHandlers>();

            services.AddScoped<IRequestHandler<MatriculaCursoCommand, Result<MatriculaCursoResponse>>, MatriculaCursoCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirMatriculaCommand, Result<ConcluirMatriculaResponse>>, ConcluirMatriculaCommandHandler>();
            services.AddScoped<IRequestHandler<PagamentoMatriculaCommand, Result<PagamentoMatriculaResponse>>, PagamentoMatriculaCommandHandler>();
            services.AddScoped<IRequestHandler<IniciarAulaCommand, Result<ProgressoAulaResponse>>, IniciarAulaCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirAulaCommand, Result<ProgressoAulaResponse>>, ConcluirAulaCommandHandler>();
            services.AddScoped<IRequestHandler<ObterCertificadosEstudanteQuery, Result<IEnumerable<CertificadoEstudanteResponse>>>, ObterCertificadosEstudanteQueryHandler>();

            // Application services
            services.AddScoped<IEstudanteService, EstudanteService>();

            // Anti-corruption layers
            //TODO! usar troca de mensagens para integrar com o sistema de conteudo
            services.AddScoped<ICursoAulaService>(x => { return null!; });
            services.AddScoped<IDetalhesUsuarioService>(x => { return null!; });

            return services;
        }
    }
}