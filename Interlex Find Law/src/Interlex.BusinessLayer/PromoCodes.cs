using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Interlex.DataLayer;
using Interlex.BusinessLayer.Enums;
using Interlex.BusinessLayer.Models;
using Newtonsoft.Json;
using System.IO;

namespace Interlex.BusinessLayer
{
    public class PromoCodes
    {
        public static PromoCode GetPromoCode(string promoCode)
        {
            PromoCode pc = null;
            DataRow row = DB.GetPromoCode(promoCode);
            if (row != null)
            {
                pc = new PromoCode(row);
                //{
                //    Id = Convert.ToInt32(row["id"]),
                //    Code = row["code"].ToString(),
                //    SellerId = Convert.ToInt32(row["seller_id"]),
                //    TypeId = Convert.ToInt32(row["type_id"]),
                //    PackAllProd = Convert.ToBoolean(row["pack_all_prod"]),
                //    StartDate = (row["start_date"] != DBNull.Value)?(DateTime?)row["start_date"]:(DateTime?)null,
                //    ExpireDate = (row["expire_date"] != DBNull.Value) ? (DateTime?)row["expire_date"] : (DateTime?)null,
                //    CreateDate = (DateTime)row["create_date"],
                //    UsedDate = (row["used_date"] != DBNull.Value) ? (DateTime?)row["used_date"] : (DateTime?)null,
                //    Active = Convert.ToBoolean(row["active"])
                //};
            }
            return pc;
        }

        public static PromoCode GetPromoCode(int promoCodeId)
        {
            PromoCode pc = null;
            DataRow row = DB.GetPromoCode(promoCodeId);
            if (row != null)
            {
                pc = new PromoCode(row);
                //{
                //    Id = Convert.ToInt32(row["id"]),
                //    Code = row["code"].ToString(),
                //    SellerId = Convert.ToInt32(row["seller_id"]),
                //    TypeId = Convert.ToInt32(row["type_id"]),
                //    PackAllProd = Convert.ToBoolean(row["pack_all_prod"]),
                //    StartDate = (row["start_date"] != DBNull.Value)?(DateTime?)row["start_date"]:(DateTime?)null,
                //    ExpireDate = (row["expire_date"] != DBNull.Value) ? (DateTime?)row["expire_date"] : (DateTime?)null,
                //    CreateDate = (DateTime)row["create_date"],
                //    UsedDate = (row["used_date"] != DBNull.Value) ? (DateTime?)row["used_date"] : (DateTime?)null,
                //    Active = Convert.ToBoolean(row["active"])
                //};
            }
            return pc;
        }
    }
}
