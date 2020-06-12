using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    
    public class TableData<T> where T : class, new()
    {
        // page number
        public int Draw { get; set; }

        public int RecordsTotal { get; set; }

        public int RecordsFiltered { get; set; }

        public IEnumerable<T> Data { get; private set; }

        public TableData(IEnumerable<T> data)
        {
            this.Data = data;
        }

        public string ErrorMessage { get; set; }
    }
}
