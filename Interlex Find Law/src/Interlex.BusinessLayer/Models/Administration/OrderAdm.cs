using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.BusinessLayer.Enums;
using Interlex.BusinessLayer.Helpers;
using Interlex.DataLayer;
using System.Data;

namespace Interlex.BusinessLayer.Models
{
    public class OrderAdm
    {
        public int OrderId { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int? SellerId { get; set; }

        public string SellerName { get; set; }

        public int StatusTypeId { get; set; }

        public OrderStatusTypes StatusType { get; set; }

        public int ApplyTypeId { get; set; }

        public OrderApplyTypes ApplyType { get; set; }

        public int? PromoCodeId { get; set; }

        public string PromoCode { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateAsString { get; set; }

      //  public string InvoiceJSON { get; set; }

        public string InvoiceGUID { get; set; }

        public string ProductsJSON { get; set; }  // Consider making a collection to hold products
        
        public string ClientProductsJSON { get; set; } // Client products to the moment of ordering

        public string UserProductsJSON { get; set; } // User products to the moment of ordering

        public int PaymentTypeId { get; set; }

        public PaymentTypes PaymentType { get; set; }

        public string TransactionStatus { get; set; } // Used by BrainTree for credit card payments. Not populated in other types.

        public ulong TotalQueryCount { get; set; }

        public OrderAdm()
        {
            //defaults
            this.StatusTypeId = 1;
            this.StatusType = OrderStatusTypes.Pending;
            this.ApplyTypeId = 1;
            this.ApplyType = OrderApplyTypes.ContinuePeriod;
            this.PaymentTypeId = 11;
            this.PaymentType = PaymentTypes.PayPal;
        }

        public OrderAdm(IDataRecord row)
        {
            this.OrderId = int.Parse(row["order_id"].ToString());
            this.ClientId = int.Parse(row["client_id"].ToString());
            this.ClientName = row["client_name"].ToString();
            if (row["seller_id"] != DBNull.Value)
            {
                this.SellerId = int.Parse(row["seller_id"].ToString());
                this.SellerName = row["seller_name"].ToString();
            }

            this.StatusTypeId = int.Parse(row["status_type_id"].ToString());
            this.StatusType = (OrderStatusTypes)this.StatusTypeId;
            this.ApplyTypeId = int.Parse(row["apply_type_id"].ToString());
            this.ApplyType = (OrderApplyTypes)this.ApplyTypeId;
            if (row["promo_code_id"] != DBNull.Value)
            {
                this.PromoCodeId = int.Parse(row["promo_code_id"].ToString());
                this.PromoCode = row["promo_code"].ToString();
            }

            this.CreateDate = DateTime.Parse(row["create_date"].ToString());
            this.CreateDateAsString = this.CreateDate.ToShortDateString();
           // this.InvoiceJSON = row["invoice_json"].ToString();
            if (!(String.IsNullOrEmpty(row["invoice_guid"].ToString())))
            {
                this.InvoiceGUID = row["invoice_guid"].ToString();
            }
          
            this.ProductsJSON = row["products_json"].ToString();
            this.ClientProductsJSON = row["client_products_json"].ToString();
            this.UserProductsJSON = row["user_products_json"].ToString();

            if (row["payment_type_id"] != DBNull.Value)
            {
                this.PaymentTypeId = int.Parse(row["payment_type_id"].ToString());
                this.PaymentType = (PaymentTypes)this.PaymentTypeId;
            }

            if (row["transaction_status"] != DBNull.Value && row["transaction_status"] != null && row["transaction_status"].ToString() != "")
            {
                this.TransactionStatus = row["transaction_status"].ToString().Replace("_", " ").ToLower().CapitalizeFirstLetter(); // normalizing string
            }

            this.TotalQueryCount = UInt64.Parse(row["total_query_count"].ToString());
        }

        public static List<OrderAdm> GetOrdersAdm(int orderId, int clientId, int sellerId, int statusTypeId, int applyTypeId, string promoCode, DateTime dateFrom, DateTime dateTo, string orderBy, string orderDir, int skip, int take, string searchText, int paymentTypeId, int currentSellerId)
        {
            var orders = new List<OrderAdm>();

            foreach (var item in DB.GetOrdersAdm(orderId, clientId, sellerId, statusTypeId, applyTypeId, promoCode, dateFrom, dateTo, orderBy, orderDir, skip, take, searchText, paymentTypeId, currentSellerId))
            {
                orders.Add(new OrderAdm(item));
            }

            return orders;
        }

        public static string GetInvoiceDetailsJSON(int orderId)
        {
            return DB.GetInvoiceDetailsJSON(orderId);
        }

        public static Dictionary<int, string> GetOrderStatusTypes()
        {
            var dict = new Dictionary<int, string>();

            foreach (var item in DB.GetOrderStatusTypes())
            {
                dict.Add(Convert.ToInt32(item["id"]), item["name"].ToString());
            }

            return dict;
        }

        public static Dictionary<int, string> GetOrderApplyTypes()
        {
            var dict = new Dictionary<int, string>();

            foreach (var item in DB.GetOrderApplyTypes())
            {
                dict.Add(Convert.ToInt32(item["id"]), item["name"].ToString());
            }

            return dict;
        }
    }
}
