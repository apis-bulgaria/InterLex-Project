using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EuFins.DataReader;

namespace EuFins.Table
{
    public class SofiDataRow : DataRow
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
            var euroLiborDataRows = dataRows.OfType<SofiDataRow>().ToList();
            if (euroLiborDataRows.Count > 1)
            {
                euroLiborDataRows[0].ValueChangeTable = "&nbsp;";
                euroLiborDataRows[0].ValueChange = 0;
                for (int i = 1; i < euroLiborDataRows.Count; i++)
                {
                    var changeLibor = euroLiborDataRows[i].Value - euroLiborDataRows[i - 1].Value;
                    if (changeLibor > 0)
                    {
                        euroLiborDataRows[i].ValueChangeTable = "<p><span class=\"fa fa-arrow-up text-danger\"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                    else
                    {
                        euroLiborDataRows[i].ValueChangeTable = "<p><span class=\"fa fa-arrow-down text-success\"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                }
            }
        }

        public override ReaderMeta GetReaderMeta(params string[] parameters)
        {
            ReaderMeta meta = new ReaderMeta();
            meta.PostgreSqlQuery = " SELECT DISTINCT m, " + parameters[4] + " FROM lsbb WHERE m >= '" + parameters[0] + "' AND m < '" + parameters[1] + "' AND n = '" + parameters[5] + "' ORDER BY m " + parameters[2] + " ; ";
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "Date", PropType = "System.DateTime", PrgeColumn = "m" });
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "Value", PropType = "System.Decimal", PrgeColumn = parameters[4] });

            return meta;
        }
    }
}