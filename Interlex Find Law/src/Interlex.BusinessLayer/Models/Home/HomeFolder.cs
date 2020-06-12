using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlex.BusinessLayer.Entities;

namespace Interlex.BusinessLayer.Models
{
    public class HomeFolder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<HomeFolderData> Folders { get; set; }

        public HomeFolder()
        {
            Folders = new List<HomeFolderData>();
        }
    }
}
