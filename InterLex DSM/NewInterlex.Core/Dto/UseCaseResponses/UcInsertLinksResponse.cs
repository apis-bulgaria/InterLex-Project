namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using Interfaces;

    public class UcInsertLinksResponse : UseCaseResponseMessage
    {
        public string ContentJson { get; set; }
        
        public UcInsertLinksResponse(string json, bool success = false, string message = null) : base(success, message)
        {
            this.ContentJson = json;
        }
    }
}