using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class PalProvision
    {
        public string ToParOriginal { get; set; }

        public int ToDocParId { get; set; }
        public List<int> LinkIds { get; set; }
        public string Title { get; set; }
        public string SortTitle { get; set; }
        public int DBOrder { get; set; }
    }
}
