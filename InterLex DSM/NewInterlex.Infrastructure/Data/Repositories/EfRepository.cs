namespace NewInterlex.Infrastructure.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Core.Interfaces.Gateways.Repositories;
    using Core.Shared;
    using Microsoft.EntityFrameworkCore;

    public abstract class EfRepository<T> : BaseEfRepository<T>, IRepository<T> where T : BaseIdEntity
    {
        protected EfRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<T> GetById(int id)
        {
            return await this.dbContext.FindAsync<T>(id);
        }
    }
}