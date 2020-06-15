namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using Interfaces;

    public class UcExportReportResponse : UseCaseResponseMessage
    {
        public UcExportReportResponse(byte[] content, bool success = false, string message = null) : base(success, message)
        {
            this.FileContent = content;
        }

        public byte[] FileContent { get; set; }
    }
}