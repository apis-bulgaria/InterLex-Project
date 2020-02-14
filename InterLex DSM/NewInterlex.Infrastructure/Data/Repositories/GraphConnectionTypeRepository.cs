namespace NewInterlex.Infrastructure.Data.Repositories
{
    using Core.Entities;
    using Core.Interfaces.Gateways.Repositories;

    internal sealed class GraphConnectionTypeRepository : EfRepository<GraphConnectionType>, IGraphConnectionTypeRepository
    {
        public GraphConnectionTypeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}