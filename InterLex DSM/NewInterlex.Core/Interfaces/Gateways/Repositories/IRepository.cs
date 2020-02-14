namespace NewInterlex.Core.Interfaces.Gateways.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Shared;

    public interface IRepository<T>: IBaseRepository<T> where T : BaseIdEntity
    {
        Task<T> GetById(int id);
    }
}