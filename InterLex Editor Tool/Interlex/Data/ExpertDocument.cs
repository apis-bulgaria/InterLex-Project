namespace Interlex.Data
{
    using System;

    public class ExpertDocument
    {
        public Guid Id { get; set; }
        public int? ExpertMaterialId { get; set; }
        public byte[] Content { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public int LanguageId { get; set; }

        public virtual ExpertMaterial ExpertMaterial { get; set; }
        public virtual Language Language { get; set; }
    }
}