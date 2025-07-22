using Peo.Core.DomainObjects;
using System.Linq.Expressions;

namespace Peo.Core.Interfaces.Data
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetAsync(Guid id);

        Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        void Insert(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        IRepository<T> WithTracking();

        IRepository<T> WithoutTracking();
    }
}