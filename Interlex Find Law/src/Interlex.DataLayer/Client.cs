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
        //Used in real clients list in administration with enabled editing
        public static IEnumerable<IDataRecord> GetClients(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int sellerId, int originId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_clients", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(searchText))
                    comm.Parameters.AddWithValue("_search_text", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_start_from", startFrom);
                comm.Parameters.AddWithValue("_page_size", pageSize);
                comm.Parameters.AddWithValue("_sort_by", sortBy);
                comm.Parameters.AddWithValue("_sort_dir", sortDir);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_origin_id", originId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        //Used in administrations statistics
        public static IEnumerable<IDataRecord> GetClientsStats(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int sellerId, int currentSellerId, int originId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_clients_stat", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(searchText))
                    comm.Parameters.AddWithValue("_search_text", DBNull.Value);
                else
                    comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_start_from", startFrom);
                comm.Parameters.AddWithValue("_page_size", pageSize);
                comm.Parameters.AddWithValue("_sort_by", sortBy);
                comm.Parameters.AddWithValue("_sort_dir", sortDir);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);
                comm.Parameters.AddWithValue("_origin_id", originId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static DataRow GetClient(int clientId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_client", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    DataTable dt = GetDataTableFromDataReader(reader);
                    if (dt.Rows.Count == 1)
                        return dt.Rows[0];
                }
            }
            return null;
        }

        public static IEnumerable<IDataRecord> GetClientProducts(int clientId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_client_products", conn);
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

        public static int GetClientProductLicenseCount(int clientId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_client_product_license_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_product_id", productId);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static int GetClientProductAvaiableLicenseCount(int clientId, int productId, int userId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_client_product_avaiable_license_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_product_id", productId);
                comm.Parameters.AddWithValue("_user_id", userId);

                try
                {
                    return int.Parse(comm.ExecuteScalar().ToString());
                }
                catch (Exception) 
                {
                    return 0;
                }
            }
        }

        public static bool ClientHasUserFromSite(int clientId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("client_has_user_from_site", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_client_id", clientId);

                return Convert.ToBoolean(comm.ExecuteScalar());
            }
        }
    }
}
