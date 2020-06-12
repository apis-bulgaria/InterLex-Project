using EuFins.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EuFins.Table
{
    public abstract class DataRow : IDataRow
    {
        public int Id { get; set; }

        public abstract void CalculateChanges(IEnumerable<IDataRow> dataRows);

        public abstract ReaderMeta GetReaderMeta(params string[] parameters);
    }
}