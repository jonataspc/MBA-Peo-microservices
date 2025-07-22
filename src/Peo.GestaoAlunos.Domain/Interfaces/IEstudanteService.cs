using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Domain.Interfaces
{
    public interface IEstudanteService
    {
        Task<Estudante> CriarEstudanteAsync(Guid usuarioId, CancellationToken cancellationToken = default);

        Task<Matricula> MatricularEstudanteAsync(Guid estudanteId, Guid cursoId, CancellationToken cancellationToken = default);

        Task<ProgressoMatricula> IniciarAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default);

        Task<ProgressoMatricula> ConcluirAulaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken = default);

        Task<Matricula> MatricularEstudanteComUserIdAsync(Guid usuarioId, Guid cursoId, CancellationToken cancellationToken = default);

        Task<int> ObterProgressoGeralCursoAsync(Guid matriculaId, CancellationToken cancellationToken = default);

        Task<Matricula> ConcluirMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Certificado>> ObterCertificadosDoEstudanteAsync(Guid estudanteId, CancellationToken cancellationToken = default);

        Task<Estudante> ObterEstudantePorUserIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    }
}