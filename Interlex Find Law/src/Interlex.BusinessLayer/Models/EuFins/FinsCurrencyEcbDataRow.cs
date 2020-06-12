using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class FinsCurrencyEcbDataRow
    {
        private decimal forEur;
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

        public string ForEurTable { get; set; }

        public string ForEurChangeTable { get; set; }

        public static IEnumerable<FinsCurrencyEcbDataRow> GetFinsCurrency(string currentCurrency, DateTime dateFrom, string orderDir)
        {
            var items = new List<FinsCurrencyEcbDataRow>();
            int count = 0;

            foreach (var r in DB.GetFinsCurrency(currentCurrency, dateFrom, orderDir, "ecb"))
            {
                var item = new FinsCurrencyEcbDataRow();
                item.Date = DateTime.Parse(r["m"].ToString());
                if (r["eur"] != null && r["eur"] != DBNull.Value)
                {
                    item.ForEur = Decimal.Parse(r["eur"].ToString());
                }

                item.Id = count;

                items.Add(item);

                count++;
            }

            CalculateChanges(items);

            return items;
        }

        private static void CalculateChanges(List<FinsCurrencyEcbDataRow> dataRows)
        {
            if (dataRows.Count > 1)
            {
                dataRows[0].ForEurChangeTable = "-";
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
                }
            }
        }
    }
}
