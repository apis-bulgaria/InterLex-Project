namespace Interlex.Models.RequestModels
{
    using System;
    using ResponseModels;

    public class FileModel
    {
        public Guid Id { get; set; }
        public string Base64Content { get; set; }

        public string Filename { get; set; }

        public string MimeType { get; set; }

        public LanguageModel Language { get; set; }
    }
}