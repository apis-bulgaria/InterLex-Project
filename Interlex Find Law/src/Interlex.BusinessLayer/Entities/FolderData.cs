using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Entities
{
    public class FolderData
    {
        //private List<FolderData> _children;

        public FolderData()
        {
            //_children = new List<FolderData>();
            this.selected = false;
        }

        public string title
        {
            get;
            set;
        }

        public string tooltip { get; set; }

        public int? ord { get; set; }

        public Guid id
        {
            get;
            set;
        }

        public string key
        {
            get;
            set;
        }

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

        //public List<FolderData> children
        //{
        //    get
        //    {
        //        return _children;
        //    }
        //}

        public string extraClasses { get; set; }

        public int tree_level { get; set; }

        public int docs_count { get; set; }

        public bool hideCheckbox { get; set; }

        public bool unselectable { get; set; }

    }
}