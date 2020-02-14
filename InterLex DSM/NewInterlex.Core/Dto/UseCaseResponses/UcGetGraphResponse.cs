namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using Interfaces;

    public class UcGetGraphResponse : UseCaseResponseMessage
    {
        public string Title { get; set; }

        public string Data { get; set; }

        public UcGetGraphResponse(string title, string data, bool success = false, string message = null) : base(success, message)
        {
            this.Title = title;
            this.Data = data;
        }

        public UcGetGraphResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}