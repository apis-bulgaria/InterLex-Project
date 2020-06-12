using Interlex.BusinessLayer.Entities;
using System.Collections.Generic;

namespace Interlex.BusinessLayer.Models
{
    public class HomeData
    {
        public List<Document> NewDocs { get; set; }

        public List<HomeFolderData> Folders { get; set; } = new List<HomeFolderData>();
    }
}
