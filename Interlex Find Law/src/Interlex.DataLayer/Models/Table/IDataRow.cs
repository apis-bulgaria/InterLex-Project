using EuFins.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuFins.Table
{
    public interface IDataRow
    {
        int Id { get; set; }

        void CalculateChanges(IEnumerable<IDataRow> dataRows);

        ReaderMeta GetReaderMeta(params string[] parameters);
    }
}
