using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Npgsql;
using Interlex.DataLayer.Models.DataReader;

namespace Interlex.DataLayer
{
    public partial class DB
    {
        public static DataRow GetDocByDocNumber(string docNumber, int langId, int? userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_document_by_doc_number", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_lang_id", langId);
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

        public static DataRow GetDocument(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_document", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static DataRow GetDocByDocIdentifier(string guid)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_document_by_doc_identifier", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_identifier", guid);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static void AddRecentDoc(int userId, int docId, int maxCount, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_recent_doc", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_doc_id", docId);
                comm.Parameters.AddWithValue("_max_count", maxCount);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.ExecuteNonQuery();
            }
        }

        public static void AddOpenedDoc(int userId, int docId, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.add_opened_doc", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_doc_lang_id", docId);
                comm.Parameters.AddWithValue("_product_id", productId);

                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetRecentDocs(int userId, int siteLangId, bool? pinned, int? docType, int period, string orderBy, string orderDir, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_recent_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_pinned", (pinned.HasValue) ? pinned.Value : (bool?)null);
                comm.Parameters.AddWithValue("_doc_type", (docType.HasValue) ? docType.Value : (int?)null);
                comm.Parameters.AddWithValue("_period", period);
                comm.Parameters.AddWithValue("_order_by", orderBy);
                comm.Parameters.AddWithValue("_sort_dir", orderDir);
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

        public static IEnumerable<IDataRecord> GetNewEuFinsDocuments(int siteLangId, int userId, DateTime startDate, DateTime endDate)
        {
            var sequence = ExecuteReader.Yield(
                    connectionString: connPG,
                    functionName: "get_new_eufins_docs",
                    parameterBag: new
                    {
                        _site_lang_id = siteLangId,
                        _start_date = startDate,
                        _end_date = endDate,
                        _user_id = userId
                    }
                );

            return sequence;
        }

        public static IEnumerable<IDataRecord> GetEuFinsDocumentsPeriods(int siteLangId)
        {
            var sequence = ExecuteReader.Yield(
                    connectionString: connPG,
                    functionName: "get_eufins_documents_date_of_effect",
                    parameterBag: new { _site_lang_id = siteLangId }
                );

            return sequence;
        }

        public static IEnumerable<IDataRecord> GetNewDocs(int userId, int siteLangId, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_new_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
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


        public static void SetRecentDocPin(int id, bool pinned)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_recent_doc_pin", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);
                comm.Parameters.AddWithValue("_pinned", pinned);
                comm.ExecuteNonQuery();
            }
        }

