using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Entities
{
    public class HomeFolderData
    {

        public HomeFolderData()
        {
            this.selected = false;
        }

        public string title
        {
            get;
            set;
        }

        public string tooltip { get; set; }

        public int id
        {
            get;
            set;
        }

        public int? parent_id { get; set; }

        public string key
        {
            get;
            set;
        }

        public int? queryType { get; set; }
        public string query { get; set; }

        public bool selected { get; set; }

        public bool folder
        {
            get;
            set;
        }

        public bool lazy
        {
            get;
            set;
        }

        public bool expanded
        {
            get;
            set;
        }

        public int ord { get; set; }

        public string extraClasses { get; set; }

        public int DocsCount { get; set; }

        public IEnumerable<HomeFolderData> children { get; set; }

    }
}
