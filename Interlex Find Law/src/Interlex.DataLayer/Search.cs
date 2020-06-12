using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Npgsql;
using Newtonsoft.Json;


namespace Interlex.DataLayer
{
    public partial class DB
    {
        //public static IEnumerable<IDataRecord> GetSearchList(int searchId, Guid[] classifierFilters, string sortBy, string sortDir, int page, int pageSize, int userId)
        //{
        //    using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
        //    {
        //        conn.Open();
        //        NpgsqlCommand comm = new NpgsqlCommand("get_search_list", conn);
        //        comm.CommandType = System.Data.CommandType.StoredProcedure;

        //        comm.Parameters.AddWithValue("_search_id", searchId);
        //        if (classifierFilters == null || classifierFilters.Length == 0)
        //            comm.Parameters.AddWithValue("_class_filter_ids", DBNull.Value);
        //        else
        //            comm.Parameters.AddWithValue("_class_filter_ids", classifierFilters);
        //        comm.Parameters.AddWithValue("_sort", sortBy);
        //        comm.Parameters.AddWithValue("_sort_dir", sortDir);
        //        comm.Parameters.AddWithValue("_page", page);
        //        comm.Parameters.AddWithValue("_page_size", pageSize);
        //        comm.Parameters.AddWithValue("_user_id", userId);

        //        using (NpgsqlDataReader reader = comm.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                yield return reader;
        //            }
        //        }
        //    }
        //}

        public static IEnumerable<IDataRecord> GetSearchList(int searchId, int searchSource, Guid[] classifierFilters, string sortBy, string sortDir, int page, int pageSize, int userId, int siteLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_search_list_2", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("_search_id", searchId);
                comm.Parameters.AddWithValue("_search_source", searchSource);
                if (classifierFilters == null || classifierFilters.Length == 0)
                    comm.Parameters.AddWithValue("_class_filter_ids", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_class_filter_ids", classifierFilters);
                comm.Parameters.AddWithValue("_sort", sortBy);
                comm.Parameters.AddWithValue("_sort_dir", sortDir);
                comm.Parameters.AddWithValue("_page", page);
                comm.Parameters.AddWithValue("_page_size", pageSize);
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

        public static DataRow GetSearchStatistics(int clientId, string searchText, DateTime from, DateTime to, int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_search_statistics_area", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_from_date", from);
                comm.Parameters.AddWithValue("_to_date", to);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static int GetSearchListCount(int searchId, Guid[] classifierFilters)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_search_list_count_2", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_id", searchId);
                if (classifierFilters == null || classifierFilters.Length == 0)
                    comm.Parameters.AddWithValue("_class_filter_ids", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_class_filter_ids", classifierFilters);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        public static int AddSearch(int userId, int sessionId, int siteSearchId, string searchBoxFiltersJSON, ref Dictionary<int, List<int>> searchTextIds, int siteLangId, byte[] searchIds, bool searchPerformed, int? docLangId, int? toDocParId, int[] linkId, out int totalCount)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_session_id", sessionId);
                comm.Parameters.AddWithValue("_site_search_id", siteSearchId);
                comm.Parameters.AddWithValue("_search_filters_json", searchBoxFiltersJSON);
                if (searchTextIds != null && searchTextIds.Count > 0)
                {
                    var searchTextIdsToPass = new StringBuilder();
                    foreach (var item in searchTextIds)
                    {
                        // opening current element
                        searchTextIdsToPass.Append("(");
                        searchTextIdsToPass.Append(item.Key); // key
                        // consolidated array start
                        searchTextIdsToPass.Append(",");

                        if (item.Value.Count > 0)
                        {
                            searchTextIdsToPass.Append("'{");
                            searchTextIdsToPass.Append(String.Join(",", item.Value));
                            searchTextIdsToPass.Append("}'");
                        }
                        else
                        {
                            searchTextIdsToPass.Append("null");
                        }
                        
                        // conslidated array end;
                        searchTextIdsToPass.Append("),"); // closing current element
                    }
                    searchTextIdsToPass.Remove(searchTextIdsToPass.Length - 1, 1); // trimming last comma; sry :D
                    // ending whole object
                    //   comm.Parameters.AddWithValue("_search_text_ids", "(" + String.Join("),(", searchTextIds) + ")");

                    comm.Parameters.AddWithValue("_search_text_ids", searchTextIdsToPass.ToString());
                }
                else
                {
                    comm.Parameters.AddWithValue("_search_text_ids", DBNull.Value);
                }
                   
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_search_ids", searchIds);
                comm.Parameters.AddWithValue("_search_performed", searchPerformed);
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_to_doc_par_id", toDocParId);
                comm.Parameters.AddWithValue("_link_id", linkId);

                totalCount = -1;
                int searchId = -1;
                bool refreshSearchIds = false;
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        searchId = Convert.ToInt32(reader["search_id"]);
                        totalCount = Convert.ToInt32(reader["total_count"]);
                        refreshSearchIds = Convert.ToBoolean(reader["refresh_search_ids"]);
                    }
                }

