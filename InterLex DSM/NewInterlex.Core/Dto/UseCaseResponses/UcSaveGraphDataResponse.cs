namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using Interfaces;

    public class UcSaveGraphDataResponse : UseCaseResponseMessage
    {
        public UcSaveGraphDataResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}