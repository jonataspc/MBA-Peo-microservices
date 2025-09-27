using MassTransit;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Application.Services;

public class AlunoService(
    IRequestClient<ObterDetalhesCursoRequest> requestClientObterDetalhesCurso,
    IAlunoRepository alunoRepository,
    IRequestClient<ObterDetalhesUsuarioRequest> requestClientObterDetalhesUsuario,
    IAppIdentityUser appIdentityUser) : IAlunoService
{
    public async Task<Aluno> CriarAlunoAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var aluno = new Aluno(usuarioId);
        await alunoRepository.AddAsync(aluno, cancellationToken);
        await alunoRepository.UnitOfWork.CommitAsync(cancellationToken);
        return aluno;
    }

    public async Task<Matricula> MatricularAlunoAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        var aluno = await alunoRepository.GetByIdAsync(alunoId, cancellationToken);
        if (aluno == null)
            throw new ArgumentException("Aluno não encontrado", nameof(alunoId));

        var responseCurso = await requestClientObterDetalhesCurso.GetResponse<ObterDetalhesCursoResponse>(new ObterDetalhesCursoRequest(cursoId));

        if (responseCurso.Message == null || responseCurso.Message.CursoId == null)
            throw new ArgumentException("Curso não encontrado ou sem preço definido", nameof(cursoId));

        var cursoJaMatriculado = await alunoRepository.AnyAsync(s => s.Id == alunoId && s.Matriculas.Any(e => e.CursoId == cursoId), cancellationToken);

        if (cursoJaMatriculado)
        {
            throw new ArgumentException("Aluno já está matriculado neste curso");
        }

        var matricula = new Matricula(alunoId, cursoId);
        await alunoRepository.AddMatriculaAsync(matricula, cancellationToken);
        await alunoRepository.UnitOfWork.CommitAsync(cancellationToken);
        return matricula;
    }

    public async Task<Matricula> MatricularAlunoComUserIdAsync(Guid usuarioId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        Aluno aluno = await ObterAlunoPorUserIdAsync(usuarioId, cancellationToken);

        return await MatricularAlunoAsync(aluno.Id, cursoId, cancellationToken);
    }

    public async Task<Aluno> ObterAlunoPorUserIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var aluno = await alunoRepository.GetByUserIdAsync(usuarioId, cancellationToken);

        aluno ??= await CriarAlunoAsync(usuarioId, cancellationToken);
        return aluno;
    }

    public async Task<ProgressoMatricula> IniciarAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default)
    {
        var matricula = await alunoRepository.GetMatriculaByIdAsync(matriculaId, cancellationToken)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarAlunoEhUsuarioLogado(matricula, cancellationToken);

        if (matricula.Status != StatusMatricula.Ativo)
            throw new InvalidOperationException("Não é possível iniciar aula para matrícula inativa");

        var progressoExistente = await alunoRepository.GetProgressoMatriculaAsync(matriculaId, aulaId, cancellationToken);
        if (progressoExistente != null)
            throw new InvalidOperationException("Aula já iniciada");

        var progresso = new ProgressoMatricula(matriculaId, aulaId);
        await alunoRepository.AddProgressoMatriculaAsync(progresso, cancellationToken);
        await alunoRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progresso;
    }

    private async Task ValidarAlunoEhUsuarioLogado(Matricula matricula, CancellationToken cancellationToken)
    {
        var aluno = await ObterAlunoPorUserIdAsync(appIdentityUser.GetUserId(), cancellationToken);

        if (aluno.Id != matricula.AlunoId)
        {
            throw new DomainException("Violação de isolamento detectada (usuário atual não é o aluno da matrícula)");
        }
    }

    public async Task<ProgressoMatricula> ConcluirAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default)
    {
        var matricula = await alunoRepository.GetMatriculaByIdAsync(matriculaId, cancellationToken)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarAlunoEhUsuarioLogado(matricula, cancellationToken);

        var progresso = await alunoRepository.GetProgressoMatriculaAsync(matriculaId, aulaId, cancellationToken)
            ?? throw new ArgumentException("Aula não iniciada", nameof(aulaId));

        if (progresso.EstaConcluido)
            throw new InvalidOperationException("Aula já concluída");

        // Marcar aula como concluída
        progresso.MarcarComoConcluido();
        await alunoRepository.AtualizarProgressoMatriculaAsync(progresso, cancellationToken);

        // Calcular e atualizar progresso geral
        var responseCurso = await requestClientObterDetalhesCurso.GetResponse<ObterDetalhesCursoResponse>(new ObterDetalhesCursoRequest(matricula.CursoId));

        if (responseCurso.Message == null || responseCurso.Message.CursoId == null)
            throw new ArgumentException("Curso não encontrado", nameof(matricula.CursoId));

        if (responseCurso.Message.TotalAulas == null)
            throw new ArgumentException("Total de aulas do curso não informado", nameof(matricula.CursoId));
        var totalAulas = responseCurso.Message.TotalAulas.Value;
        var aulasConcluidas = await alunoRepository.GetAulasConcluidasCountAsync(matriculaId, cancellationToken);

        var novoPercentualProgresso = (int)(aulasConcluidas * 100.0 / totalAulas);
        matricula.AtualizarProgresso(novoPercentualProgresso);

        // Se todas as aulas foram concluídas, marcar matrícula como concluída
        if (novoPercentualProgresso == 100)
        {
            matricula.Concluir();
        }

        await alunoRepository.AtualizarMatricula(matricula, cancellationToken);
        await alunoRepository.UnitOfWork.CommitAsync(cancellationToken);

        return progresso;
    }

    public async Task<int> ObterProgressoGeralCursoAsync(Guid matriculaId, CancellationToken cancellationToken = default)
    {
        var matricula = await alunoRepository.GetMatriculaByIdAsync(matriculaId, cancellationToken)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarAlunoEhUsuarioLogado(matricula, cancellationToken);

        return matricula.PercentualProgresso;
    }

    public async Task<Matricula> ConcluirMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken = default)
    {
        var matricula = await alunoRepository.GetMatriculaByIdAsync(matriculaId, cancellationToken)
            ?? throw new ArgumentException("Matrícula não encontrada", nameof(matriculaId));

        await ValidarAlunoEhUsuarioLogado(matricula, cancellationToken);

        if (matricula.Status != StatusMatricula.Ativo)
            throw new InvalidOperationException($"Não é possível concluir matrícula no status {matricula.Status}");

        // Verificar se todas as aulas foram concluídas
        var responseCurso = await requestClientObterDetalhesCurso.GetResponse<ObterDetalhesCursoResponse>(new ObterDetalhesCursoRequest(matricula.CursoId));

        if (responseCurso.Message == null || responseCurso.Message.CursoId == null)
            throw new ArgumentException("Curso não encontrado", nameof(matricula.CursoId));

        var totalAulas = responseCurso.Message.TotalAulas;
        var aulasConcluidas = await alunoRepository.GetAulasConcluidasCountAsync(matriculaId, cancellationToken);

        if (aulasConcluidas < totalAulas)
            throw new InvalidOperationException($"Não é possível concluir matrícula. {aulasConcluidas} de {totalAulas} aulas concluídas.");

        matricula.Concluir();
        await alunoRepository.AtualizarMatricula(matricula, cancellationToken);

        var numeroCertificado = GerarNumeroCertificado();

        var certificado = new Certificado(
            matriculaId: matricula.Id,
            conteudo: await GerarConteudoCertificadoAsync(matricula, numeroCertificado),
            dataEmissao: DateTime.Now,
            numeroCertificado: numeroCertificado
        );
        await alunoRepository.AddCertificadoAsync(certificado, cancellationToken);

        await alunoRepository.UnitOfWork.CommitAsync(cancellationToken);

        return matricula;
    }

    private async Task<string> GerarConteudoCertificadoAsync(Matricula matricula, string numeroCertificado)
    {
        var msg = await requestClientObterDetalhesUsuario.GetResponse<ObterDetalhesUsuarioResponse>(new ObterDetalhesUsuarioRequest(matricula.Aluno!.UsuarioId));

        var nomeUsuario = msg?.Message?.Nome ?? throw new ArgumentException("Usuário não encontrado", nameof(matricula.AlunoId));

        var responseCurso = await requestClientObterDetalhesCurso.GetResponse<ObterDetalhesCursoResponse>(new ObterDetalhesCursoRequest(matricula.CursoId));

        if (responseCurso.Message == null || responseCurso.Message.CursoId == null)
            throw new ArgumentException("Curso não encontrado", nameof(matricula.CursoId));

        var tituloCurso = responseCurso.Message.Titulo!;

        return $"Certificado de Conclusão\nMatrícula: {matricula.Id}\nEmitido em: {DateTime.Now:yyyy-MM-dd}\nNúmero: {numeroCertificado}\nCurso: {tituloCurso}\nNome do aluno: {nomeUsuario}";
    }

    private static string GerarNumeroCertificado()
    {
        return $"CERT-{DateTime.Now:yyyyMMddHHmmss}-{Guid.CreateVersion7().ToString("N").Substring(0, 8)}";
    }

    public async Task<IEnumerable<Certificado>> ObterCertificadosDoAlunoAsync(Guid alunoId, CancellationToken cancellationToken = default)
    {
        var aluno = await alunoRepository.GetByIdAsync(alunoId, cancellationToken);
        if (aluno == null)
            throw new ArgumentException("Aluno não encontrado", nameof(alunoId));

        var certificados = await alunoRepository.GetCertificadosByAlunoIdAsync(alunoId, cancellationToken);
        return certificados;
    }

    public async Task<IEnumerable<Matricula>> ObterMatriculas(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var aluno = await ObterAlunoPorUserIdAsync(usuarioId, cancellationToken);

        var matriculas = await alunoRepository.GetMatriculasByAlunoIdAsync(aluno.Id, cancellationToken);

        return matriculas;
    }
}