namespace NewInterlex.Infrastructure.Data.Repositories
{
    using Core.Entities;
    using Core.Interfaces.Gateways.Repositories;

    internal sealed class LinkTypeRepository : EfRepository<LinkType>, ILinkTypeRepository
    {
        public LinkTypeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}