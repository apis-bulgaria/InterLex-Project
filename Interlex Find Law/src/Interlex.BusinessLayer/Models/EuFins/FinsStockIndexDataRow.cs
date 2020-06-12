using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class FinsStockIndexDataRow
    {
        public int Id { get; set; }

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

        public static IEnumerable<FinsStockIndexDataRow> GetFinsEuroLibor(string indexName, DateTime dateFrom, string orderDir)
        {
            var items = new List<FinsStockIndexDataRow>();
            int count = 0;

            foreach (var r in DB.GetFinsStockIndex(indexName, dateFrom, orderDir))
            {
                var item = new FinsStockIndexDataRow();
                item.Date = DateTime.Parse(r["m"].ToString());
                item.Value = Decimal.Parse(r["v"].ToString());

                item.Id = count;

                items.Add(item);

                count++;
            }

            CalculateChanges(items);

            return items;
        }

        public static DateTime GetFinsStockIndexLastDate(string indexName)
        {
            return DB.GetFinsStockIndexLastDate(indexName);
        }

        public static Dictionary<string, DateTime> GetFinsStockIndexLastDate()
        {
            return DB.GetFinsStockIndexLastDate();
        }

        private static void CalculateChanges(List<FinsStockIndexDataRow> dataRows)
        {
            if (dataRows.Count > 1)
            {
                dataRows[0].ValueChangeTable = "&nbsp;";
                dataRows[0].ValueChange = 0;
                for (int i = 1; i < dataRows.Count; i++)
                {
                    var changeLibor = dataRows[i].Value - dataRows[i - 1].Value;
                    if (changeLibor > 0)
                    {
                        dataRows[i].ValueChangeTable = "<p class=\"f-blue\"><span class=\"fa fa-arrow-up \"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                    else if (changeLibor == 0)
                    {
                        dataRows[i].ValueChangeTable = "<p class=\"f-lgrey\"><span class=\"fa fa-arrow-right\"></span> " + changeLibor.ToString() + "</p>";
                    }
                    else
                    {
                        dataRows[i].ValueChangeTable = "<p class=\"f-orange\"><span class=\"fa fa-arrow-down \"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                }
            }
        }
    }
}
