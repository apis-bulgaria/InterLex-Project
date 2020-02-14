namespace NewInterlex.Core.Entities
{
    using System.Collections.Generic;
    using Shared;

    public class MasterGraphCategory : BaseIdEntity
    {
        public MasterGraphCategory()
        {
            MasterGraph = new HashSet<MasterGraph>();
        }

        public string Category { get; set; }

        public virtual ICollection<MasterGraph> MasterGraph { get; set; }
    }
}