using Peo.Core.Interfaces.Data;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Domain.Interfaces;

public interface IEstudanteRepository : IRepository<Estudante>
{
    Task<Estudante?> GetByUserIdAsync(Guid usuarioId);

    Task<Estudante?> GetByIdAsync(Guid estudanteId);

    Task AddAsync(Estudante estudante);

    Task AddMatriculaAsync(Matricula matricula);

    Task<Matricula?> GetMatriculaByIdAsync(Guid matriculaId);

    Task AtualizarMatricula(Matricula matricula);

    Task AddProgressoMatriculaAsync(ProgressoMatricula progresso);

    Task<ProgressoMatricula?> GetProgressoMatriculaAsync(Guid matriculaId, Guid aulaId);

    Task AtualizarProgressoMatriculaAsync(ProgressoMatricula progresso);

    Task<int> GetAulasConcluidasCountAsync(Guid matriculaId);

    Task AddCertificadoAsync(Certificado certificado);

    Task<IEnumerable<Certificado>> GetCertificadosByEstudanteIdAsync(Guid estudanteId);

    Task<IEnumerable<Matricula>> GetMatriculasByEstudanteIdAsync(Guid estudanteId);
}