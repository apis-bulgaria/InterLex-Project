namespace Interlex.BusinessLayer
{
    using Interlex.DataLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    public class AdminStatisticsExport
    {
        /// <summary>
        /// Gets clients data as a csv string
        /// </summary>
        public static string GetClientsData(string searchText, string sortBy, string sortDir, int sellerId, int currentSellerId, int originId)
        {
            var data = DB.GetClientsStats(searchText, 0, int.MaxValue, sortBy, sortDir, sellerId, currentSellerId, originId);
            return DBUtil.ConstructCSVStringFromData(data);
        }

        /// <summary>
        /// Gets users data as a csv string
        /// </summary>
        public static string GetUsersData(string searchText, string sortBy, string sortDir, int userTypeId, int sellerId, string note,
            int minLoginCount, int maxLoginCount, int originId, int currentSellerId)
        {
            var data = DB.GetUsersStat(searchText, 0, int.MaxValue, sortBy, sortDir, userTypeId, sellerId, note, minLoginCount, maxLoginCount, originId, currentSellerId);
            return DBUtil.ConstructCSVStringFromData(data);
        }

        /// <summary>
        /// Gets logins data as a csv string
        /// </summary>
        public static string GetLoginsData(int browserId, DateTime dateFrom, DateTime dateTo, string orderby, string orderdir, string searchText, int currentSellerId)
        {
            var data = DB.GetLogins(browserId, dateFrom, dateTo, 0, int.MaxValue, orderby, orderdir, searchText, currentSellerId);
            return DBUtil.ConstructCSVStringFromData(data);
        }

        /// <summary>
        /// Gets last logins data as a csv string
        /// </summary>
        public static string GetLastLoginsData(DateTime dateFrom, DateTime dateTo, int userId, int sellerId, int clientId, bool showOnlyNulls, string note, string orderby, string orderdir, int currentSellerId)
        {
            var data = new List<IDataRecord>();

            if (showOnlyNulls)
            {
                data = DB.GetLastLoginsNulls(userId, sellerId, clientId, dateFrom, dateTo, 0, int.MaxValue, orderby, orderdir, note, currentSellerId) as List<IDataRecord>;
            }
            else
            {
                data = DB.GetLastLogins(userId, sellerId, clientId, dateFrom, dateTo, 0, int.MaxValue, orderby, orderdir, note, currentSellerId) as List<IDataRecord>;
            }

            return DBUtil.ConstructCSVStringFromData(data);
        }

        /// <summary>
        /// Gets searches data as a csv string
        /// </summary>
        public static string GetSearchesData(string searchText)
        {
            var data = DB.GetStatSearches(searchText, int.MaxValue);
            return DBUtil.ConstructCSVStringFromData(data);
        }

        /// <summary>
        /// Gets opened documents data as a csv string
        /// </summary>
        public static string GetOpenedDocumentsData(string documentTitle, int userId, int clientId, int sellerId, string note, 
            DateTime dateFrom, DateTime dateTo, string orderby, string orderdir, int currentSellerId, int productId)
        {
            var data = DB.GetOpenedDocuments(documentTitle, userId, clientId, sellerId, note, dateFrom, dateTo, 0, int.MaxValue, orderby, orderdir, currentSellerId, productId);
            return DBUtil.ConstructCSVStringFromData(data);
        }

        /// <summary>
        /// Gets opened documenets counts data as a csv string
        /// </summary>
        /// <returns></returns>
        public static string GetOpenedDocumentsCountsData()
        {
            //TODO: Implement
            return String.Empty;
        }
    }
}
