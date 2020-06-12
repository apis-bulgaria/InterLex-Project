namespace Interlex.DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;
    using System.Data;
    using Npgsql;

    public partial class DB
    {
        public static int SetStatSearch(string txt)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.set_stat_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_txt", txt);
                int statSearchId = Convert.ToInt32(comm.ExecuteScalar());
                return statSearchId;
            }
        }

        public static int SetStatSearch(string txt, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.set_stat_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_txt", txt);
                comm.Parameters.AddWithValue("_product_id", productId);
                int statSearchId = Convert.ToInt32(comm.ExecuteScalar());
                return statSearchId;
            }
        }

        public static void SetStatSearchDoc(int statSearchId, int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.set_stat_search_doc", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_stat_search_id", statSearchId);
                comm.Parameters.AddWithValue("_doc_id", docId);
                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetStatSearches(string like, int take)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_input", like);
                comm.Parameters.AddWithValue("_take", take);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        // By product
        public static IEnumerable<IDataRecord> GetStatSearches(string like, int take, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_input", like);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_product_id", productId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }


        public static IEnumerable<IDataRecord> GetStatSearches(string like, int take, int skip, string orderby, string orderdir)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_input", like);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_skip", skip);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetStatSearches(string like, int take, int skip, string orderby, string orderdir, int? productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_input", like);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_skip", skip);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);
                comm.Parameters.AddWithValue("_product_id", productId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int GetAllSearchesCount()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_total_searches_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static int GetLikeSearchesCount(string like)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_like_searches_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_like", like);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static int GetLikeSearchesCount(string like, int? productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_stat_like_searches_count_product", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_like", like);
                comm.Parameters.AddWithValue("_product_id", productId);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static IEnumerable<IDataRecord> GetOpenedDocumentsStatByLanguage(int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_stat_by_language", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetOpenedDocumentsStatByType(int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_stat_by_type", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetOpenedDocumentsStatByProduct(int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_stat_by_product", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }
        
        public static void AddLogin(int userId, int browserId, bool isMobileDevice, string ip, int clientId, int? sellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.add_login", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_browser_id", browserId);
                comm.Parameters.AddWithValue("_ip_addr", ip);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_is_mobile_device", isMobileDevice);

                comm.ExecuteNonQuery();
            }
        }

        public static void AddInvalidLogin(string username, string ip, string errorMsg)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.add_invalid_login", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_ip_addr", ip);
                comm.Parameters.AddWithValue("_error_msg", errorMsg);

                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetLogins(int browserId, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir, string searchText, int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_logins", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_browser_id", browserId);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_start", start);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetLastLogins(int userId, int sellerId, int clientId, DateTime dateFrom, DateTime dateTo, int skip, int take, string orderby, string orderdir, string note, int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_last_logins", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_skip", skip);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetLastLoginsNulls(int userId, int sellerId, int clientId, DateTime dateFrom, DateTime dateTo, int skip, int take, string orderby, string orderdir, string note, int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_last_logins_nulls", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_skip", skip);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        #region Stat Export Functions

        /// <summary>
        /// Exports clients' administration statistics into a path to the DB server;
        /// </summary>
        /// <param name="path">The path for the export to be saved. All other parameters are the same as the statistics' function parameters</param>
        public static void ExportStatClients(string searchText, string sortBy, string sortDir, int sellerId, int currentSellerId, string path)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                using (NpgsqlCommand comm = new NpgsqlCommand("stat.export_clients_stat", conn))
                {
                    comm.CommandType = System.Data.CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("_search_text", searchText);
                    comm.Parameters.AddWithValue("_sort_by", sortBy);
                    comm.Parameters.AddWithValue("_sort_dir", sortDir);
                    comm.Parameters.AddWithValue("_seller_id", sellerId);
                    comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);
                    comm.Parameters.AddWithValue("_path", path);

                    comm.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Exports users' administration statistics into a path to the DB server;
        /// </summary>
        /// <param name="path">The path for the export to be saved. All other parameters are the same as the statistics' function parameters</param>
        public static void ExportStatUsers(string searchText, string sortBy, string sortDir, int userTypeId, int sellerId, string note, int minLoginCount, int maxLoginCount, int originId, int currentSellerId, string path)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                using (NpgsqlCommand comm = new NpgsqlCommand("stat.export_users_stat", conn))
                {
                    comm.CommandType = System.Data.CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("_search_text", searchText);
                    comm.Parameters.AddWithValue("_sort_by", sortBy);
                    comm.Parameters.AddWithValue("_sort_dir", sortDir);
                    comm.Parameters.AddWithValue("_usertype_id", userTypeId);
                    comm.Parameters.AddWithValue("_seller_id", sellerId);
                    comm.Parameters.AddWithValue("_note", note);
                    comm.Parameters.AddWithValue("_min_login_count", minLoginCount);
                    comm.Parameters.AddWithValue("_max_login_count", maxLoginCount);
                    comm.Parameters.AddWithValue("_origin_id", originId);
                    comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);
                    comm.Parameters.AddWithValue("_path", path);

                    comm.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Exports searches' administration statistics into a path to the DB server
        /// </summary>
        /// <param name="path">The path for the export to be saved. All other parameters are the same as the statistics' function parameters</param>
        public static void ExportStatSearches(string searchText, string sortBy, string sortDir, string path)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                using (NpgsqlCommand comm = new NpgsqlCommand("stat.export_searches_stat", conn))
                {
                    comm.CommandType = System.Data.CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("_search_text", searchText);
                    comm.Parameters.AddWithValue("_sort_by", sortBy);
                    comm.Parameters.AddWithValue("_sor_dir", sortDir);
                    comm.Parameters.AddWithValue("_path", path);

                    comm.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Exports logins' administration statistics into a path to the DB server;
        /// </summary>
        /// <param name="path">The path for the export to be saved. All other parameters are the same as the statistics' function parameters</param>
        public static void ExportStatLogins(string searchText, DateTime dateFrom, DateTime dateTo, int browserId, int currentSellerId, string orderBy, string orderDir, string path)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                using (NpgsqlCommand comm = new NpgsqlCommand("stat.export_logins_stat", conn))
                {
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("_search_text", searchText);
                    comm.Parameters.AddWithValue("_date_from", dateFrom);
                    comm.Parameters.AddWithValue("_date_to", dateTo);
                    comm.Parameters.AddWithValue("_browser_id", browserId);
                    comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);
                    comm.Parameters.AddWithValue("_sort_by", orderBy);
                    comm.Parameters.AddWithValue("_sort_dir", orderDir);
                    comm.Parameters.AddWithValue("_path", path);

                    comm.ExecuteNonQuery();
                }
            }
        }

        #endregion

        public static string[] GetClassifiersUsage(string formName, int clientId, string text, DateTime dateFrom, DateTime dateTo)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_classifiers_usage", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_form", formName);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_text", text);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);

                return (string[])comm.ExecuteScalar();
            }
        }
    }
}
