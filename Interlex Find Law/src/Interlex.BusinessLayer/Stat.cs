namespace Interlex.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;
    using Interlex.DataLayer;
    using Interlex.BusinessLayer.Models;

    public class Stat
    {
        #region Search statistics functions

        public static int SetStatSearch(string txt)
        {
            return DB.SetStatSearch(txt);
        }

        public static int SetStatSearch(string txt, int productId)
        {
            return DB.SetStatSearch(txt, productId);
        }

        public static void SetStatSearchDoc(int statSearchId, int docId)
        {
            DB.SetStatSearchDoc(statSearchId, docId);
        }

        public static Dictionary<string, int> GetStatSearches(string like, int productId)
        {
            int entriesToReturn = int.Parse(ConfigurationManager.AppSettings["AutoComplete_GlobalSearchesCount"]);

            IEnumerable<IDataRecord> allSearches = DB.GetStatSearches(like, entriesToReturn, productId);

            Dictionary<string, int> searchesDict = new Dictionary<string, int>();

            foreach (var item in allSearches)
            {
                var currentSearchLabel = item["txt"].ToString();
                var currentCount = int.Parse(item["cnt"].ToString());
                searchesDict.Add(currentSearchLabel, currentCount);
            }

            return searchesDict;
        }

        public static List<StatSearch> GetStatSearches(string like, int take, int skip, string orderby, string orderdir, int? productId)
        {
            IEnumerable<IDataRecord> desiredSearches = DB.GetStatSearches(like, take, skip, orderby, orderdir, productId);

            int totalSearchesCount = DB.GetAllSearchesCount();

            var returnedSearches = new List<StatSearch>();

            foreach (var search in desiredSearches)
            {
                var curCount = int.Parse(search["cnt"].ToString());
                var curPart = (((decimal)curCount) / ((decimal)totalSearchesCount));
                var curPercentage = curPart * 100;
                string productName;

                switch (Convert.ToInt32(search["product_id"].ToString()))
                {
                    case 1:
                        productName = "EuroCases";
                        break;
                    case 2:
                        productName = "TAX & FINANCIAL STANDARDS";
                        break;
                    default:
                        productName = "EuroCases";
                        break;
                }

                var currentSearch = new StatSearch
                {
                    Id = int.Parse(search["id"].ToString()),
                    Text = search["txt"].ToString(),
                    Count = curCount,
                    Percentage = (Math.Round(curPercentage, 2)).ToString() + "%",
                    ProductName = productName
                };

                returnedSearches.Add(currentSearch);
            }

            return returnedSearches;
        }

        public static int GetStatSearchesCount()
        {
            return DB.GetAllSearchesCount();
        }

        public static int GetLikeSearchesCount(string like)
        {
            return DB.GetLikeSearchesCount(like);
        }

        public static int GetLikeSearchesCount(string like, int? productId)
        {
            return DB.GetLikeSearchesCount(like, productId);
        }

        public static SearchStatistics GetSearchStatistics(int clientId, string searchText, DateTime from, DateTime to, int currentSellerID)
        {
            var dataRecord = DB.GetSearchStatistics(clientId, searchText, from, to, currentSellerID);
            var searchStat = new SearchStatistics(dataRecord);
            return searchStat;
        }

        #endregion

        #region Login statistics functions
        public static void AddLogin(int userId, int browserId, bool isMobileDevice, string ip, int clientId, int? sellerId)
        {
            DB.AddLogin(userId, browserId, isMobileDevice, ip, clientId, sellerId);
        }

        public static void AddInvalidLogin(string username, string ip, string errorMsg)
        {
            DB.AddInvalidLogin(username, ip, errorMsg);
        }
        #endregion

        #region Stat Export Functions

        public static void ExportStatClients(string searchText, string sortBy, string sortDir, int sellerId, int currentSellerId, string path)
        {
            DB.ExportStatClients(searchText, sortBy, sortDir, sellerId, currentSellerId, path);
        }

        public static void ExportStatUsers(string searchText, string sortBy, string sortDir, int userTypeId, int sellerId, string note, int minLoginCount, int maxLoginCount, int originId, int currentSellerId, string path)
        {
            DB.ExportStatUsers(searchText, sortBy, sortDir, userTypeId, sellerId, note, minLoginCount, maxLoginCount, originId, currentSellerId, path);
        }

        public static void ExportStatSearches(string searchText, string sortBy, string sortDir, string path)
        {
            DB.ExportStatSearches(searchText, sortBy, sortDir, path);
        }

        public static void ExportStatLogins(string searchText, DateTime dateFrom, DateTime dateTo, int browserId, int currentSellerId, string sortBy, string sortDir, string path)
        {
            DB.ExportStatLogins(searchText, dateFrom, dateTo, browserId, currentSellerId, sortBy, sortDir, path);
        }



        #endregion

        public static Dictionary<string, int> GetClassifiersUsage(string formName, int clientId, string text, DateTime dateFrom, DateTime dateTo, bool returnNullValues)
        {
            var dict = new Dictionary<string, int>();
            string[] arrayFromDB = DB.GetClassifiersUsage(formName, clientId, text, dateFrom, dateTo);
            foreach (var record in arrayFromDB)
            {
                var splitted = record.Split(':');

                if (returnNullValues)
                {
                    dict.Add(splitted[0], int.Parse(splitted[1]));
                }
                else // not if-else-if intentionally for future changes ease
                {
                    if (splitted[1] != "0")
                    {
                        dict.Add(splitted[0], int.Parse(splitted[1]));
                    }
                } 
            }

            return dict;
        }
    }
}
