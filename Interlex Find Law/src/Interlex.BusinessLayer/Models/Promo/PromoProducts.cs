namespace Interlex.BusinessLayer.Models
{
    using Interlex.DataLayer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PromoProduct : Product
    {
        public double Discount { get; set; }

        public int AddDays { get; set; }

        public static List<PromoProduct> GetAllProducts()
        {
            var items = new List<PromoProduct>();

            foreach (var element in DB.GetProducts())
            {
                var item = new PromoProduct();
                item.ProductId = int.Parse(element["product_id"].ToString());
                item.ProductName = element["product_name"].ToString();
                item.LicenseCnt = 0;

                items.Add(item);
            }

            return items;
        }
    }
}
