namespace Peo.Core.Interfaces.Data
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}