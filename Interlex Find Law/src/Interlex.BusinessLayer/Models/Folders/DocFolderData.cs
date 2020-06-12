using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.Folders
{//
    public class DocFolderData
    {
        public int UserDocId { get; set; }

        public string DocName { get; set; }

        public DocFolderData()
        {
        }

        public DocFolderData(int userDocId, string docName)
        {
            this.UserDocId = userDocId;
            this.DocName = docName;
        }
    }
}
