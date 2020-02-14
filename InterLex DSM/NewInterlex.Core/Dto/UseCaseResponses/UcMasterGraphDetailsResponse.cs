namespace NewInterlex.Core.Dto.UseCaseResponses
{
    using System.Collections.Generic;
    using Interfaces;

    public class UcMasterGraphDetailsResponse : UseCaseResponseMessage
    {
        public string Title { get; set; }

        public IEnumerable<GraphListModel> Graphs { get; set; }

        public UcMasterGraphDetailsResponse(string title, IEnumerable<GraphListModel> graphs, bool success = false, string message = null) : base(success, message)
        {
            this.Title = title;
            this.Graphs = graphs;
        }
    }
}