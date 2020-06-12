using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class EuroLiborDataRow
    {
        public int Id { get; set; }

        private DateTime date;

        public string DateTable
        {
            get; set;
        }

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

        public static IEnumerable<EuroLiborDataRow> GetFinsEuroLibor(string liborFor, DateTime dateFrom, string liborType, string orderDir)
        {
            var items = new List<EuroLiborDataRow>();
            int count = 0;

            foreach (var r in DB.GetFinsEuroLibor(liborFor, dateFrom, liborType, orderDir))
            {
                var item = new EuroLiborDataRow()
                {
                    Date = DateTime.Parse(r["m"].ToString()),
                    Value = Decimal.Parse(r[liborFor].ToString()),
                    Id = count
                };
                items.Add(item);

                count++;
            }

            CalculateChanges(items);

            return items;
        }

        public static IEnumerable<EuroLiborDataRow> GetFinsEuribor(string liborFor, int year, string orderDir)
        {
            var items = new List<EuroLiborDataRow>();
            int count = 0;

            foreach (var r in DB.GetFinsEuribor(liborFor, year, orderDir))
            {
                var item = new EuroLiborDataRow()
                {
                    Date = DateTime.Parse(r["m"].ToString()),
                    Value = Decimal.Parse(r[liborFor].ToString()),
                    Id = count
                };
                items.Add(item);

                count++;
            }

            CalculateChanges(items);

            return items;
        }

        private static void CalculateChanges(List<EuroLiborDataRow> dataRows)
        {
            var euroLiborDataRows = dataRows;
            if (euroLiborDataRows.Count > 1)
            {
                euroLiborDataRows[0].ValueChangeTable = "&nbsp;";
                euroLiborDataRows[0].ValueChange = 0;
                for (int i = 1; i < euroLiborDataRows.Count; i++)
                {
                    var changeLibor = euroLiborDataRows[i].Value - euroLiborDataRows[i - 1].Value;
                    if (changeLibor > 0)
                    {
                        euroLiborDataRows[i].ValueChangeTable = "<p class=\"f-blue\"><span class=\"fa fa-arrow-up\"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                    else if (changeLibor == 0)
                    {
                        euroLiborDataRows[i].ValueChangeTable = "<p class=\"f-lgrey\"><span class=\"fa fa-arrow-right\"></span> " + changeLibor.ToString() + "</p>";
                    }
                    else
                    {
                        euroLiborDataRows[i].ValueChangeTable = "<p class=\"f-orange\"><span class=\"fa fa-arrow-down\"></span> " + Math.Abs(changeLibor) + "</p>";
                    }
                }
            }
        }
    }
}
