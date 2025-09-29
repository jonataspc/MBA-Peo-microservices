using Microsoft.Extensions.DependencyInjection;
using Peo.GestaoAlunos.Application.Commands.Aula;
using Peo.GestaoAlunos.Application.Commands.Matricula;
using Peo.GestaoAlunos.Application.Commands.MatriculaCurso;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterCertificadosAluno;
using Peo.GestaoAlunos.Application.Services;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.DependencyInjectionConfiguration
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForGestaoAlunos(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(AlunoService).Assembly);
            });

            services.AddScoped<IRequestHandler<MatriculaCursoCommand, Result<MatriculaCursoResponse>>, MatriculaCursoCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirMatriculaCommand, Result<ConcluirMatriculaResponse>>, ConcluirMatriculaCommandHandler>();
            services.AddScoped<IRequestHandler<IniciarAulaCommand, Result<ProgressoAulaResponse>>, IniciarAulaCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirAulaCommand, Result<ProgressoAulaResponse>>, ConcluirAulaCommandHandler>();
            services.AddScoped<IRequestHandler<ObterCertificadosAlunoQuery, Result<IEnumerable<CertificadoAlunoResponse>>>, ObterCertificadosAlunoQueryHandler>();

            // Application services
            services.AddScoped<IAlunoService, AlunoService>();

            return services;
        }
    }
}