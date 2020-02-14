namespace Interlex.Data
{
    using System;
    using System.Collections.Generic;

    public class ExpertMaterial : IMetaCase
    {
        public ExpertMaterial()
        {
            ExpertMaterialsLog = new HashSet<ExpertMaterialsLog>();
            ExpertDocuments = new HashSet<ExpertDocument>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Caption { get; set; }
        public DateTime LastChange { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<ExpertMaterialsLog> ExpertMaterialsLog { get; set; }

        public virtual ICollection<ExpertDocument> ExpertDocuments { get; set; }

    }
}