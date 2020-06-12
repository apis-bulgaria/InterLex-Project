using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using Interlex.DataLayer.Models;

namespace Interlex.DataLayer
{
    public partial class DB
    {
        private static string connPG = ConfigurationManager.ConnectionStrings["ConnPG"].ConnectionString;

        public static void ExecSQL(string sql)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                comm.ExecuteNonQuery();
            }
        }

        public static DataTable GetDataTableFromDataReader(IDataReader dataReader)
        {
            DataTable schemaTable = dataReader.GetSchemaTable();
            DataTable resultTable = new DataTable();

            foreach (DataRow dataRow in schemaTable.Rows)
            {
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = dataRow["ColumnName"].ToString();
                dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
              //  dataColumn.ReadOnly = Convert.ToBoolean(dataRow["IsReadOnly"]);
               // dataColumn.AutoIncrement = Convert.ToBoolean(dataRow["IsAutoIncrement"]);
               // dataColumn.Unique = Convert.ToBoolean(dataRow["IsUnique"]);

                resultTable.Columns.Add(dataColumn);
            }

            while (dataReader.Read())
            {
                DataRow dataRow = resultTable.NewRow();
                for (int i = 0; i < resultTable.Columns.Count; i++)
                {
                    dataRow[i] = dataReader[i];
                }
                resultTable.Rows.Add(dataRow);
            }

            return resultTable;
        }

        public static IEnumerable<IDataRecord> SearchCourtActs()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("search_court_acts", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                //comm.Parameters.AddWithValue("_id", id);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        //public static DataTable GetDocument(int id)
        //{
        //    using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
        //    {
        //        conn.Open();
        //        NpgsqlCommand comm = new NpgsqlCommand("get_document", conn);
        //        comm.CommandType = System.Data.CommandType.StoredProcedure;
        //        comm.Parameters.AddWithValue("_id", id);

        //        using (NpgsqlDataReader reader = comm.ExecuteReader())
        //        {
        //            return GetDataTableFromDataReader(reader);
        //        }
        //    }
        //}

        public static DataTable GetDocument(int id, int userId, int productId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_document", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_product_id", productId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    return GetDataTableFromDataReader(reader);
                }
            }
        }

        public static DataTable GetLang(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_lang", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_id", id);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    return GetDataTableFromDataReader(reader);
                }
            }
        }


        public static void ClearOldSessions()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("clear_old_sessions", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.ExecuteNonQuery();
            }
        }

        public static int AddSession(int userId, string ipAddr)
        {
            int sessionId = -1;
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_session", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_ip_addr", ipAddr);

                sessionId = Convert.ToInt32(comm.ExecuteScalar());
            }
            return sessionId;
        }

        public static void DelSession(int sessionId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("del_session", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_session_id", sessionId);
                comm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Manage session object in the DB.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="timeoutSeconds">Web server session timeout in seconds</param>
        /// <returns>true: the session last_access is updated; false: session does not exists or is just deleted.</returns>
        public static bool UpdateSession(int sessionId, int timeoutSeconds)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("update_session", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_session_id", sessionId);
                comm.Parameters.AddWithValue("_timeoutSeconds", timeoutSeconds);

                bool result = Convert.ToBoolean(comm.ExecuteScalar());
                return result;
            }
        }

        public static IEnumerable<IDataRecord> GetProducts()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_products", conn);
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

        public static IEnumerable<IDataRecord> GetProducts(int clientId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_products", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetLangs()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_langs", conn);
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

        public static IEnumerable<IDataRecord> GetInterfaceLangs()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_interface_langs", conn);
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


        public static void AddCookiesAgreement(int? userId, string ip)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("add_cookies_agreement", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_ip", ip);

                comm.ExecuteNonQuery();
            }
        }

        public static bool GetCookiesAgreement(int? userId, string ip)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_cookies_agreement", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_ip", ip);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }

        public static IEnumerable<IDataRecord> GetCountries(int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_countries", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
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

        public static IEnumerable<IDataRecord> GetEurLexArticles(int docLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_eurlex_articles", conn);
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

        public static DataRow GetCountry(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_country", conn);
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

        /// <summary>
        /// Gets all langs from db
        /// </summary>
        /// <returns>Returns IDataRecord collection ordered by site lang priority and then by ord from database</returns>
        public static IEnumerable<IDataRecord> GetAllLangs()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_all_langs", conn);
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

        public static string GetCountryNameByIdAndLang(int countryId, int langId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();

                NpgsqlCommand comm = new NpgsqlCommand("get_country_name_by_id_and_lang", conn);
                comm.Parameters.AddWithValue("_country_id", countryId);
                comm.Parameters.AddWithValue("_lang_id", langId);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                return comm.ExecuteScalar().ToString();
            }
        }

        public static string GetMaxDBVersion()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_max_db_version", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                return comm.ExecuteScalar().ToString();
            }
        }

        public static String[] GetLinksToPars(int[] linkIds)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_links_to_pars", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_link_ids", linkIds);

                return (String[]) comm.ExecuteScalar();
            }
        }

        public static string GetParNumByParId(int parId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_par_num_by_par_id", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_par_id", parId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static void InsertLawRelations(LawRelationUpdateModel[] updateModel)
        {
           // NpgsqlConnection.MapCompositeGlobally<LawRelationUpdateModel>("law_relation_update_model_test");
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("insert_law_relations", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_items", Newtonsoft.Json.JsonConvert.SerializeObject(updateModel));

                comm.ExecuteNonQuery();
            }
        }

        public static void ManipulateLawRelationsWithCSV(string path, bool isDelete)
        {
            string procedureName = isDelete ? "delete_law_relations_from_csv" : "import_law_relations_from_csv";

            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand(procedureName, conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_path", path);

                comm.ExecuteNonQuery();
            }
        }

        [Obsolete("Use ManipulateLawRelationsWithCSV with isDelete == false value instead")]
        public static void ImportLawRelationsFromCSV(string path)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("import_law_relations_from_csv", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_path", path);

                comm.ExecuteNonQuery();
            }
        }

        public static object ExecuteStoredProc(string procName, Dictionary<string, object> parameters, StoredProcedureExecuteType execType)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                // initialize
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand(procName, conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                // populating params
                foreach (var item in parameters)
                {
                    comm.Parameters.AddWithValue(item.Key, item.Value);
                }

                // execute
                if (execType == StoredProcedureExecuteType.NonQuery)
                {
                    comm.ExecuteNonQuery();
                    return null;
                }
                else if (execType == StoredProcedureExecuteType.Scalar)
                {
                    return comm.ExecuteScalar();
                }
                else // reader
                {
                    var list = new List<IDataRecord>();

                    using (NpgsqlDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader);
                        }
                    }

                    return list;
                }
            }
        }
    }
}
