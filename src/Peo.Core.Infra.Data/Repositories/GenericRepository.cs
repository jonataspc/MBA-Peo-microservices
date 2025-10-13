using Microsoft.EntityFrameworkCore;
using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;
using Peo.Core.Interfaces.Data;
using System.Linq.Expressions;

namespace Peo.Core.Infra.Data.Repositories
{
    public class GenericRepository<TEntity, TContext> : IRepository<TEntity>
       where TEntity : EntityBase, IAggregateRoot
       where TContext : DbContext
    {
        // Default behavior: tracking is disabled
        protected bool _isTracking = false;

        public IUnitOfWork UnitOfWork => (IUnitOfWork)_dbContext;

        protected readonly TContext _dbContext;

        public GenericRepository(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<TEntity> WithTracking()
        {
            _isTracking = true;
            return this;
        }

        public IRepository<TEntity> WithoutTracking()
        {
            _isTracking = false;
            return this;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await _dbContext.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var dbSet = _dbContext.Set<TEntity>();

            if (!_isTracking)
            {
                return await dbSet.AsNoTracking()
                                  .Where(x => x.Id == id)
                                  .FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                return await dbSet.FindAsync(id, cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<IEnumerable<TEntity>?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void Insert(TEntity entity, CancellationToken cancellationToken)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity, CancellationToken cancellationToken)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .AnyAsync(predicate, cancellationToken)
                                   .ConfigureAwait(false);
        }
    }
}