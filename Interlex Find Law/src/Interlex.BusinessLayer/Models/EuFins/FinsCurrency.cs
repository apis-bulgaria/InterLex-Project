using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class FinsCurrency
    {
        public DateTime Date { get; set; }

        public string DateAsString { get; set; }

        public string Period { get; set; }

        public List<FinsCurrencyType> FinsCurrencies { get; set; }

        public Dictionary<string, string> LastDates { get; set; }

        public FinsCurrency(string currType = "currencies")
        {
            this.FinsCurrencies = new List<FinsCurrencyType>();
            this.LastDates = this.GetFinsCurrencyLastDates(currType);
        }

        public static List<FinsCurrencyType> GetFinsCurrencyEcbTypes()
        {
            return FinsCurrency.GetFinsCurrencyTypes("ecb");
        }

        public static List<FinsCurrencyType> GetFinsCurrencyTypes(string currType = "currencies")
        {
            var items = new List<FinsCurrencyType>();

            foreach (var item in DB.GetFinsCurrencyTypes(currType))
            {
                var libor = new FinsCurrencyType();

                libor.Name = item["cn"].ToString();
                libor.Country = item["n"].ToString();

                items.Add(libor);
            }

          //  throw new ArgumentException("TEST");

            return items;
        }

        public static DateTime GetFinsCurrencyEcbLastDate(string liborType)
        {
            return FinsCurrency.GetFinsCurrencyLastDate(liborType, "ecb");
        }

        public static DateTime GetFinsCurrencyLastDate(string liborType, string currType = "currencies")
        {
            return DB.GetFinsCurrencyLastDate(liborType, currType);
        }

        public Dictionary<string, string> GetFinsCurrencyLastDates(string currType = "currencies")
        {
            return DB.GetFinsCurrencyLastDate(currType).ToDictionary(kv => kv.Key, kv => kv.Value.ToString("yyyy-MM-dd"));
        }
    }

    public class FinsCurrencyType
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public string Name { get; set; }
    }
}
