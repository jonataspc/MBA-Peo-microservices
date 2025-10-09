using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Domain.Services
{
    public interface IAlunoService
    {
        Task<Aluno> CriarAlunoAsync(Guid usuarioId, CancellationToken cancellationToken = default);

        Task<Matricula> MatricularAlunoAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default);

        Task<ProgressoMatricula> IniciarAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default);

        Task<ProgressoMatricula> ConcluirAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default);

        Task<Matricula> MatricularAlunoComUserIdAsync(Guid usuarioId, Guid cursoId, CancellationToken cancellationToken = default);

        Task<int> ObterProgressoGeralCursoAsync(Guid matriculaId, CancellationToken cancellationToken = default);

        Task<Matricula> ConcluirMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Certificado>> ObterCertificadosDoAlunoAsync(Guid alunoId, CancellationToken cancellationToken = default);

        Task<Aluno> ObterAlunoPorUserIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Matricula>> ObterMatriculas(Guid alunoId, bool apenasConcluidas, CancellationToken cancellationToken = default);

    }
}