using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    public class EuroStatTable
    {
        public string TableName { get; set; }

        public List<EurostatDataRow> Stats { get; set; }

        public EuroStatTable()
        {
            this.Stats = new List<EurostatDataRow>();
        }
    }
}
