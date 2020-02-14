namespace NewInterlex.Core.Dto
{
    using System;
    using System.Collections.Generic;
    using Enumerations;

    public class MasterGraphDetails
    {
        public string Title { get; set; }

        public IEnumerable<GraphListModel> Graphs { get; set; }
    }
    
    
    public class GraphListModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public GraphTypes Type { get; set; }
        
//        Countries??
    }
}