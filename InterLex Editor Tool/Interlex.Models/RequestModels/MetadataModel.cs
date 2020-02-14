namespace Interlex.Models.RequestModels
{
    public class MetadataModel : CaseModel
    {
        public FileModel File { get; set; }

        public FileModel TranslatedFile { get; set; }
    }

    
}