        public static IEnumerable<IDataRecord> GetAllDocLangIds()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_all_doc_lang_ids", conn);
                comm.CommandType = CommandType.StoredProcedure;

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static string GetDocText(int docLangId, bool plainXml, int productId, bool showFreeDocuments)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_text", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                comm.Parameters.AddWithValue("_id", docLangId);
                comm.Parameters.AddWithValue("_plain_xml", plainXml);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_show_free_documents", showFreeDocuments);
                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetDocTextTest(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_text_test", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                comm.Parameters.AddWithValue("_id", docLangId);
                comm.Parameters.AddWithValue("_plain_xml", false);
                comm.Parameters.AddWithValue("_product_id", 1);
                comm.Parameters.AddWithValue("_show_free_documents", true);
                return comm.ExecuteScalar().ToString();
            }
        }

        /// <summary>
        /// Get full xml document from database
        /// </summary>
        /// <param name="docId">Document id</param>
        /// <param name="plainXml">Indicates if additional processing should be applyed. If false original document xml is returned.</param>
        /// <returns></returns>
        public static string GetDocText(int docLangId, bool plainXml, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_text", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", docLangId);
                comm.Parameters.AddWithValue("_plain_xml", plainXml);
                comm.Parameters.AddWithValue("_product_id", productId);
                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetDocText(int docLangId, bool plainXml)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_text", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", docLangId);
                comm.Parameters.AddWithValue("_plain_xml", plainXml);
                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetDocText(int docId, int productId)
        {
            return GetDocText(docId, false, productId);
        }

        public static DataRow GetParText(string linkType, string docNumber, string toPar, int? LangIdFromDoc, int? userId, int siteLangId, bool lastCons, bool lastConsWithText)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_par_text", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_link_type", linkType);
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_to_par", toPar);
                if (LangIdFromDoc.HasValue)
                    comm.Parameters.AddWithValue("_lang_id_from_doc", LangIdFromDoc.Value);
                else
                    comm.Parameters.AddWithValue("_lang_id_from_doc", DBNull.Value);
                if (userId.HasValue)
                    comm.Parameters.AddWithValue("_user_id", userId.Value);
                else
                    comm.Parameters.AddWithValue("_user_id", DBNull.Value);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_last_cons", lastCons);
                comm.Parameters.AddWithValue("_last_cons_with_text", lastConsWithText);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static DataRow GetDocHintByIdentifier(string identifier, string toPar)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_hint_by_identifier", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_identifier", identifier);
                if (!String.IsNullOrEmpty(toPar))
                    comm.Parameters.AddWithValue("_to_par", toPar);
                else
                    comm.Parameters.AddWithValue("_to_par", DBNull.Value);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static string GetDocNumberByDocLangId(int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_number_by_doc_lang_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", langId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string CheckEID(int docLangId, string eid, string num)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("check_for_eid", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_eid", eid);
                comm.Parameters.AddWithValue("_num", num);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static DataRow GetEIDfromToPar(int docLangId, string toPar)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_par_from_topar", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_to_par", toPar);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }

                return null;
            }
        }

        public static DataRow GetDocLinksTitle(int docLangId, int? toParId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_links_title", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_to_par_id", toParId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }

                return null;
            }
        }

        public static DataRow GetDocLinksTitle(string docNumber, string toPar, int siteLangId, int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_links_title", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_to_par", toPar);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_user_id", userId);


                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Used in Api Controller
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="toPar"></param>
        /// <param name="filterDomain"></param>
        /// <param name="userId"></param>
        /// <param name="siteLangId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IEnumerable<IDataRecord> GetDocInLinks(string docNumber, string toPar, string filterDomain, int? userId, int siteLangId, int limit)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_in_links", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                if (String.IsNullOrWhiteSpace(toPar))
                    comm.Parameters.AddWithValue("_to_par", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_to_par", toPar);
                comm.Parameters.AddWithValue("_filter_domain", filterDomain);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_limit", limit);

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
        /// Used in site
        /// </summary>
        /// <param name="toDocLangId"></param>
        /// <param name="toParId"></param>
        /// <param name="filterDomain"></param>
        /// <param name="userId"></param>
        /// <param name="siteLangId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IEnumerable<IDataRecord> GetDocInLinks(int toDocLangId, int? toParId, string filterDomain, int? userId, int siteLangId, int limit)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_in_links", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", toDocLangId);
                comm.Parameters.AddWithValue("_to_par_id", toParId);
                comm.Parameters.AddWithValue("_filter_domain", filterDomain);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_limit", limit);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetDocInLinks(int toDocLangId, int? toParId, int productId, int[] linkIds, string subTitle, bool showFreeDocuments)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_in_links", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", toDocLangId);
                comm.Parameters.AddWithValue("_to_par_id", toParId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_link_ids", linkIds);
                comm.Parameters.AddWithValue("_article_subchar", subTitle);
                comm.Parameters.AddWithValue("_show_free_documents", showFreeDocuments);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetDocInLinks(string docNumber, string toPar, string domain, int userId, int langId, int limit)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_in_links", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_to_par", toPar);
                comm.Parameters.AddWithValue("_filter_domain", domain);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", langId);
                comm.Parameters.AddWithValue("_limit", limit);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetDocListByIds(int[] docLangIds, string filterDomain)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_list_by_ids", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_ids", docLangIds);
                comm.Parameters.AddWithValue("_filter_domain", filterDomain);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetDocContents(int docLangId)
        {
            using (var connenction = new NpgsqlConnection(connPG))
            {
                connenction.Open();

                var command = new NpgsqlCommand(cmdText: "get_doc_contents_info", connection: connenction);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@_doc_lang_id", docLangId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        [Obsolete("Use GetDocContents")]
        public static IEnumerable<IDataRecord> _GetDocContents(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_contents", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetDocLangs(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_langs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int GetDocLanguageByDocLangId(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_language_by_doc_lang_id", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                return (int)comm.ExecuteScalar();
            }
        }

        public static DataRow GetLinkInfo(string docNumber, string toPar, int? userId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_link_info", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_to_par", toPar);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static IEnumerable<IDataRecord> GetDocVersions(int docLangId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_versions", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetJudgmentRelatedDocs(int docLangId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_judgment_related_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static int GetActProvisionsCorrectDocLangId(string docNumber, string numSimple)
        {
            if (numSimple.Contains("."))
            {
                numSimple = numSimple.Split('.')[0];
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                using (NpgsqlCommand comm = new NpgsqlCommand("get_act_provisions_correct_doc_lang_id", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("_doc_number", docNumber);
                    comm.Parameters.AddWithValue("_num_simple", numSimple);

                    return Convert.ToInt32(comm.ExecuteScalar());
                }
            }
        }

        public static IEnumerable<IDataRecord> GetActProvisionsLinkedByCases(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_act_provisions_linked_by_cases", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetActProvisionsLinkedByCases(int docLangId, string numSimple)
        {
            if (numSimple.Contains("."))
            {
                numSimple = numSimple.Split('.')[0];
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_act_provisions_linked_by_cases", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_num_simple", numSimple);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }


        public static IEnumerable<IDataRecord> GetDocArticleProvisions(int docLangId, string eid, int productId, bool showFreeDocuments)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_article_provisions", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_eid", eid);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_show_free_documents", showFreeDocuments);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetPALDocList(int[] docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_pal_doc_list", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_ids", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetReferedActECHRDocList(int[] docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_refered_act_echr_doc_list", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_ids", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static string GetDocLangIdByIdentifier(string docIdentifier)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_lang_id_by_identifier", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_identifier", docIdentifier);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetDocMeta(string docNumber, int langIdFromDoc, int siteLangId, int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_meta", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_lang_id_from_doc", langIdFromDoc);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_user_id", userId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static int? GetLastConsDocLangId(string docNumber, int langIdFromDoc, int siteLangId, int userId, bool withText)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_last_cons_doc_lang_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_number", docNumber);
                comm.Parameters.AddWithValue("_from_doc_lang_id", langIdFromDoc);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_with_text", withText);

                return comm.ExecuteScalar() as int?;
            }
        }

        public static int[] GetDocProducts(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_products", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                return (int[])comm.ExecuteScalar();
            }
        }

        public static IEnumerable<IDataRecord> GetDocConsVersions(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_cons_versions", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static bool IsDemoDoc(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("is_demo_doc", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }

        public static bool CheckDocumentIsClassifiedBy(int docLangId, string classifierIdentifier)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("check_document_is_classified_by", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_classifier_identifier", classifierIdentifier);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }

        public static int AddDemoDocs(int[] docLangIds)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_demo_docs", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_ids", docLangIds);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        public static IEnumerable<IDataRecord> GetRefDocsArticle(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_all_article_ref_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_to_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_to_doc_par_ids", docParIds);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static string GetDocParByParId(int docParId, int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_par_text_by_doc_par_id", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_par_id", docParId);
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                return Convert.ToString(comm.ExecuteScalar());
            }
        }

        public static int GetDocParIdByEid(string articleEid, int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_par_id_by_eid", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_article_eid", articleEid);
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                return Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        public static string GetDocumentNumberByDocLangId(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_document_number_by_doc_lang_id", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_lang_id", docLangId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetDocArtTextByDocParId(int docParId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_doc_art_text_by_doc_par_id", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_par_id", docParId);

                return Convert.ToString(comm.ExecuteScalar());
            }
        }

        //get_article_old_eu_ref_docs

        public static IEnumerable<IDataRecord> GetOldArticleEuRefDocs(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_article_old_eu_ref_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_to_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_to_doc_par_ids", docParIds);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetOldArticleFinsRefDocs(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_article_old_fins_ref_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_to_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_to_doc_par_ids", docParIds);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetNewArticleRefDocs(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_article_new_ref_docs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_to_doc_lang_id", docLangId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_to_doc_par_ids", docParIds);
                comm.Parameters.AddWithValue("_lang_id", langId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static string GetPathToArticle(int docParId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_path_to_article", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_doc_par_id", docParId);
                return Convert.ToString(comm.ExecuteScalar());
            }
        }

        public static IEnumerable<IDataRecord> GetArticlesCorrelation(int userId, int siteLangId, int docParId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_articles_correlation", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_site_lang_id", siteLangId);
                comm.Parameters.AddWithValue("_doc_par_id", docParId);
                comm.Parameters.AddWithValue("_lang_id", langId);


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
