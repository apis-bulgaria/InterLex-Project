using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Interlex.BusinessLayer.Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int SellerId { get; set; }
        public int TypeId { get; set; }
        public bool PackAllProd { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UsedDate { get; set; }
        public bool Active { get; set; }

        public PromoCode()
        {
        }

        public PromoCode(DataRow row)
        {
            if (row != null)
            {
                Id = Convert.ToInt32(row["id"]);
                Code = row["code"].ToString();
                SellerId = Convert.ToInt32(row["seller_id"]);
                TypeId = Convert.ToInt32(row["type_id"]);
                PackAllProd = Convert.ToBoolean(row["pack_all_prod"]);
                StartDate = (row["start_date"] != DBNull.Value) ? (DateTime?)row["start_date"] : (DateTime?)null;
                ExpireDate = (row["expire_date"] != DBNull.Value) ? (DateTime?)row["expire_date"] : (DateTime?)null;
                CreateDate = (DateTime)row["create_date"];
                UsedDate = (row["used_date"] != DBNull.Value) ? (DateTime?)row["used_date"] : (DateTime?)null;
                Active = Convert.ToBoolean(row["active"]);
            }
        }
    }
}
