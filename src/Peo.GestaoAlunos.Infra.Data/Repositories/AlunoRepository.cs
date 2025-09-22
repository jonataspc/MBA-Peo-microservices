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

    public async Task<Aluno?> GetByIdAsync(Guid alunoId)
    {
        return await _dbContext.Alunos.FirstOrDefaultAsync(e => e.Id == alunoId);
    }

    public async Task AddAsync(Aluno aluno)
    {
        await AddRangeAsync(new[] { aluno });
    }

    public async Task AddMatriculaAsync(Matricula matricula)
    {
        await _dbContext.Matriculas.AddAsync(matricula);
    }

    public async Task<Matricula?> GetMatriculaByIdAsync(Guid matriculaId)
    {
        return await _dbContext.Matriculas
            .Include(m => m.Aluno)
            .FirstOrDefaultAsync(m => m.Id == matriculaId);
    }

    public Task AtualizarMatricula(Matricula matricula)
    {
        _dbContext.Matriculas.Update(matricula);
        return Task.CompletedTask;
    }

    public async Task AddProgressoMatriculaAsync(ProgressoMatricula progresso)
    {
        await _dbContext.ProgressosMatricula.AddAsync(progresso);
    }

    public async Task<ProgressoMatricula?> GetProgressoMatriculaAsync(Guid matriculaId, Guid aulaId)
    {
        return await _dbContext.ProgressosMatricula
            .FirstOrDefaultAsync(p => p.MatriculaId == matriculaId && p.AulaId == aulaId);
    }

    public Task AtualizarProgressoMatriculaAsync(ProgressoMatricula progresso)
    {
        _dbContext.ProgressosMatricula.Update(progresso);
        return Task.CompletedTask;
    }

    public async Task<int> GetAulasConcluidasCountAsync(Guid matriculaId)
    {
        return await _dbContext.ProgressosMatricula
            .CountAsync(p => p.MatriculaId == matriculaId && p.DataConclusao.HasValue);
    }

    public async Task AddCertificadoAsync(Certificado certificado)
    {
        await _dbContext.Certificados.AddAsync(certificado);
    }

    public async Task<Aluno?> GetByUserIdAsync(Guid usuarioId)
    {
        return await _dbContext.Alunos.Where(s => s.UsuarioId == usuarioId)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Certificado>> GetCertificadosByAlunoIdAsync(Guid alunoId)
    {
        var matriculas = await _dbContext.Matriculas.Where(m => m.AlunoId == alunoId).ToListAsync();
        var matriculaIds = matriculas.Select(m => m.Id).ToList();
        return await _dbContext.Certificados.Where(c => matriculaIds.Contains(c.MatriculaId)).ToListAsync();
    }

    public async Task<IEnumerable<Matricula>> GetMatriculasByAlunoIdAsync(Guid alunoId)
    {
        return await _dbContext.Matriculas
            .Where(m => m.AlunoId == alunoId)
            .AsNoTracking()
            .ToListAsync();
    }
}