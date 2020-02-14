namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using System.Collections.Generic;
    using Entities;
    using Interfaces;

    public class UcGetMetaInfoResponse : UseCaseResponseMessage
    {
        public UcGetMetaInfoResponse(IEnumerable<GraphConnectionType> graphConnectionTypes, IEnumerable<Language> languages, IEnumerable<LinkType> linkTypes, bool success = false, string message = null) : base(success, message)
        {
            this.GraphConnectionTypes = graphConnectionTypes;
            this.Languages = languages;
            this.LinkTypes = linkTypes;
        }
        
        public IEnumerable<GraphConnectionType> GraphConnectionTypes { get; set; }

        public IEnumerable<Language> Languages { get; set; }

        public IEnumerable<LinkType> LinkTypes { get; set; }
    }
}