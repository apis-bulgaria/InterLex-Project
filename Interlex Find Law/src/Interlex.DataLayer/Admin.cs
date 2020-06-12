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
        public static IEnumerable<IDataRecord> GetOpenedDocuments(string documentTitle, int userId, int clientId, int sellerId, string note, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir, int currentSellerId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_adm", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_document_title", documentTitle);
                comm.Parameters.AddWithValue("_user_id", userId);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_start", start);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);
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

        public static IEnumerable<IDataRecord> GetOpenedDocumentsCounts(string username, string clientName, int sellerId, string note, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir, int currentSellerId, int productId) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_counts_adm", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_client_name", clientName);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_start", start);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_orderby", orderby);
                comm.Parameters.AddWithValue("_orderdir", orderdir);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);
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

        public static IEnumerable<IDataRecord> GetOpenedDocumentsNullCounts(string username, string clientName, string sellerName, string note, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_null_counts_adm", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_client_name", clientName);
                comm.Parameters.AddWithValue("_seller_name", sellerName);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_start", start);
                comm.Parameters.AddWithValue("_take", take);
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

        public static int GetOpenedDocumentsCount(string documentTitle, string username, string clientName, string sellerName, string note, DateTime dateFrom, DateTime dateTo) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_adm_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_document_title", documentTitle);
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_client_name", clientName);
                comm.Parameters.AddWithValue("_seller_name", sellerName);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static int GetOpenedDocumentsCountsCount(string username, string clientName, string sellerName, string note, DateTime dateFrom, DateTime dateTo)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("stat.get_opened_docs_counts_adm_count", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_username", username);
                comm.Parameters.AddWithValue("_client_name", clientName);
                comm.Parameters.AddWithValue("_seller_name", sellerName);
                comm.Parameters.AddWithValue("_note", note);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);

                return int.Parse(comm.ExecuteScalar().ToString());
            }
        }

        public static IEnumerable<IDataRecord> GetOpenedDocuments(int start, int take)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_opened_docs_adm", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_start", start);
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

        public static IEnumerable<IDataRecord> GetOrdersAdm(int orderId, int clientId, int sellerId, int statusTypeId, int applyTypeId, string promoCode, DateTime dateFrom, DateTime dateTo, string orderBy, string orderDir, int skip, int take, string searchText, int paymentTypeId, int currentSellerId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("get_orders_adm", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_order_id", orderId);
                comm.Parameters.AddWithValue("_client_id", clientId);
                comm.Parameters.AddWithValue("_seller_id", sellerId);
                comm.Parameters.AddWithValue("_status_type_id", statusTypeId);
                comm.Parameters.AddWithValue("_apply_type_id", applyTypeId);
                comm.Parameters.AddWithValue("_promo_code", promoCode);
                comm.Parameters.AddWithValue("_date_from", dateFrom);
                comm.Parameters.AddWithValue("_date_to", dateTo);
                comm.Parameters.AddWithValue("_order_by", orderBy);
                comm.Parameters.AddWithValue("_order_dir", orderDir);
                comm.Parameters.AddWithValue("_start", skip);
                comm.Parameters.AddWithValue("_take", take);
                comm.Parameters.AddWithValue("_search_text", searchText);
                comm.Parameters.AddWithValue("_payment_type_id", paymentTypeId);
                comm.Parameters.AddWithValue("_current_seller_id", currentSellerId);

                using (NpgsqlDataReader dr = comm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        yield return dr;
                    }
                }
            }
        }

        public static string GetInvoiceDetailsJSON(int orderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("sales.get_order_invoice_details", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_order_id", orderId);

                return comm.ExecuteScalar().ToString();
            }
        }

        public static IEnumerable<IDataRecord> GetOrderStatusHistory(int orderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("sales.get_order_status_history", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_order_id", orderId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetOrderVatCheckEu(int orderId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("sales.get_vat_check_eu", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("_order_id", orderId);

                using (NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public static IEnumerable<IDataRecord> GetOrderApplyTypes()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("sales.get_order_apply_types", conn);
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

        public static IEnumerable<IDataRecord> GetOrderStatusTypes()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connPG))
            {
                conn.Open();
                NpgsqlCommand comm = new NpgsqlCommand("sales.get_order_status_types", conn);
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
    }
}
