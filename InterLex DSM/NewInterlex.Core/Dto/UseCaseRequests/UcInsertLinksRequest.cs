namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using System;
    using Interfaces;
    using UseCaseResponses;

    public class UcInsertLinksRequest : IUseCaseRequest<UcInsertLinksResponse>
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
        

        public UcInsertLinksRequest(Guid id, string content)
        {
            this.Id = id;
            this.Content = content;
        }
    }
}