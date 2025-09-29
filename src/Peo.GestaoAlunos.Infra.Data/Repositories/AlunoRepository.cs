using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Repositories;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Infra.Data.Contexts;

namespace Peo.GestaoAlunos.Infra.Data.Repositories;

public class AlunoRepository : GenericRepository<Aluno, GestaoAlunosContext>, IAlunoRepository
{
    public AlunoRepository(GestaoAlunosContext context) : base(context)
    {
    }

    public async Task<Aluno?> GetByIdAsync(Guid alunoId, CancellationToken cancellationToken)
    {
        return await _dbContext.Alunos.FirstOrDefaultAsync(e => e.Id == alunoId, cancellationToken);
    }

    public async Task AddAsync(Aluno aluno, CancellationToken cancellationToken)
    {
        await AddRangeAsync(new[] { aluno }, cancellationToken);
    }

    public async Task AddMatriculaAsync(Matricula matricula, CancellationToken cancellationToken)
    {
        await _dbContext.Matriculas.AddAsync(matricula, cancellationToken);
    }

    public async Task<Matricula?> GetMatriculaByIdAsync(Guid matriculaId, CancellationToken cancellationToken)
    {
        return await _dbContext.Matriculas
            .Include(m => m.Aluno)
            .FirstOrDefaultAsync(m => m.Id == matriculaId, cancellationToken);
    }

    public Task AtualizarMatricula(Matricula matricula, CancellationToken cancellationToken)
    {
        _dbContext.Matriculas.Update(matricula);
        return Task.CompletedTask;
    }

    public async Task AddProgressoMatriculaAsync(ProgressoMatricula progresso, CancellationToken cancellationToken)
    {
        await _dbContext.ProgressosMatricula.AddAsync(progresso, cancellationToken);
    }

    public async Task<ProgressoMatricula?> GetProgressoMatriculaAsync(Guid matriculaId, Guid aulaId, CancellationToken cancellationToken)
    {
        return await _dbContext.ProgressosMatricula
            .FirstOrDefaultAsync(p => p.MatriculaId == matriculaId && p.AulaId == aulaId, cancellationToken);
    }

    public Task AtualizarProgressoMatriculaAsync(ProgressoMatricula progresso, CancellationToken cancellationToken)
    {
        _dbContext.ProgressosMatricula.Update(progresso);
        return Task.CompletedTask;
    }

    public async Task<int> CountAulasConcluidasAsync(Guid matriculaId, CancellationToken cancellationToken)
    {
        return await _dbContext.ProgressosMatricula
            .CountAsync(p => p.MatriculaId == matriculaId && p.DataConclusao.HasValue, cancellationToken);
    }

    public async Task AddCertificadoAsync(Certificado certificado, CancellationToken cancellationToken)
    {
        await _dbContext.Certificados.AddAsync(certificado, cancellationToken);
    }

    public async Task<Aluno?> GetByUserIdAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        return await _dbContext.Alunos.Where(s => s.UsuarioId == usuarioId)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Certificado>> GetCertificadosByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken)
    {
        var matriculas = await _dbContext.Matriculas.Where(m => m.AlunoId == alunoId).ToListAsync(cancellationToken);
        var matriculaIds = matriculas.Select(m => m.Id).ToList();
        return await _dbContext.Certificados.Where(c => matriculaIds.Contains(c.MatriculaId)).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Matricula>> GetMatriculasByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken)
    {
        return await _dbContext.Matriculas
            .Where(m => m.AlunoId == alunoId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}