                if (refreshSearchIds) // PAL+FTI search
                {
                    int[] ids = DB.GetSearchAllIds(searchId);
                    byte[] bytes = new byte[ids.Length * sizeof(int)];
                    Buffer.BlockCopy(ids, 0, bytes, 0, bytes.Length);

                    DB.SetSearchBytes(searchId, bytes);
                }

                return searchId;
            }
        }

        public static int[] GetSearchAllIds(int searchId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_search_all_ids", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("_search_id", searchId);

                return (int[])comm.ExecuteScalar();
            }
        }

        public static void SetSearchBytes(int searchId, byte[] bytes)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_search_bytes", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("_search_id", searchId);
                comm.Parameters.AddWithValue("_search_ids", bytes);

                comm.ExecuteNonQuery();
            }
        }

        public static int[] GetPALSearchResult(int[] docLangIds, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_pal_search_result", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                //comm.Parameters.AddWithValue("_doc_lang_ids", "(" + String.Join("),(", docLangIds) + ")");
                comm.Parameters.AddWithValue("_doc_lang_ids", docLangIds);
                comm.Parameters.AddWithValue("_product_id", productId);

                return (int[])comm.ExecuteScalar();
            }
        }

        public static int[] GetReferedActECHRSearchResult(int[] docLangIds)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_refered_act_echr_search_result", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_ids", docLangIds);

                return (int[])comm.ExecuteScalar();
            }
        }

        //public static IEnumerable<IDataRecord> GetFilterClassifierTypes(int searchId, List<Tuple<int, string>> classifierFilters)
        //{
        //    using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
        //    {
        //        conn.Open();
        //        NpgsqlCommand comm = new NpgsqlCommand("get_filter_classifier_types", conn);
        //        comm.CommandType = System.Data.CommandType.StoredProcedure;
        //        comm.Parameters.AddWithValue("_search_id", searchId);
        //        if (classifierFilters == null || classifierFilters.Count == 0)
        //            comm.Parameters.AddWithValue("_class_filter_ids", DBNull.Value);
        //        else
        //            comm.Parameters.AddWithValue("_class_filter_ids", JsonConvert.SerializeObject(classifierFilters));

        //        using (NpgsqlDataReader reader = comm.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                yield return reader;
        //            }
        //        }
        //    }
        //}

        public static IEnumerable<IDataRecord> GetFilterClassifier(int searchId, Guid? parent, int classifierTypeId, int langId, Guid[] classifierFiltersIds)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_filter_classifier", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_id", searchId);
                comm.Parameters.AddWithValue("_parent_id", parent);
                comm.Parameters.AddWithValue("_classifier_type_id", classifierTypeId);
                comm.Parameters.AddWithValue("_lang_id", langId);
                if (classifierFiltersIds.Length == 0)
                    comm.Parameters.AddWithValue("_class_filter_ids", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_class_filter_ids", classifierFiltersIds);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetFilterClassifiers(Guid[] selectedClassifiers, int langId, string docClassifiersJSON)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_filter_classifiers", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_selected_classifiers", selectedClassifiers);
                comm.Parameters.AddWithValue("_lang_id", langId);
                comm.Parameters.AddWithValue("_doc_classifiers", docClassifiersJSON);
                
                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int[] GetSearchDocLangIds(int searchId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_search_doc_lang_ids", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_id", searchId);
                
                return (int[])comm.ExecuteScalar();
            }
        }

        public static byte[] GetSearchDocLangIdsBytes(int searchId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_search_ids_as_bytes", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_search_id", searchId);

                return (byte[])comm.ExecuteScalar();
            }
        }

        public static void DelSearch(int searchId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_search", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", searchId);

                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetMultiDictSearchItems(int langId, string searchText, string leadingCharacter)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_multidict_search_items", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_lang_id", langId);
                comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_leading_character", leadingCharacter);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetMultiDictTranslations(string itemId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_multidict_item_translations", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_item_id", itemId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        /// <summary>
        /// Gets letters for all languages for the multidictionary.
        /// </summary>
        /// <returns>Letter with corresponding lang_id per row and a boolean for avaiable occurencies in the dictionary</returns>
        public static IEnumerable<IDataRecord> GetMultiDictAlphabetLetters()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                NpgsqlCommand comm = new NpgsqlCommand("get_multidict_alphabet_letters", conn);
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

        public static IEnumerable<IDataRecord> GetAllMultiDictItems()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                NpgsqlCommand comm = new NpgsqlCommand("get_all_multidict_items", conn);
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
    }
}
