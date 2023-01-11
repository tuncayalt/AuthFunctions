using AuthFunctions.Data.Contexts;
using AuthFunctions.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthFunctions.Data.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly AuthDbContext _dbContext;

        public EfRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await _dbContext.AddAsync(entity);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }
            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }
            return await query.Where(predicate).ToListAsync();
        }
    }
}
