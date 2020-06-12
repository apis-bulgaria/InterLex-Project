namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.DataLayer;
    using Interlex.BusinessLayer.Enums;
    using System.Data;

    public class PromoCodeData
    {
        public int? Id { get; set; }

        public bool Active { get; set; }

        public int? SellerId { get; set; }

        public string ClientName { get; set; }

        public string Code { get; set; }

        PromoCodeTypes Type { get; set; }

        public bool PackAllCodes { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreateDateAsString { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string ExpiryDateAsString { get; set; }

        public DateTime? UsedDate { get; set; }

        public List<PromoProduct> Products { get; set; }

        public PromoCodeData()
        {
            this.Type = PromoCodeTypes.SellerBase;
            this.Products = BusinessLayer.Models.PromoProduct.GetAllProducts();
        }

        public PromoCodeData(PromoCodeData pcd)
        {
            this.Id = pcd.Id;
            this.Active = pcd.Active;
            this.SellerId = pcd.SellerId;
            this.Code = pcd.Code;
            this.Type = pcd.Type;
            this.PackAllCodes = pcd.PackAllCodes;
            this.CreateDate = pcd.CreateDate;
            this.CreateDateAsString = pcd.CreateDateAsString;
            this.ExpiryDate = pcd.ExpiryDate;
            this.ExpiryDateAsString = pcd.ExpiryDateAsString;
            this.UsedDate = pcd.UsedDate;
            this.ClientName = pcd.ClientName;
            this.Products = pcd.Products;
        }

        public PromoCodeData(IDataRecord rCode)
            : this()
        {
            this.Active = Convert.ToBoolean(rCode["active"].ToString());
            this.SellerId = int.Parse(rCode["seller_id"].ToString());
            this.Code = rCode["code"].ToString();
            this.PackAllCodes = Convert.ToBoolean(rCode["pack_all_prod"].ToString());
        
            if (!(String.IsNullOrEmpty(rCode["create_date"].ToString())))
            {
                this.CreateDate = DateTime.Parse(rCode["create_date"].ToString());
                this.CreateDateAsString = rCode["create_date"].ToString();
            }

            if (!(String.IsNullOrEmpty(rCode["expire_date"].ToString())))
            {
                this.ExpiryDate = DateTime.Parse(rCode["expire_date"].ToString());
                this.ExpiryDateAsString = rCode["expire_date"].ToString();
            }

            if (!(String.IsNullOrEmpty(rCode["used_date"].ToString())))
            {
               this.UsedDate = DateTime.Parse(rCode["used_date"].ToString());
            }
        }

        public static List<PromoCodeData> GetSellerPromoCodes(int sellerId) 
        {
            var items = new List<PromoCodeData>();

            var tableFromDB = DB.GetSellerPromoCodes(sellerId);

            foreach (var row in tableFromDB)
            {
                var item = new PromoCodeData(row);
                items.Add(item);
            }

            return items;
        }

        public static List<PromoCodeData> NewSellerPromoCode(int sellerId) 
        {
            var items = new List<PromoCodeData>();

            var tableFromDB = DB.NewSellerPromoCode(sellerId);

            foreach (var row in tableFromDB)
            {
                var item = new PromoCodeData(row);
                items.Add(item);
            }

            return items;
        }
    }
}
