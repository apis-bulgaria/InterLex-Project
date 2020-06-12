namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Interlex.DataLayer;

    public class ClientData
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int? SellerId { get; set; }
        public int? SellerUserId { get; set; }
        public string SellerName { get; set; }

        public string Note { get; set; }

        public int CountryId { get; set; }

        public int TotalQueryCount { get; set; }

        public string CountryName { get; set; } //only for visualization

        public int OriginId { get; set; } = 0;
        
        private List<Product> _productsList = null;

        public List<Product> Products 
        {
            get
            {
                if (this._productsList == null)
                    this._productsList = UserMng.GetProducts(this.ClientId);
                return this._productsList;
            }
            set
            {
                this._productsList = value;
            }
        }
        //public string ProductsList { get; set; }

        public string[] ProductsList
        {
            get
            {
                if (Products != null)
                {
                    string[] products = new string[Products.Count];
                    for (int i = 0; i < Products.Count; i++)
                        products[i] = Products[i].ProductName;
                    return products;
                }
                else
                    return null;
            }
        }

        public ClientData()
        {
        }

        public ClientData(DataRow row) : base()
        {
            this.ClientId = Convert.ToInt32(row["client_id"]);
            this.ClientName = row["client_name"].ToString();

            if (row["seller_id"] != null && row["seller_id"].ToString() != "" && row["seller_id"].ToString().ToUpper() != "NULL")
            {
                this.SellerId = int.Parse(row["seller_id"].ToString());
            }

            if (row["seller_name"] != null && row["seller_name"].ToString() != "" && row["seller_name"].ToString().ToUpper() != "NULL")
            {
                this.SellerName = row["seller_name"].ToString();
            }

            if (row["seller_user_id"] != null && row["seller_user_id"].ToString() != "" && row["seller_user_id"].ToString().ToUpper() != "NULL")
            {
                this.SellerUserId = int.Parse(row["seller_user_id"].ToString());
            }

            if (row["note"] != null)
            {
                this.Note = row["note"].ToString();
            }

            if (row["country_id"] != null)
            {
                this.CountryId = int.Parse(row["country_id"].ToString());
            }

            if (row["country_name"] != null)
            {
                this.CountryName = row["country_name"].ToString();
            }

            try
            {
                if (row["total_query_count"] != null && row["total_query_count"] != DBNull.Value)
                {
                    this.TotalQueryCount = Convert.ToInt32(row["total_query_count"]);
                }
            }
            catch (Exception)
            {
                this.TotalQueryCount = 0;
            }

            try
            {
                if (row["origin_id"] != DBNull.Value && row["origin_id"] != null)
                {
                    this.OriginId = int.Parse(row["origin_id"].ToString());
                }
            }
            catch (Exception)
            {
                this.OriginId = 1; // consider adding a fictive value
            }
           
        }

        public static bool ClientHasUserFromSite(int clientId)
        {
            return DB.ClientHasUserFromSite(clientId);
        }
    }
}
