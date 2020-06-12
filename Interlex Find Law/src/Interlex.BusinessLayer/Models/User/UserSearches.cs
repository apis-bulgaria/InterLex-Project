namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.DataLayer;
    using System.Configuration;
    using Newtonsoft.Json;

    public enum UserSearchDatePeriod
    {
        All = 0,
        Today = 1,
        Yesterday = 2,
        LastWeek = 3,
        LastMonth = 4
    }

    public enum UserSearchType
    {
        All = 0,
        Simple = 1,
        Law = 2,
        Cases = 3,
        FinancesAdvanced = 4
    }

    public class UserSearchFilters
    {
        public UserSearchDatePeriod Period { get; set; }

        public UserSearchType Type { get; set; }

        public UserSearchFilters()
        {
            Period = UserSearchDatePeriod.All;
            Type = UserSearchType.All;
        }
    }

    public class UserSearches
    {
        public List<UserSearch> Items { get; set; }

        public UserSearchFilters Filters { get; set; }

        public UserSearches()
        {
            this.Items = new List<UserSearch>();
            this.Filters = new UserSearchFilters();
        }

        public static UserSearches GetSearchesFromDB(int userId, UserSearchDatePeriod period, UserSearchType type, int productId)
        {
            var newEntries = UserSearches.GetAllSearches(userId, "", period, type, productId);

            var model = new UserSearches();

            foreach (var search in newEntries)
            {
                var currentSearch = new UserSearch
                {
                    Text = search["txt"].ToString(),
                    Date = DateTime.Parse(search["search_date"].ToString()),
                    Id = int.Parse(search["id"].ToString()),
                    Details = JsonConvert.DeserializeObject<SearchDetails>(search["details"].ToString(), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include })
                };

                if (currentSearch.Details.Cases != null)
                {
                    currentSearch.Details.Cases.DateFromShort = currentSearch.Details.Cases.DateFrom.ToShortDateString();
                    currentSearch.Details.Cases.DateToShort = currentSearch.Details.Cases.DateTo.ToShortDateString();
                    currentSearch.Details.Cases.DateDefaultByCultureShort = new DateTime(1, 1, 1).ToShortDateString();
                }

                if (currentSearch.Details.Law != null)
                {
                    currentSearch.Details.Law.DateFromShort = currentSearch.Details.Law.DateFrom.ToShortDateString();
                    currentSearch.Details.Law.DateToShort = currentSearch.Details.Law.DateTo.ToShortDateString();
                    currentSearch.Details.Law.DateDefaultByCultureShort = new DateTime(1, 1, 1).ToShortDateString();
                }

                if (currentSearch.Details.Finances != null)
                {
                    currentSearch.Details.Finances.DateFromShort = currentSearch.Details.Finances.DateFrom.ToShortDateString();
                    currentSearch.Details.Finances.DateToShort = currentSearch.Details.Finances.DateTo.ToShortDateString();
                    currentSearch.Details.Finances.DateDefaultByCultureShort = new DateTime(1, 1, 1).ToShortDateString();
                }


                model.Items.Add(currentSearch);
            }

            return model;
        }

        public static IEnumerable<IDataRecord> GetTopSearches(int userId, string like, int productId)
        {
            int entriesToReturn = int.Parse(ConfigurationManager.AppSettings["AutoComplete_UserSearchesCount"]);

            return DB.GetUserSearches(userId, like, (int)UserSearchDatePeriod.All, (int)UserSearchType.All, productId).Take(entriesToReturn);
        }

        public static IEnumerable<IDataRecord> GetAllSearches(int userId, string like, UserSearchDatePeriod period, UserSearchType type, int productId)
        {
            return DB.GetUserSearches(userId, like, (int)period, (int)type, productId).Take(int.Parse(ConfigurationManager.AppSettings["UserSearchCount"]));
        }

        public static void DelUserSearch(int searchId)
        {
            DB.DelUserSearch(searchId);
        }

        public static void DelAllUserSearches(int userId, int productId)
        {
            DB.DelAllUserSearches(userId, productId);
        }

        public static void DelAllUserRecentDocuments(int userId)
        {
            DB.DelAllUserRecentDocuments(userId);
        }

        public static string GetUserSearchDetails(int searchId)
        {
            return DB.GetUserSearchDetails(searchId);
        }
    }
}
