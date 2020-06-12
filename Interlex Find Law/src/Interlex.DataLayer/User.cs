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
        public static DataRow GetUser(string username, string password, bool passHashed)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_id_by_username_password", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_password", password);
                comm.Parameters.AddWithValue("_pass_hashed", passHashed);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static DataRow GetUser(string email)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_id_by_email", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_email", email);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static DataRow GetUserByUsername(string username)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_by_username", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static DataRow GetUser(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static IEnumerable<IDataRecord> GetSessions(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, bool showDemoSessions)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_sessions", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(searchText))
                    comm.Parameters.AddWithValue("_search_text", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_start_from", startFrom);
                comm.Parameters.AddWithValue("_page_size", pageSize);
                comm.Parameters.AddWithValue("_sort_by", sortBy);
                comm.Parameters.AddWithValue("_sort_dir", sortDir);
                comm.Parameters.AddWithValue("_show_demo_sessions", showDemoSessions);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int GetTotalSessionsCount(string searchText, bool showDemoSessions)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_total_sessions_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_show_demo_sessions", showDemoSessions);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        //Used in administration's real users' list with enabled editing
        public static IEnumerable<IDataRecord> GetUsers(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int userTypeId, int sellerId, string note, int minLoginCount, int maxLoginCount, int originId, DateTime from, DateTime to)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_users", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(searchText))
                    comm.Parameters.AddWithValue("_search_text", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_search_text", searchText);

                comm.Parameters.AddWithValue("_start_from", startFrom);
                comm.Parameters.AddWithValue("_page_size", pageSize);
                comm.Parameters.AddWithValue("_sort_by", sortBy);
                comm.Parameters.AddWithValue("_sort_dir", sortDir);
                comm.Parameters.AddWithValue("_usertype_id", userTypeId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_min_login_count", minLoginCount);
                comm.Parameters.AddWithValue("_max_login_count", maxLoginCount);
                comm.Parameters.AddWithValue("_origin_id", originId);
                comm.Parameters.AddWithValue("_from_date", from);
                comm.Parameters.AddWithValue("_to_date", to);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }
        
        //Used in administration statistics
        public static IEnumerable<IDataRecord> GetUsersStat(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int userTypeId, int sellerId, string note, int minLoginCount, int maxLoginCount, int originId, int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_users_stat", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(searchText))
                    comm.Parameters.AddWithValue("_search_text", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_start_from", startFrom);
                comm.Parameters.AddWithValue("_page_size", pageSize);
                comm.Parameters.AddWithValue("_sort_by", sortBy);
                comm.Parameters.AddWithValue("_sort_dir", sortDir);
                comm.Parameters.AddWithValue("_usertype_id", userTypeId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_min_login_count", minLoginCount);
                comm.Parameters.AddWithValue("_max_login_count", maxLoginCount);
                comm.Parameters.AddWithValue("_origin_id", originId);
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

        public static IEnumerable<IDataRecord> GetUsers(int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_users", conn);
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

        public static int GetUsersCount(int userTypeId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_users_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_usertype_id", userTypeId);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static void DelUser(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_user", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.ExecuteNonQuery();
            }
        }

        public static void DelClient(int clientId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_client", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.ExecuteNonQuery();
            }
        }

        public static int SetUser(int mngUserId, int userId, int clientId, string username, string email, string password, int userTypeId,
            string fullName, bool pushSession, int maxLoginCount, int sessionTimeout, string code, string phone, string skypeName, int? countryId, int[,] products, string note, int? parentSellerId,
            bool active, int originId, bool emailValid, List<string> allowedIps)
        {
            if (userTypeId == 1 || userTypeId == 3 || userTypeId == 6 || userTypeId == 7)
            {
                clientId = 1; //APIS EUROPE
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_user", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_mng_user_id", mngUserId);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_email", email);
                comm.Parameters.AddWithValue("_password", password);
                comm.Parameters.AddWithValue("_usertype_id", userTypeId);
                comm.Parameters.AddWithValue("_fullname", fullName);
                comm.Parameters.AddWithValue("_push_sess", pushSession);
                comm.Parameters.AddWithValue("_max_login_count", maxLoginCount);
                comm.Parameters.AddWithValue("_session_timeout", sessionTimeout);
                comm.Parameters.AddWithValue("_code", code);
                comm.Parameters.AddWithValue("_phone", phone);
                comm.Parameters.AddWithValue("_skype_name", skypeName);
                comm.Parameters.AddWithValue("_country_id", countryId);
                comm.Parameters.AddWithValue("_prods", products);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_parent_seller_id", parentSellerId);
                comm.Parameters.AddWithValue("_active", active);
                comm.Parameters.AddWithValue("_origin_id", originId);
                comm.Parameters.AddWithValue("_email_valid", emailValid);
                comm.Parameters.AddWithValue("_allowed_ip", allowedIps.ToArray());
                int result = Convert.ToInt32(comm.ExecuteScalar());
                return result;
            }
        }

        public static int SetClient(int mngUserId, int clientId, string clientName, int? sellerId, int[] prodIds, int[] prodLic, DateTime[,] prodDates, bool[] prodDemo, string note, int countryId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_client", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_mng_user_id", mngUserId);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_client_name", clientName);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_prod_ids", prodIds);
                comm.Parameters.AddWithValue("_prod_lic", prodLic);
                comm.Parameters.AddWithValue("_prod_dates", prodDates);
                comm.Parameters.AddWithValue("_prod_demo", prodDemo);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_country_id", countryId);

                int result = Convert.ToInt32(comm.ExecuteScalar());
                return result;
            }
        }

        public static IEnumerable<IDataRecord> GetSellers(int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_sellers", conn);
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

        public static int GetSellerIdByName(string sellerName) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_seller_id_by_name", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_seller_name", sellerName);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static string GetSellerNameBySellerId(int sellerId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_seller_name_by_seller_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_seller_id", sellerId);

                return comm.ExecuteScalar().ToString();
            }
        }


        public static IEnumerable<IDataRecord> GetUserTypes()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_usertypes", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetUserProducts(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_products", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetClients(int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_clients", conn);
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

        public static bool ExistsUsername(string username) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("exists_username", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);

                bool result = Convert.ToBoolean(comm.ExecuteScalar());
                return result;
            }
        }

        public static bool ExistsEmail(string email)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("exists_email", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_email", email);

                bool result = Convert.ToBoolean(comm.ExecuteScalar());
                return result;
            }
        }

        public static bool ExistsEmail(string email, int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("exists_email", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_email", email);
                comm.Parameters.AddWithValue("_user_id", userId);

                bool result = Convert.ToBoolean(comm.ExecuteScalar());
                return result;
            }
        }

        public static bool ExistsSellerCode(string code, int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("exists_seller_code", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_code", code);
                comm.Parameters.AddWithValue("_user_id", userId);

                bool result = Convert.ToBoolean(comm.ExecuteScalar());
                return result;
            }
        }

        public static bool UserExists(int? userId, string username)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("username_exists", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_username", username);

                bool result = Convert.ToBoolean(comm.ExecuteScalar());
                return result;
            }
        }

        public static void SetUserLinksInNewTab(int userId, bool linksInNewTab)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_user_links_in_new_tab", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_links_in_new_tab", linksInNewTab);
                comm.ExecuteNonQuery();
            }
        }

        public static void UpdateUserCommonSettings(int userId, bool linksInNewTab, bool showFreeDocuments)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("update_user_common_settings", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_links_in_new_tab", linksInNewTab);
                comm.Parameters.AddWithValue("_show_free_documents", showFreeDocuments);
                comm.ExecuteNonQuery();
            }
        }

        public static void UserChangePassword(int userId, string password)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("user_change_password", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_password", password);
                comm.ExecuteNonQuery();
            }
        }

        public static bool UserChangeEmail(int userId, string email, string password) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("user_change_email", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_email", email);
                comm.Parameters.AddWithValue("_password", password);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }

        public static void UpdateLanguagePreferences(int userId, int[,] preferences)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("update_user_lang_preferences", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_preferences", preferences);
                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetLanguagePreferences(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_lang_preferences", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static void AddUserSearch(int userId, string searchText, string searchResultJSON, int maxCount, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_user_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_txt", searchText);
                comm.Parameters.AddWithValue("_search_obj", searchResultJSON);
                comm.Parameters.AddWithValue("_max_user_search_count", maxCount);
                comm.Parameters.AddWithValue("_product_id", productId);

                comm.ExecuteNonQuery();
            }
        }

        public static void DelUserSearch(int searchId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_user_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_id", searchId);
                comm.ExecuteNonQuery();
            }
        }

        public static void DelAllUserSearches(int userId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_all_user_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.ExecuteNonQuery();
            }
        }

        public static void DelAllUserRecentDocuments(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_all_user_recent_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetUserSearches(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetUserSearches(int userId, string like)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_like", like);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetUserSearches(int userId, string like, int period)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_like", like);
                comm.Parameters.AddWithValue("_period", period);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetUserSearches(int userId, string like, int period, int type)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_like", like);
                comm.Parameters.AddWithValue("_period", period);
                comm.Parameters.AddWithValue("_type", type);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetUserSearches(int userId, string like, int period, int type, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_searches", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_like", like);
                comm.Parameters.AddWithValue("_period", period);
                comm.Parameters.AddWithValue("_type", type);
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
        
        public static string GetUserSearchDetails(int searchId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_search_details", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_id", searchId);
                return comm.ExecuteScalar().ToString();
            }
        }

        public static void InsertPasswordReset(int userId, string code)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_password_reset", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_code", code);
                comm.ExecuteNonQuery();
            }
        }

        public static DataRow GetPasswordReset(string code)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_password_reset", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_code", code);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    try
                    {
                        DataTable dt = GetDataTableFromDataReader(reader);
                        return dt.Rows[0];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                }
            }
        }

        public static void UpdatePasswordResetExpiry(string code) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("update_password_reset_expiry_date", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_code", code);
                comm.ExecuteNonQuery();
            }
        }

        public static void AddUserDoc(int userId, int docLangId, int productId, int? folderId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_user_doc", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_folder_id", folderId);

                comm.ExecuteNonQuery();
            }
        }

        public static void DelUserDoc(int userId, int docLangId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_user_doc", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_product_id", productId);

                comm.ExecuteNonQuery();
            }
        }

        public static void DelAllUserDocs(int userId, int? folderId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_all_user_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_folder_id", folderId);

                comm.ExecuteNonQuery();
            }
        }

        public static int GetUserDocsCount(int userId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_docs_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_product_id", productId);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static bool GetUserHasDocument(int userId, int docLangId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_has_document", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_product_id", productId);

                return bool.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static int[] GetUserLangPrefForSearch(int userId, int siteLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_lang_pref_for_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);

                return (int[])comm.ExecuteScalar();
            }
        }

        public static IEnumerable<IDataRecord> GetUserDocs(int userId, int siteLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetUserDocsFolder(int userId, int siteLangId, string orderBy, string orderDir, int productId, int? folderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_docs_folder", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_order_by", orderBy);
                comm.Parameters.AddWithValue("_order_dir", orderDir);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_folder_id", folderId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static void AddUserRequest(string requestedPage, DateTime requestedOn, int time) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_user_request", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_requested_page", requestedPage);
                comm.Parameters.AddWithValue("_requested_on", requestedOn);
                comm.Parameters.AddWithValue("_time_ms", time);

                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetUserRequests(string orderBy, string orderDir, int take) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_requests", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_order_by", orderBy);
                comm.Parameters.AddWithValue("_order_dir", orderDir);
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

        public static IEnumerable<IDataRecord> GetSellerPromoCodes(int sellerId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("promo.get_seller_promo_codes", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_seller_id", sellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> NewSellerPromoCode(int sellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("promo.new_seller_promo_code", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetProductsList(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_products_list", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int AddAuthToken(int userId, string token, DateTime expire)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_auth_token", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_token", token);
                comm.Parameters.AddWithValue("_expire_date", expire);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTokenId"></param>
        /// <param name="token"></param>
        /// <returns>UserId</returns>
        public static int CheckAuthToken(int authTokenId, string token)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("check_auth_token", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_auth_token_id", authTokenId);
                comm.Parameters.AddWithValue("_token", token);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        public static void DelAuthToken(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_auth_token", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);

                comm.ExecuteNonQuery();
            }
        }

        public static void SetUserValidEMail(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_user_valid_email", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.ExecuteNonQuery();
            }
        }

        public static DataRow GetSeller(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_seller", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        // Gets all docs and folders of given user
        public static IEnumerable<IDataRecord> GetUserFoldersParent(int userId, int productId, int? parentFolderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_folders_parent", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_parent_id", parentFolderId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static DataRow AddUserFolder(int userId, int productId, string folderName, int? parentId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_user_folder", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_productId", productId);
                comm.Parameters.AddWithValue("_folder_name", folderName);
                comm.Parameters.AddWithValue("_parent_id", parentId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }

            return null;
        }

        // Returns false if doc already exists in folder
        public static bool AddUserDocToFolder(int userDocId, int folderId, int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_user_doc_to_folder", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_doc_id", userDocId);
                comm.Parameters.AddWithValue("_folder_id", folderId);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }

        // Returns false if folder was not found
        public static bool RenameUserFolder(int userId, int folderId, string newFolderName)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("rename_user_folder", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_folder_id", folderId);
                comm.Parameters.AddWithValue("_new_folder_name", newFolderName);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }

        public static void DeleteUserFolder(int userId, int folderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("delete_user_folder", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_folder_id", folderId);

                comm.ExecuteNonQuery();
            }
        }

        public static int GetUserDocsCountFolder(int userId, int productId, int? folderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_docs_count_folder", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_folder_id", folderId);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        public static void MoveDocsFolderToFolder(int userId, int productId, int? fromId, int? toId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("move_docs_folder_to_folder", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_from_id", fromId);
                comm.Parameters.AddWithValue("_to_id", toId);

                comm.ExecuteNonQuery();
            }
        }

        public static DataRow GetUserInformationRemote(string username)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_user_information_remote", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Helper function that extracts json objects from saved user searches. Criteria for select are written in the procedure itself.
        /// </summary>
        public static IEnumerable<IDataRecord> GetUserSearchesMultiDictObjects()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand comm = new NpgsqlCommand("get_user_searches_multidict_objects", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    using (NpgsqlDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return reader;
                        }
                    }
                }
            }
        }

        public static void UpdateUserSearchesMultiDictObjects(string[,] updateModel)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                using (NpgsqlCommand comm = new NpgsqlCommand("update_user_searches_multidict_objects", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    comm.Parameters.AddWithValue("_update_model", updateModel);
                    comm.ExecuteNonQuery();
                }
            }
        }
        
        public static string AddUserValidateToken(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_user_validate_token", conn);
                comm.CommandType = CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("_user_id", userId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static bool ActivateUser(Guid token)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("activate_user", conn);
                comm.CommandType = CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("_token", token);

                return (bool)comm.ExecuteScalar();
            }
        }
    }
}
