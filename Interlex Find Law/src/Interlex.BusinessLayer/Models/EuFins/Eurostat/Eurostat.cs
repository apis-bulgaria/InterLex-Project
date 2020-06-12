using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    public class Eurostat
    {
        public string StatisticName { get; set; }
        
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public List<EuroStatTable> Tables { get; set; }

        public DateTime LastExtraction { get; set; }

        public Eurostat()
        {
            this.Tables = new List<EuroStatTable>();
        }
    }
}
