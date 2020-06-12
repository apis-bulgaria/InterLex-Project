using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class FinsCurrencyDataRow
    {
        private decimal forEur;
        private decimal forUsd;
        private decimal forGbp;
        private DateTime date;

        public int Id { get; set; }

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

        public static IEnumerable<FinsCurrencyDataRow> GetFinsEuroLibor(string currentCurrency, DateTime dateFrom, string orderDir)
        {
            var items = new List<FinsCurrencyDataRow>();
            int count = 0;

            foreach (var r in DB.GetFinsCurrency(currentCurrency, dateFrom, orderDir, "currencies"))
            {
                var item = new FinsCurrencyDataRow();
                item.Date = DateTime.Parse(r["m"].ToString());
                if (r["eur"] != null && r["eur"] != DBNull.Value)
                {
                    item.ForEur = Decimal.Parse(r["eur"].ToString());
                }
                if (r["usd"] != null && r["usd"] != DBNull.Value)
                {
                    item.ForUsd = Decimal.Parse(r["usd"].ToString());
                }
                if (r["gbp"] != null && r["gbp"] != DBNull.Value)
                {
                    item.ForGbp = Decimal.Parse(r["gbp"].ToString());
                }

                item.Id = count;

                items.Add(item);

                count++;
            }

            CalculateChanges(items);

            return items;
        }

        private static void CalculateChanges(List<FinsCurrencyDataRow> dataRows)
        {
            if (dataRows.Count > 1)
            {
                dataRows[0].ForEurChangeTable = "-";
                dataRows[0].ForUsdChangeTable = "-";
                dataRows[0].ForGbpChangeTable = "-";
                for (int i = 1; i < dataRows.Count; i++)
                {
                    var changeEur = dataRows[i].ForEur - dataRows[i - 1].ForEur;
                    if (changeEur > 0)
                    {
                        dataRows[i].ForEurChangeTable = "<p class=\"f-blue\"><span class=\"fa fa-arrow-up \"></span> " + Math.Abs(changeEur) + "</p>";
                    }
                    else if (changeEur == 0)
                    {
                        dataRows[i].ForEurChangeTable = "<p class=\"f-lgrey\"><span class=\"fa fa-arrow-right\"></span> " + changeEur.ToString() + "</p>";
                    }
                    else
                    {
                        dataRows[i].ForEurChangeTable = "<p class=\"f-orange\"><span class=\"fa fa-arrow-down \"></span> " + Math.Abs(changeEur) + "</p>";
                    }

                    var changeUsd = dataRows[i].ForUsd - dataRows[i - 1].ForUsd;
                    if (changeUsd > 0)
                    {
                        dataRows[i].ForUsdChangeTable = "<p class=\"f-blue\"><span class=\"fa fa-arrow-up \"></span> " + Math.Abs(changeUsd) + "</p>";
                    }
                    else if (changeUsd == 0)
                    {
                        dataRows[i].ForUsdChangeTable = "<p class=\"f-lgrey\"><span class=\"fa fa-arrow-right\"></span> " + changeUsd.ToString() + "</p>";
                    }
                    else
                    {
                        dataRows[i].ForUsdChangeTable = "<p class=\"f-orange\"><span class=\"fa fa-arrow-down \"></span> " + Math.Abs(changeUsd) + "</p>";
                    }

                    var changeGbp = dataRows[i].ForGbp - dataRows[i - 1].ForGbp;
                    if (changeGbp > 0)
                    {
                        dataRows[i].ForGbpChangeTable = "<p class=\"f-blue\"><span class=\"fa fa-arrow-up text-danger\"></span> " + Math.Abs(changeGbp) + "</p>";
                    }
                    else if (changeGbp == 0)
                    {
                        dataRows[i].ForGbpChangeTable = "<p class=\"f-lgrey\"><span class=\"fa fa-arrow-right\"></span> " + changeGbp.ToString() + "</p>";
                    }
                    else
                    {
                        dataRows[i].ForGbpChangeTable = "<p class=\"f-orange\"><span class=\"fa fa-arrow-down\"></span> " + Math.Abs(changeGbp) + "</p>";
                    }
                }
            }
        }
    }
}
