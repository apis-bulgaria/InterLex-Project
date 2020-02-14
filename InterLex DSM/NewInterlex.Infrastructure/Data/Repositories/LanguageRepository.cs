namespace NewInterlex.Infrastructure.Data.Repositories
{
    using Core.Entities;
    using Core.Interfaces.Gateways.Repositories;

    internal sealed class LanguageRepository : EfRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}