namespace Interlex.Data
{
    using System;

    public class ExpertMaterialsLog
    {
        public int Id { get; set; }
        public int ExpertMaterialId { get; set; }
        public string UserId { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Content { get; set; }

        public virtual ExpertMaterial ExpertMaterial { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}