namespace Peo.Core.Interfaces.Services.Acls;

public interface ICursoAulaService
{
    Task<int> ObterTotalAulasDoCursoAsync(Guid cursoId);

    Task<string?> ObterTituloCursoAsync(Guid cursoId);

    Task<bool> ValidarSeCursoExisteAsync(Guid cursoId);

    Task<decimal> ObterPrecoCursoAsync(Guid cursoId);
}