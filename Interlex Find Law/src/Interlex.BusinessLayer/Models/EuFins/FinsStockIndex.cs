using Interlex.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class FinsStockIndex
    {
        public DateTime Date { get; set; }

        public string DateAsString { get; set; }

        public string Period { get; set; }

        public List<FinsStockIndexType> StockIndexes { get; set; }

        public Dictionary<string, string> LastDates { get; set; }

        public FinsStockIndex()
        {
            this.StockIndexes = new List<FinsStockIndexType>();
            this.LastDates = this.GetFinsStockIndexLastDate();
        }

        public static List<FinsStockIndexType> GetFinsStockIndexTypes()
        {
            var items = new List<FinsStockIndexType>();

            foreach (var item in DB.GetFinsStockIndexTypes())
            {
                var libor = new FinsStockIndexType();

                libor.Name = item["n"].ToString();

                items.Add(libor);
            }

            return items;
        }

        public static DateTime GetFinsStockIndexLastDate(string liborType)
        {
            return DB.GetFinsStockIndexLastDate(liborType);
        }

        public Dictionary<string, string> GetFinsStockIndexLastDate()
        {
            return DB.GetFinsStockIndexLastDate().ToDictionary(kv => kv.Key, kv => kv.Value.ToString("yyyy-MM-dd")); ;
        }
    }

    public class FinsStockIndexType
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
