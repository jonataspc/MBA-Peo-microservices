using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Interfaces.Data;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Infra.Data.Contexts;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Infra.Data.Contexts;

namespace Peo.Tests.IntegrationTests.Setup;

public class TestDatabaseSetup
{
    private readonly IEstudanteRepository _estudanteRepository;
    private readonly IRepository<Curso> _cursoRepository;

    private readonly IServiceScope _escopo;

    internal readonly string SenhaUsuarioTeste = "Test123!91726312389831625192JHTBADPDJANDHJPXASDO";
    internal readonly string EmailUsuarioTeste = $"{Guid.CreateVersion7()}@example.com";

    public TestDatabaseSetup(IServiceProvider serviceProvider)
    {
        _escopo = serviceProvider.CreateScope();

        _estudanteRepository = _escopo.ServiceProvider.GetRequiredService<IEstudanteRepository>();
        _cursoRepository = _escopo.ServiceProvider.GetRequiredService<IRepository<Curso>>();
    }

    public async Task<Estudante> CriarEstudanteTesteAsync(Guid usuarioId)
    {
        var userManager = _escopo.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var usuario = new Usuario(usuarioId, $"John Conor {Random.Shared.NextDouble()}", EmailUsuarioTeste);

        IUserRepository usuarioRepo = _escopo.ServiceProvider.GetRequiredService<IUserRepository>();
        usuarioRepo.Insert(usuario);
        await usuarioRepo.UnitOfWork.CommitAsync(default);

        // Adiciona usuário ao Identity
        var identityUser = new IdentityUser
        {
            Id = usuarioId.ToString(),
            UserName = usuario.Email,
            Email = usuario.Email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(identityUser, SenhaUsuarioTeste);
        await userManager.AddToRoleAsync(identityUser, AccessRoles.Aluno);

        var estudante = new Estudante(usuarioId);
        await _estudanteRepository.AddAsync(estudante);
        await _estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return estudante;
    }

    public async Task CriarUsuarioAdmin(Guid usuarioId)
    {
        var userManager = _escopo.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var usuario = new Usuario(usuarioId, $"Sara Conor {Random.Shared.NextDouble()}", EmailUsuarioTeste);

        IUserRepository usuarioRepo = _escopo.ServiceProvider.GetRequiredService<IUserRepository>();
        usuarioRepo.Insert(usuario);
        await usuarioRepo.UnitOfWork.CommitAsync(default);

        // Adiciona usuário ao Identity
        var identityUser = new IdentityUser
        {
            Id = usuarioId.ToString(),
            UserName = usuario.Email,
            Email = usuario.Email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(identityUser, SenhaUsuarioTeste);
        await userManager.AddToRoleAsync(identityUser, AccessRoles.Admin);
    }

    public async Task<Matricula> CriarMatriculaTesteAsync(Guid estudanteId, Guid cursoId, bool pagamentoRealizado)
    {
        var matricula = new Matricula(estudanteId, cursoId);

        if (pagamentoRealizado)
            matricula.ConfirmarPagamento();

        await _estudanteRepository.AddMatriculaAsync(matricula);
        await _estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return matricula;
    }

    public async Task<ProgressoMatricula> CriarProgressoAulaTesteAsync(Guid matriculaId, Guid aulaId)
    {
        var progresso = new ProgressoMatricula(matriculaId, aulaId);
        await _estudanteRepository.AddProgressoMatriculaAsync(progresso);
        await _estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return progresso;
    }

    public async Task<Certificado> CriarCertificadoTesteAsync(Guid matriculaId, string conteudo)
    {
        var certificado = new Certificado(matriculaId, conteudo, DateTime.UtcNow, $"CERT-{Guid.CreateVersion7():N}");
        await _estudanteRepository.AddCertificadoAsync(certificado);
        await _estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return certificado;
    }

    public async Task<Curso> CriarCursoTesteAsync(
        string titulo = "Curso de Teste",
        string descricao = "Descrição do Curso de Teste",
        Guid? instrutorId = null,
        decimal preco = 99.99m,
        bool publicado = true)
    {
        instrutorId ??= Guid.CreateVersion7();

        var curso = new Curso(
            titulo,
            descricao,
            instrutorId.Value,
            null, // Sem conteúdo de programa para teste
            preco,
            publicado,
            publicado ? DateTime.UtcNow : null,
            new List<string> { "teste", "integracao" },
        new List<Aula>
        {
                new Aula("", "", "", TimeSpan.FromSeconds(10), new List<ArquivoAula>
                {
                    new ArquivoAula( "", "", Guid.Empty),
                    new ArquivoAula( "", "", Guid.Empty),
                    new ArquivoAula( "", "", Guid.Empty)
                }, Guid.Empty),
                new Aula("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Aula("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Aula("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Aula("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty),
                new Aula("", "", "", TimeSpan.FromSeconds(10), default!, Guid.Empty)
            }
        );

        _cursoRepository.Insert(curso);
        await _cursoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return curso;
    }

    public async Task LimparAsync()
    {
        // Adicione lógica de limpeza aqui se necessário
        _escopo.Dispose();
        await Task.CompletedTask;
    }

    internal async Task InitializeAsync()
    {
        await _escopo.ServiceProvider.GetRequiredService<GestaoEstudantesContext>().Database.MigrateAsync();

        await _escopo.ServiceProvider.GetRequiredService<IdentityContext>().Database.MigrateAsync();
    }
}