namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.DataLayer;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Models;
    using System.Data;

    public class LastLoginData : LoginData
    {
        public string SellerName { get; set; }

        public string ProductsString { get; set; }

        public string Note { get; set; }

        public static List<LastLoginData> GetLastLoginsDataNulls(int userId, int sellerId, int clientId, DateTime dateFrom, DateTime dateTo, int skip, int take, string orderby, string orderdir, bool showOnlyNulls, string note, int currentSellerId)
        {
            var items = new List<LastLoginData>();

            IEnumerable<IDataRecord> loginsData = new List<IDataRecord>();
            if (showOnlyNulls)
            {
                loginsData = DB.GetLastLoginsNulls(userId, sellerId, clientId, dateFrom, dateTo, skip, take, orderby, orderdir, note, currentSellerId);
            }
            else
            {
                loginsData = DB.GetLastLogins(userId, sellerId, clientId, dateFrom, dateTo, skip, take, orderby, orderdir, note, currentSellerId);
            }

            foreach (var r in loginsData)
            {
                var item = new LastLoginData();
                item.GlobalQueryCount = int.Parse(r["global_query_count"].ToString());
                item.Username = r["username"].ToString();
                item.SellerName = r["seller_name"].ToString();
                item.ProductsString = r["products_string"].ToString();
                item.Note = r["note"].ToString();
                item.Date = r["last_login"].ToString();

                items.Add(item);
            }

            return items;
        }
    }
}
