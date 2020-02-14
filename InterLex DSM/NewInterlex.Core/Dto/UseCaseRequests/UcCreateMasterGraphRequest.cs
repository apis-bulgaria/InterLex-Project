namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using Enumerations;
    using Interfaces;
    using UseCaseResponses;

    public class UcCreateMasterGraphRequest : IUseCaseRequest<UcCreateMasterGraphResponse>
    {
        public UcCreateMasterGraphRequest(string title, int order, MasterGraphCategories masterGraphCategory)
        {
            this.Title = title;
            this.Order = order;
            this.MasterGraphCategory = masterGraphCategory;
        }

        public string Title { get; set; }

        public int Order { get; set; }

        public MasterGraphCategories MasterGraphCategory { get; set; }
    }
}