using Peo.Core.Interfaces.Data;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Domain.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<Aluno?> GetByUserIdAsync(Guid usuarioId, CancellationToken cancellationToken);

    Task<Aluno?> GetByIdAsync(Guid alunoId, CancellationToken cancellationToken);

    Task AddAsync(Aluno aluno, CancellationToken cancellationToken);

    Task AddMatriculaAsync(Matricula matricula, CancellationToken cancellationToken);

    Task<Matricula?> GetMatriculaByIdAsync(Guid matriculaId, CancellationToken cancellationToken);

    Task AtualizarMatricula(Matricula matricula, CancellationToken cancellationToken);

    Task AddProgressoMatriculaAsync(ProgressoMatricula progresso, CancellationToken cancellationToken);

    Task<ProgressoMatricula?> GetProgressoMatriculaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken);

    Task AtualizarProgressoMatriculaAsync(ProgressoMatricula progresso, CancellationToken cancellationToken);

    Task<int> CountAulasConcluidasAsync(Guid matriculaId, CancellationToken cancellationToken);

    Task AddCertificadoAsync(Certificado certificado, CancellationToken cancellationToken);

    Task<IEnumerable<Certificado>> GetCertificadosByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken);

    Task<IEnumerable<Matricula>> GetMatriculasByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken);

    Task<IEnumerable<Matricula>> GetMatriculasConcluidaByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken);
}