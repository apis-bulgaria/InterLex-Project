using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class RecentDoc : Document
    {
        public int RecentDocId { get; set; }
        public bool Pinned { get; set; }

        public string OpenDate { get; set; }
    }
}
