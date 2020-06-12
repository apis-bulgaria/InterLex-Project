using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.DataLayer;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class EuroLibor
    {
        public DateTime Date { get; set; }

        public string DateAsString { get; set; }

        public string Period { get; set; }

        public Dictionary<string, string> LastDates { get; set; }

        public List<EuroLiborType> Libors { get; set; }

        public EuroLibor()
        {
            this.Libors = new List<EuroLiborType>();
            this.LastDates = this.GetFinsEuroLiborLastDate();
        }

        public static List<EuroLiborType> GetFinsEuroLiborTypes()
        {
            var items = new List<EuroLiborType>();

            foreach (var item in DB.GetFinsEuroLiborTypes())
            {
                if (item["n"].ToString().ToLower() == "euribor1")
                {
                    continue; // skipping EuRibor
                }

                var libor = new EuroLiborType()
                {
                    Name = item["n"].ToString()
                };
                items.Add(libor);
            }

            return items;
        }

        public static DateTime GetFinsEuroLiborLastDate(string liborType)
        {
            return DB.GetFinsEuroLiborLastDate(liborType);
        }

        public Dictionary<string, string> GetFinsEuroLiborLastDate()
        {
            return DB.GetFinsEuroLiborLastDate().ToDictionary(kv => kv.Key, kv => kv.Value.ToString("yyyy-MM-dd"));
        }
    }

    public class EuroLiborType
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
