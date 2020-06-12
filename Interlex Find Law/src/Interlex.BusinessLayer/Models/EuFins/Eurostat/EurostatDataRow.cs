using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    public abstract class EurostatDataRow
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public List<EurostatValue> HicpValues { get; set; }

        public static DateTime GetLastExtraction()
        {
            return DB.GetFinsEurostatLastExtraction();
        }

        protected static IEnumerable<HicpDataRow> GetEurostatDataRows(string statisticType, string tableType, string tableTypeAffix, DateTime dateFrom, DateTime dateTo, int langId)
        {
            var items = new List<HicpDataRow>();
            var rowDatas = new Dictionary<string, List<EurostatValue>>();
            var unitToCountry = new Dictionary<string, string>();

            foreach (var r in DB.GetFinsEurostat(statisticType, tableType, tableTypeAffix, dateFrom, dateTo, langId))
            {

                string geoUnit = (string)r["geo_unit"];

                var eurostatValue = new EurostatValue();
                eurostatValue.TimePeriod = (string)r["time_period"];
                SetOrder(eurostatValue);

                var value = r["value"].ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    decimal valueDec = 0;
                    if (decimal.TryParse(value, out valueDec))
                    {
                        eurostatValue.Value = valueDec;
                    }
                }

                string valueAffix = (string)r["valueaffix"];
                if (valueAffix != ":")
                {
                    eurostatValue.TableValue = "<p>" + eurostatValue.Value.ToString("0.00") + "<span>" + valueAffix + "</span></p>";
                }
                else
                {
                    eurostatValue.TableValue = "<p>" + valueAffix + "</p>";
                }

                string country = (r["country"] != DBNull.Value) ? (string)r["country"] : geoUnit;
                if (rowDatas.ContainsKey(geoUnit))
                {
                    rowDatas[geoUnit].Add(eurostatValue);
                }
                else
                {
                    unitToCountry.Add(geoUnit, country);
                    rowDatas.Add(geoUnit, new List<EurostatValue>() { eurostatValue });
                }
            }

            var valueRows = rowDatas.OrderBy(x =>
                               x.Key.StartsWith("EU") ? 1 :
                               x.Key.StartsWith("EA") ? 2 : 3)
                               .ThenBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

            int id = 1;
            foreach (var rowData in valueRows)
            {
                HicpDataRow item = new HicpDataRow();
                item.Country = unitToCountry[rowData.Key];
                item.HicpValues = rowData.Value.OrderBy(x => x.Order).ThenBy(x => x.TimePeriod).ToList();

                item.Id = id;

                items.Add(item);
            }

            // string json = JsonConvert.SerializeObject(items);
            return items;
        }

        private static void SetOrder(EurostatValue eurostatValue)
        {
            if (eurostatValue.TimePeriod.Contains("M"))
            {
                var args = eurostatValue.TimePeriod.Split(new char[] { 'M' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (args != null && args.Count == 2)
                {
                    int year = 0;
                    if (int.TryParse(args[0], out year))
                    {
                        int month = 0;
                        if (int.TryParse(args[1], out month))
                        {
                            eurostatValue.TimePeriod = ToRoman(month) + "." + year;
                            eurostatValue.Order = int.Parse($"{year}{month.ToString("00")}");
                        }
                    }
                }
            }
            else
            {
                int year = 0;
                if (int.TryParse(eurostatValue.TimePeriod, out year))
                {
                    eurostatValue.Order = year;
                }
            }
        }

        private static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }

    public class EurostatValue
    {
        public string TimePeriod { get; set; }

        public string TableValue { get; set; }

        public decimal Value { get; set; }

        public int Order { get; set; }
    }
}
