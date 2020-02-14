namespace Interlex.Models.RequestModels
{
    using System.Collections.Generic;

    public class ExpertModel : CaseModel
    {
        public List<FileModel> Files { get; set; }
    }
}