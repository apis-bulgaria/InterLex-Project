using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EuFins.Table
{
    public interface ITableData<out T>
    {
        int Draw { get; set; }

        int RecordsTotal { get; set; }

        int RecordsFiltered { get; set; }

        IEnumerable<T> Data { get; }

        string ErrorMessage { get; set; }
    }


    public class TableData<T> : ITableData<T>
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