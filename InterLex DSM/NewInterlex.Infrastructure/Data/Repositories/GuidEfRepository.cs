namespace NewInterlex.Infrastructure.Data.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Core.Interfaces.Gateways.Repositories;
    using Core.Shared;

    public class GuidEfRepository<T> : BaseEfRepository<T>, IGuidRepository<T> where T: BaseGuidEntity
    {
        public GuidEfRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<T> GetById(Guid id)
        {
            return await this.dbContext.FindAsync<T>(id);

        }
    }
}