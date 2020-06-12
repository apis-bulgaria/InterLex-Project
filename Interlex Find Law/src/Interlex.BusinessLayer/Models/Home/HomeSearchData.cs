using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class HomeSearchData
    {
        public int QueryType { get; set; }
        public string Query { get; set; }
        // All languages titles
        public Dictionary<int, string> Titles { get; set; }

        public Dictionary<int, string> Filters { get; set; } = new Dictionary<int, string>();

        public HomeSearchData()
        {
            Titles = new Dictionary<int, string>();
            this.Filters = new Dictionary<int, string>();
        }

        
    }
}
