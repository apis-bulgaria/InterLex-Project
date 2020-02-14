namespace NewInterlex.Core.Interfaces.Gateways.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dto;
    using Entities;
    using Enumerations;

    public interface IMasterGraphRepository : IGuidRepository<MasterGraph>
    {
        Task<MasterGraph> Create(string messageTitle, int messageOrder,
            MasterGraphCategories messageMasterGraphCategory);

        Task<MasterGraphDetails> GetDetailInfo(Guid messageId);
        Task<IReadOnlyList<MasterGraphListModel>> GetListWithNames();
    }
}