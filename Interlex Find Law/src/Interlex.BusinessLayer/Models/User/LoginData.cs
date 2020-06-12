namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.DataLayer;
    using Interlex.BusinessLayer.Enums;

    public class LoginData
    {
        public int? UserId { get; set; }

        public string Username { get; set; }

        public bool IsMobileDevice { get; set; }

        public Browsers Browser { get; set; }

        public string Date { get; set; }

        public string Ip { get; set; }

        public int GlobalQueryCount { get; set; }

        public LoginData() { }

        public LoginData(LoginData ld) 
        {
            this.UserId = ld.UserId;
            this.Username = ld.Username;
            this.IsMobileDevice = ld.IsMobileDevice;
            this.Browser = ld.Browser;
            this.Date = ld.Date;
            this.Ip = ld.Ip;
            this.GlobalQueryCount = ld.GlobalQueryCount;
        }

        public static List<LoginData> GetLoginsData(int browserId, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir, string searchText, int currentSellerId) 
        {
            var items = new List<LoginData>();

            foreach (var r in DB.GetLogins(browserId, dateFrom, dateTo, start, take, orderby, orderdir, searchText, currentSellerId))
            {
                var item = new LoginData();
                item.Username = r["username"].ToString();
                item.IsMobileDevice = Convert.ToBoolean(r["is_mobile_device"]);
                item.Browser = (Browsers)int.Parse((r["browser_id"].ToString()));
                item.Date = r["date"].ToString();
                item.Ip = r["ip_addr"].ToString();
                item.GlobalQueryCount = int.Parse(r["total_query_count"].ToString());

                items.Add(item);
            }

            return items;
        }
    }
}
