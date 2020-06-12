using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EuFins.DataReader;

namespace EuFins.Table
{
    public class FinsCurrencyDataRow : DataRow
    {
        private decimal forEur;
        private decimal forUsd;
        private decimal forGbp;
        private DateTime date;

        public string DateTable { get; set; }

        public string DayOfWeekTable { get; set; }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                this.DateTable = value.ToString("dd.MM.yyyy");
                this.DayOfWeekTable = value.DayOfWeek.ToString();
            }
        }

        public decimal ForEur
        {
            get { return forEur; }
            set
            {
                forEur = value;
                this.ForEurTable = value.ToString();
            }
        }

        public decimal ForUsd
        {
            get { return forUsd; }
            set
            {
                forUsd = value;
                this.ForUsdTable = value.ToString();
            }
        }

        public decimal ForGbp
        {
            get { return forGbp; }
            set
            {
                forGbp = value;
                this.ForGbpTable = value.ToString();
            }
        }

        public string ForEurTable { get; set; }

        public string ForEurChangeTable { get; set; }

        public string ForUsdTable { get; set; }

        public string ForUsdChangeTable { get; set; }

        public string ForGbpTable { get; set; }

        public string ForGbpChangeTable { get; set; }

        public override void CalculateChanges(IEnumerable<IDataRow> dataRows)
        {
            var finsCurrencyDataRows = dataRows.OfType<FinsCurrencyDataRow>().ToList();
            if (finsCurrencyDataRows.Count > 1)
            {
                finsCurrencyDataRows[0].ForEurChangeTable = "-";
                finsCurrencyDataRows[0].ForUsdChangeTable = "-";
                finsCurrencyDataRows[0].ForGbpChangeTable = "-";
                for (int i = 1; i < finsCurrencyDataRows.Count; i++)
                {
                    var changeEur = finsCurrencyDataRows[i].ForEur - finsCurrencyDataRows[i - 1].ForEur;
                    if (changeEur > 0)
                    {
                        finsCurrencyDataRows[i].ForEurChangeTable = "<p><span class=\"fa fa-arrow-up text-danger\"></span> " + Math.Abs(changeEur) + "</p>";
                    }
                    else
                    {
                        finsCurrencyDataRows[i].ForEurChangeTable = "<p><span class=\"fa fa-arrow-down text-success\"></span> " + Math.Abs(changeEur) + "</p>";
                    }

                    var changeUsd = finsCurrencyDataRows[i].ForUsd - finsCurrencyDataRows[i - 1].ForUsd;
                    if (changeUsd > 0)
                    {
                        finsCurrencyDataRows[i].ForUsdChangeTable = "<p><span class=\"fa fa-arrow-up text-danger\"></span> " + Math.Abs(changeUsd) + "</p>";
                    }
                    else
                    {
                        finsCurrencyDataRows[i].ForUsdChangeTable = "<p><span class=\"fa fa-arrow-down text-success\"></span> " + Math.Abs(changeUsd) + "</p>";
                    }

                    var changeGbp = finsCurrencyDataRows[i].ForGbp - finsCurrencyDataRows[i - 1].ForGbp;
                    if (changeGbp > 0)
                    {
                        finsCurrencyDataRows[i].ForGbpChangeTable = "<p><span class=\"fa fa-arrow-up text-danger\"></span> " + Math.Abs(changeGbp) + "</p>";
                    }
                    else
                    {
                        finsCurrencyDataRows[i].ForGbpChangeTable = "<p><span class=\"fa fa-arrow-down text-success\"></span> " + Math.Abs(changeGbp) + "</p>";
                    }
                }
            }
        }

        public override ReaderMeta GetReaderMeta(params string[] parameters)
        {
            ReaderMeta meta = new ReaderMeta();
            meta.PostgreSqlQuery = " SELECT * FROM currencies WHERE cn = '" + parameters[3] + "' AND m >= '" + parameters[0] + "' AND m < '" + parameters[1] + "' ORDER BY m " + parameters[2] + "; ";
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "Date", PropType = "System.DateTime", PrgeColumn = "m" });
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "ForEur", PropType = "System.Decimal", PrgeColumn = "eur" });
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "ForUsd", PropType = "System.Decimal", PrgeColumn = "usd" });
            meta.MapPropertyToPgreColumn.Add(new PropMeta() { PropName = "ForGbp", PropType = "System.Decimal", PrgeColumn = "gbp" });

            return meta;
        }
    }
}