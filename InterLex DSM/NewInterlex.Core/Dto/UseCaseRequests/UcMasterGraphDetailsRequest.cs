namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using Interfaces;
    using UseCaseResponses;

    public class UcMasterGraphDetailsRequest : IUseCaseRequest<UcMasterGraphDetailsResponse>
    {
        public string Id { get; set; }

        public UcMasterGraphDetailsRequest(string id)
        {
            this.Id = id;
        }
    }
}