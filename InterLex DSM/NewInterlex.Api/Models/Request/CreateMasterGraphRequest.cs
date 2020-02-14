namespace NewInterlex.Api.Models.Request
{
    using Core.Enumerations;

    public class CreateMasterGraphRequest
    {
        public string Title { get; set; }

        public int Order { get; set; }

        public MasterGraphCategories MasterGraphCategory { get; set; }
    }
}