namespace Interlex.Models.ResponseModels
{
    using System;

    public class FileResponseModel
    {
        public Guid Id { get; set; }

        public string Filename { get; set; }

        public LanguageModel Language { get; set; }
    }
}