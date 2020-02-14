namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using Interfaces;

    public class UcSaveGraphResponse : UseCaseResponseMessage
    {
        public string Id { get; set; }

        public UcSaveGraphResponse(string id, bool success = false, string message = null) : base(success, message)
        {
            this.Id = id;
        }
    }
}