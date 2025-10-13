using Peo.Core.DomainObjects;
using System.Linq.Expressions;

namespace Peo.Core.Interfaces.Data
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        void Insert(T entity, CancellationToken cancellationToken);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        void Update(T entity, CancellationToken cancellationToken);

        IRepository<T> WithTracking();

        IRepository<T> WithoutTracking();
    }
}