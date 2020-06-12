using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Npgsql;

namespace Interlex.DataLayer
{
    public partial class DB
    {
        public static IEnumerable<IDataRecord> GetFolders(int productId, int siteLangId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_folders", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_product_id", productId);
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

        public static DataRow GetFolder(int folderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_folder", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_folder_id", folderId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static IEnumerable<IDataRecord> GetFolderLangs(int folderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_folder_langs", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
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

        public static void SetFolderDocsCount(int folderId, int docsCount)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("set_folder_docs_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_folder_id", folderId);
                comm.Parameters.AddWithValue("_docs_count", docsCount);
                comm.ExecuteNonQuery();
            }
        }
    }
}
