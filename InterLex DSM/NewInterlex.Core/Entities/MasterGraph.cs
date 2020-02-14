namespace NewInterlex.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using Dto.UseCaseRequests;
    using Shared;

    public class MasterGraph : BaseGuidEntity
    {
        public MasterGraph()
        {
            Graphs = new HashSet<Graph>();
        }

        public bool? IsActive { get; set; }
        public int MasterGraphCategoryId { get; set; }
        public int PropsId { get; set; }
        
        public int Order { get; set; }

        public virtual MasterGraphCategory MasterGraphCategory { get; set; }
        public virtual CommonGraphProp Props { get; set; }
        public virtual ICollection<Graph> Graphs { get; set; }

        public Guid AddGraph(UcSaveGraphRequest message)
        {
            var graph = new Graph
            {
                Data = message.GraphContent,
                Title = message.Title,
                MasterGraphId = message.MasterGuid,
                GraphTypeId = message.GraphType, // check this?
                Id = Guid.NewGuid()
            };
            this.Graphs.Add(graph);
            return graph.Id;
        }
    }
}