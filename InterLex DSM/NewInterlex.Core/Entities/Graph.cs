namespace NewInterlex.Core.Entities
{
    using System;
    using Shared;

    public class Graph : BaseGuidEntity
    {
        public bool? IsActive { get; set; }
        public int GraphTypeId { get; set; }
        public Guid MasterGraphId { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }

        public virtual GraphType GraphType { get; set; }
        public virtual MasterGraph MasterGraph { get; set; }
    }
}