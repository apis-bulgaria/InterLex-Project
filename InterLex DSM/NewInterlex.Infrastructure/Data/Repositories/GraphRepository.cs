namespace NewInterlex.Infrastructure.Data.Repositories
{
    using Core.Entities;
    using Core.Interfaces.Gateways.Repositories;

    public class GraphRepository : GuidEfRepository<Graph>, IGraphRepository
    {
        public GraphRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}