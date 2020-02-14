namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using Interfaces;

    public class UcCreateMasterGraphResponse : UseCaseResponseMessage
    {
        public string Id { get; set; }

        public UcCreateMasterGraphResponse(string id, bool success = false, string message = null) : base(success, message)
        {
            this.Id = id;
        }
    }
}