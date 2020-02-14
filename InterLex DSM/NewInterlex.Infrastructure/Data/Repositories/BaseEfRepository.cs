namespace NewInterlex.Infrastructure.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Core.Interfaces.Gateways.Repositories;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseEfRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext dbContext;

        protected BaseEfRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAll()
        {
            return await this.dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetSingleBySpec(Expression<Func<T, bool>> spec)
        {
            return await this.dbContext.Set<T>().Where(spec).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetListBySpec(Expression<Func<T, bool>> spec)
        {
            return await this.dbContext.Set<T>().Where(spec).AsNoTracking().ToListAsync();
        }

        public async Task<T> Add(T entity)
        {
            this.dbContext.Set<T>().Add(entity);
            await this.dbContext.SaveInfoAndChangesAsync();
            return entity;
        }

        public async Task Update(T entity)
        {
            this.dbContext.Set<T>().Update(entity);
            await this.dbContext.SaveInfoAndChangesAsync();
        }

        public async Task Delete(T entity)
        {
            this.dbContext.Set<T>().Remove(entity);
            await this.dbContext.SaveInfoAndChangesAsync();
        }
    }
}