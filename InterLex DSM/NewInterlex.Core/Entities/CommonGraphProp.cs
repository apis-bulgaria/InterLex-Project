namespace NewInterlex.Core.Entities
{
    using System.Collections.Generic;
    using Shared;

    public class CommonGraphProp : BaseIdEntity
    {
        public CommonGraphProp()
        {
            this.MasterGraph = new HashSet<MasterGraph>();
        }

        public string EnText { get; set; }

        public virtual ICollection<MasterGraph> MasterGraph { get; set; }
    }
}