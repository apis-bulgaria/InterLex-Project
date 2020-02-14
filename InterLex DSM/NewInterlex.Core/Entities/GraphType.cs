namespace NewInterlex.Core.Entities
{
    using System.Collections.Generic;
    using Shared;

    public partial class GraphType : BaseIdEntity
    {
        public GraphType()
        {
            Graphs = new HashSet<Graph>();
        }

        public string Name { get; set; }

        public virtual ICollection<Graph> Graphs { get; set; }
    }
}