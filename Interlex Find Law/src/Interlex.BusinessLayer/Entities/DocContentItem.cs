using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Entities
{
    [DebuggerDisplay("{eid}")]
    public class DocContentItem
    {
        public List<DocContentItem> children { get; set; }

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

        public int? parent_id
        {
            get;
            set;
        }

        public string eid { get; set; }

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

        public string extraClasses { get; set; }

    }
}
