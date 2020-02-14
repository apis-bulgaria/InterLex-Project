namespace NewInterlex.Core.Interfaces.Gateways.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Shared;

    public interface IGuidRepository<T> : IBaseRepository<T> where T : BaseGuidEntity
    {
        Task<T> GetById(Guid id);

    }
}