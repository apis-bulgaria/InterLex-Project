using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.Folders
{//
    public class UserFolderData
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public bool IsEmptyFolder { get; set; }
        public int DocumentsCount { get; set; } = 0;

        public UserFolderData()
        {

        }

        public UserFolderData(int id, string name, bool isEmptyFolder)
        {
            this.Id = id;
            this.Title = name;
            this.IsEmptyFolder = isEmptyFolder;
        }

        public UserFolderData(int id, string name, bool isEmptyFolder, int documentsCount)
        {
            this.Id = id;
            this.Title = name;
            this.IsEmptyFolder = isEmptyFolder;
            this.DocumentsCount = documentsCount;
        }
    }
}
