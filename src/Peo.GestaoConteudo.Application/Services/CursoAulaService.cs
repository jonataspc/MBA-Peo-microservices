using Peo.Core.Interfaces.Data;
using Peo.Core.Interfaces.Services.Acls;
using Peo.GestaoConteudo.Domain.Entities;

namespace Peo.GestaoConteudo.Application.Services;

public class CursoAulaService : ICursoAulaService
{
    private readonly IRepository<Curso> _cursoRepository;

    public CursoAulaService(IRepository<Curso> cursoRepository)
    {
        _cursoRepository = cursoRepository;
    }

    public async Task<decimal> ObterPrecoCursoAsync(Guid cursoId)
    {
        var curso = await _cursoRepository.GetAsync(cursoId);
        return curso?.Preco ?? 0;
    }

    public async Task<string?> ObterTituloCursoAsync(Guid cursoId)
    {
        var curso = await _cursoRepository.GetAsync(cursoId);
        return curso?.Titulo;
    }

    public async Task<int> ObterTotalAulasDoCursoAsync(Guid cursoId)
    {
        var curso = await _cursoRepository.GetAsync(cursoId);
        return curso?.Aulas.Count ?? 0;
    }

    public async Task<bool> ValidarSeCursoExisteAsync(Guid cursoId)
    {
        return await _cursoRepository.AnyAsync(c => c.Id == cursoId);
    }
}