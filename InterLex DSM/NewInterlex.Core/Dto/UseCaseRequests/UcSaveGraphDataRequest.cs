namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using System;
    using Interfaces;
    using UseCaseResponses;

    public class UcSaveGraphDataRequest : IUseCaseRequest<UcSaveGraphDataResponse>
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
        

        public UcSaveGraphDataRequest(Guid id, string content)
        {
            this.Id = id;
            this.Content = content;
        }
    }
}