namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using System.Collections.Generic;
    using Interfaces;

    public class UcMasterGraphGetAllResponse : UseCaseResponseMessage
    {
        public IEnumerable<MasterGraphListModel> MasterGraphs { get; set; }

        public UcMasterGraphGetAllResponse(IEnumerable<MasterGraphListModel> masterGraphs, bool success = false, string message = null) : base(success, message)
        {
            this.MasterGraphs = masterGraphs;
        }
    }
}