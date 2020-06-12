using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class TableFormData
    {
        public string CurrentCurrency { get; set; }

        public DateTime CurrentDate { get; set; }
        private long currentDateJs;

        public long CurrentDateJs
        {
            get { return currentDateJs; }
            set
            {
                currentDateJs = value;
                this.CurrentDate = new DateTime(1970, 1, 1).AddMilliseconds(value);
            }
        }

        public string Name { get; set; }

        public string StockIndex { get; set; }

        public string LiborFor { get; set; }

        public string ReaderType { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Draw { get; set; }

        public Column[] Columns { get; set; }

        public Order[] Order { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public Search Search { get; set; }
    }

    public class Column
    {
        public string Data { get; set; }

        public string Name { get; set; }

        public bool Searchable { get; set; }

        public bool Orderable { get; set; }

        public Search Search { get; set; }
    }

    public class Search
    {
        public string Value { get; set; }

        public bool Regex { get; set; }
    }

    public class Order
    {
        public string Column { get; set; }

        public string Dir { get; set; }
    }
}