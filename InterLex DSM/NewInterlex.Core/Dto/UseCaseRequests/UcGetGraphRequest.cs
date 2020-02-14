namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using System;
    using Interfaces;
    using UseCaseResponses;

    public class UcGetGraphRequest : IUseCaseRequest<UcGetGraphResponse>
    {
        public Guid GraphId { get; set; }

        public UcGetGraphRequest(Guid graphId)
        {
            this.GraphId = graphId;
        }
    }
}