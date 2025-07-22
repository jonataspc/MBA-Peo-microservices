using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Repositories;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Infra.Data.Contexts;

namespace Peo.GestaoAlunos.Infra.Data.Repositories;

public class EstudanteRepository : GenericRepository<Estudante, GestaoEstudantesContext>, IEstudanteRepository
{
    public EstudanteRepository(GestaoEstudantesContext context) : base(context)
    {
    }

    public async Task<Estudante?> GetByIdAsync(Guid estudanteId)
    {
        return await _dbContext.Estudantes.FirstOrDefaultAsync(e => e.Id == estudanteId);
    }

    public async Task AddAsync(Estudante estudante)
    {
        await AddRangeAsync(new[] { estudante });
    }

    public async Task AddMatriculaAsync(Matricula matricula)
    {
        await _dbContext.Matriculas.AddAsync(matricula);
    }

    public async Task<Matricula?> GetMatriculaByIdAsync(Guid matriculaId)
    {
        return await _dbContext.Matriculas
            .Include(m => m.Estudante)
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

    public async Task<Estudante?> GetByUserIdAsync(Guid usuarioId)
    {
        return await _dbContext.Estudantes.Where(s => s.UsuarioId == usuarioId)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Certificado>> GetCertificadosByEstudanteIdAsync(Guid estudanteId)
    {
        var matriculas = await _dbContext.Matriculas.Where(m => m.EstudanteId == estudanteId).ToListAsync();
        var matriculaIds = matriculas.Select(m => m.Id).ToList();
        return await _dbContext.Certificados.Where(c => matriculaIds.Contains(c.MatriculaId)).ToListAsync();
    }
}