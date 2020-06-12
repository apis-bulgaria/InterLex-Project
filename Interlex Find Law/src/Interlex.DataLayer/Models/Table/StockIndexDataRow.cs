using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EuFins.DataReader;

namespace EuFins.Table
{
    public class StockIndexDataRow : DataRow
    {
        private DateTime date;

        public string DateTable { get; set; }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                this.DateTable = value.ToString("dd.MM.yyyy");
            }
        }

        public decimal Value { get; set; }

        public decimal ValueChange { get; set; }

        public string ValueChangeTable { get; set; }

        public override void CalculateChanges(IEnumerable<IDataRow> dataRows)
        {
            var stockIndexDataRows = dataRows.OfType<StockIndexDataRow>().ToList();
            if (stockIndexDataRows.Count > 1)
            {
                stockIndexDataRows[0].ValueChangeTable = "&nbsp;";
                stockIndexDataRows[0].ValueChange = 0;
                for (int i = 1; i < stockIndexDataRows.Count; i++)
                {
                    var changeLibor = stockIndexDataRows[i].Value - stockIndexDataRows[i - 1].Value;
                    if (changeLibor > 0)
                    {
                        stockIndexDataRows[i].ValueChangeTable = "<p><span class=\"fa fa-arrow-up text-danger\"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                    else
                    {
                        stockIndexDataRows[i].ValueChangeTable = "<p><span class=\"fa fa-arrow-down text-success\"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                }
            }
        }

        public override ReaderMeta GetReaderMeta(params string[] parameters)
        {
            ReaderMeta meta = new ReaderMeta();
            meta.PostgreSqlQuery = " SELECT m, v FROM indices WHERE m >= '" + parameters[0] + "' AND m < '" + parameters[1] + "' AND n = '" + parameters[5] + "' ORDER BY m " + parameters[2] + " ; ";
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "Date", PropType = "System.DateTime", PrgeColumn = "m" });
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "Value", PropType = "System.Decimal", PrgeColumn = "v" });

            return meta;
        }
    }
}