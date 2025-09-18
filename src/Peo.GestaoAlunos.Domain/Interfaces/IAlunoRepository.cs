using Peo.Core.Interfaces.Data;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Domain.Interfaces;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<Aluno?> GetByUserIdAsync(Guid usuarioId);

    Task<Aluno?> GetByIdAsync(Guid alunoId);

    Task AddAsync(Aluno aluno);

    Task AddMatriculaAsync(Matricula matricula);

    Task<Matricula?> GetMatriculaByIdAsync(Guid matriculaId);

    Task AtualizarMatricula(Matricula matricula);

    Task AddProgressoMatriculaAsync(ProgressoMatricula progresso);

    Task<ProgressoMatricula?> GetProgressoMatriculaAsync(Guid matriculaId, Guid aulaId);

    Task AtualizarProgressoMatriculaAsync(ProgressoMatricula progresso);

    Task<int> GetAulasConcluidasCountAsync(Guid matriculaId);

    Task AddCertificadoAsync(Certificado certificado);

    Task<IEnumerable<Certificado>> GetCertificadosByAlunoIdAsync(Guid alunoId);

    Task<IEnumerable<Matricula>> GetMatriculasByAlunoIdAsync(Guid alunoId);
}