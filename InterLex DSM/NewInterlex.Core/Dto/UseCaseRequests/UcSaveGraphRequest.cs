namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using System;
    using Interfaces;
    using UseCaseResponses;

    public class UcSaveGraphRequest : IUseCaseRequest<UcSaveGraphResponse>
    {
        public UcSaveGraphRequest(string graphContent, string title, int graphType, Guid masterGuid)
        {
            this.GraphContent = graphContent;
            this.Title = title;
            this.GraphType = graphType;
            this.MasterGuid = masterGuid;
        }

        public string GraphContent { get; set; }

        public string Title { get; set; }

        public int GraphType { get; set; }

        public Guid MasterGuid { get; set; }
    }
}