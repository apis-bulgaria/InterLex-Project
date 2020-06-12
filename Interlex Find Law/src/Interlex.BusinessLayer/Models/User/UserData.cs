using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Interlex.BusinessLayer.Enums;
using Interlex.BusinessLayer.Helpers;
using System.Text.RegularExpressions;

namespace Interlex.BusinessLayer.Models
{
    public class UserData
    {
        public int UserId { get; set; }
        public UserTypes UserType { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeText { get; set; }
        //public string UserTypeName { get; set; }
        public int SessionId { get; set; }
        public int ClientId { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string ClientName { get; set; }
        public bool PushSessions { get; set; }
        public int MaxLoginCount { get; set; }
        public bool Active { get; set; }
        public bool EmailValid { get; set; }

        public DateTime CreateDate { get; set; }

        public bool OpenDocumentsInNewTab { get; set; }

        public bool ShowFreeDocuments { get; set; } = true;

        public bool ShowHints { get; set; }

        public int? SellerId { get; set; }

        public string SellerName { get; set; }

        public string PromoCode { get; set; }

        public string Code { get; set; }
        public string Phone { get; set; }
        public string SkypeName { get; set; }
        public int? CountryId { get; set; }

        public string Note { get; set; }

        //SELLER ASSISTENT; TODO: HIERARCHY
        public int? SellerParentId { get; set; }
        public string SellerParentName { get; set; }

        public int? SellerCountryId { get; set; }

        public int TotalQueryCount { get; set; }

        /// <summary>
        /// Session timeout in minutes
        /// </summary>
        public int SessionTimeout { get; set; }

        public List<Product> Products { get; set; }

        public int OriginId { get; set; }

        public string RegistrationSource { get; set; }
        public int? RegistrationProductId { get; set; }

        private string allowedIpsReq;
        public string AllowedIpsReq
        {
            get
            {
                return allowedIpsReq;
            }
            set
            {
                if(value == null)
                {
                    this.allowedIpsReq = "";
                }
                else
                {
                    this.allowedIpsReq = Regex.Replace(value, @"\s+", "");
                }

            }
        }

        public List<string> AllowedIps
        {
            get
            {
                if (string.IsNullOrEmpty(this.AllowedIpsReq))
                {
                    return new List<string>();
                }

                var list = this.AllowedIpsReq.Split(new char[] { ';' }).ToList();
                list.RemoveAll(x => x == "");

                return list;
            }
            private set
            {
            }
        }

        public UserData()
        {
            // default values
            this.UserType = UserTypes.User;
            this.UserTypeId = (int)UserType;

            this.PushSessions = true;
            this.MaxLoginCount = 1;
            this.SessionTimeout = 30; // minutes
            this.EmailValid = true;

            this.OpenDocumentsInNewTab = true;
            this.Active = true;

            this.TotalQueryCount = 0;

            this.Products = new List<Product>();
        }

        public UserData(UserData ud)
            : this()
        {
            this.UserId = ud.UserId;
            this.UserType = ud.UserType;
            this.UserTypeId = ud.UserTypeId;
            this.UserTypeText = ud.UserTypeText;
            //this.UserTypeName = ud.UserTypeName;
            this.SessionId = ud.SessionId;
            this.ClientId = ud.ClientId;
            this.Username = ud.Username;
            this.Email = ud.Email;
            this.Password = ud.Password;
            this.Fullname = ud.Fullname;
            this.ClientName = ud.ClientName;
            this.PushSessions = ud.PushSessions;
            this.MaxLoginCount = ud.MaxLoginCount;
            this.SessionTimeout = ud.SessionTimeout;
            this.Active = ud.Active;
            this.EmailValid = ud.EmailValid;
            this.CreateDate = ud.CreateDate;

            this.OpenDocumentsInNewTab = ud.OpenDocumentsInNewTab;
            this.ShowFreeDocuments = ud.ShowFreeDocuments;

            this.SellerId = ud.SellerId;
            this.Code = ud.Code;
            this.Phone = ud.Phone;
            this.SkypeName = ud.SkypeName;
            this.CountryId = ud.CountryId;

            this.Note = ud.Note;
            this.AllowedIpsReq = ud.AllowedIpsReq;

            this.SellerParentId = ud.SellerParentId;
            if (ud.SellerParentId != null && ud.SellerParentId != -1 && ud.SellerParentId != 0)
            {
                this.SellerParentName = UserMng.GetSellerNameBySellerId((int)ud.SellerParentId);
            }

            var products = UserMng.GetUserProducts(this.UserId);
            if (products != null)
            {
                foreach (var product in products)
                {
                    this.Products.Add(product);
                }
            }

            this.OriginId = ud.OriginId;
        }

        public UserData(DataRow rUser)
            : this()
        {
            this.Products = new List<Product>();

            this.UserId = Convert.ToInt32(rUser["user_id"]);
            this.UserTypeId = Convert.ToInt32(rUser["usertype_id"]);
            this.UserType = (UserTypes)(int.Parse(rUser["usertype_id"].ToString()));
            // this.UserTypeText = rUser["usertype_text"].ToString();
            this.ClientId = Convert.ToInt32(rUser["client_id"]);
            this.Username = rUser["username"].ToString();
            if (rUser["password"] != null)
                this.Password = rUser["password"].ToString();
            this.Email = rUser["email"].ToString();
            this.Fullname = rUser["fullname"].ToString();
            this.ClientName = rUser["client_name"].ToString();
            this.PushSessions = Convert.ToBoolean(rUser["push_session"]);
            this.MaxLoginCount = Convert.ToInt32(rUser["max_login_count"]);
            this.SessionTimeout = Convert.ToInt32(rUser["session_timeout"]);
            this.Active = Convert.ToBoolean(rUser["active"].ToString());
            this.EmailValid = Convert.ToBoolean(rUser["email_valid"].ToString());

            bool hasCreateDate = rUser.Table.Columns.Contains("create_date");
            hasCreateDate = hasCreateDate && !String.IsNullOrEmpty(rUser["create_date"]?.ToString());
            if (hasCreateDate)
            {
                this.CreateDate = Convert.ToDateTime(rUser["create_date"].ToString());
            }

            bool hasSellerId = rUser.Table.Columns.Contains("seller_id");
            hasSellerId = hasSellerId && !String.IsNullOrEmpty(rUser["seller_id"]?.ToString());

            if (hasSellerId)
            {
                this.SellerId = int.Parse(rUser["seller_id"].ToString());
            }
            else
            {
                this.SellerId = null;
            }

            int originId = 1;
            bool hasOriginId = rUser.Table.Columns.Contains("origin_id");
            hasOriginId = hasOriginId && int.TryParse(rUser["origin_id"]?.ToString() ?? "1", out originId); // use '1'  as default
            this.OriginId = originId;

            if (rUser.Table.Columns.Contains("registration_source"))
            {
                this.RegistrationSource = rUser["registration_source"].ToString();
            }

            if (rUser.Table.Columns.Contains("registration_product_id") && rUser["registration_product_id"] != DBNull.Value)
            { 
                this.RegistrationProductId = Convert.ToInt32(rUser["registration_product_id"]);
            }

            if(rUser.Table.Columns.Contains("code"))
            {
                this.Code = rUser["code"].ToString();
            }

            if(rUser.Table.Columns.Contains("phone"))
            {
                this.Phone = rUser["phone"].ToString();
            }
            
            if(rUser.Table.Columns.Contains("skype_name"))
            {
                this.SkypeName = rUser["skype_name"].ToString();
            }

            if (rUser.Table.Columns.Contains("links_in_new_tab"))
            {
                this.OpenDocumentsInNewTab = Boolean.Parse(rUser["links_in_new_tab"].ToString());
            }

            if (rUser.Table.Columns.Contains("show_free_documents"))
            {
                this.ShowFreeDocuments = Boolean.Parse(rUser["show_free_documents"].ToString());
            }

            if (rUser.Table.Columns.Contains("parent_seller_id") && rUser["parent_seller_id"] != null && rUser["parent_seller_id"].ToString() != "")
            {
                var b = rUser["parent_seller_id"].ToString();
                this.SellerParentId = int.Parse(rUser["parent_seller_id"].ToString());
                this.SellerParentName = UserMng.GetSellerNameBySellerId((int)this.SellerParentId);
            }

            if(rUser.Table.Columns.Contains("allowed_ip") && rUser["allowed_ip"] != null && rUser["allowed_ip"] != DBNull.Value)
            {
                this.AllowedIpsReq = ((string[])rUser["allowed_ip"]).ToString(";");
            }

            if (rUser.Table.Columns.Contains("note") && rUser["note"] != null)
            {
                this.Note = rUser["note"].ToString();
            }

            if (rUser.Table.Columns.Contains("country_id") && rUser["country_id"] != null)
            {
                if (rUser["country_id"].ToString() != "" && rUser["country_id"].ToString() != null && string.IsNullOrEmpty(rUser["country_id"].ToString()) == false)
                {
                    this.CountryId = int.Parse(rUser["country_id"].ToString());
                }
            }

            if (rUser.Table.Columns.Contains("seller_country_id") && rUser["seller_country_id"] != null)
            {
                if (rUser["seller_country_id"].ToString() != "" && rUser["seller_country_id"].ToString() != null && string.IsNullOrEmpty(rUser["seller_country_id"].ToString()) == false)
                {
                    this.SellerCountryId = int.Parse(rUser["seller_country_id"].ToString());
                }
            }
            else
            {
                this.SellerCountryId = -1;
            }
            
            var products = UserMng.GetUserProducts(this.UserId);
            if (products != null)
            {
                foreach (var product in products)
                {
                    this.Products.Add(product);
                }
            }

            // Not used in this method
            this.SessionId = 0;
        }

        public UserData(DataRow rUser, DataTable dtProducts)
            : this(rUser)
        {
            if (dtProducts.Rows.Count > 0)
            {
                foreach (DataRow row in dtProducts.Rows)
                {
                    Product p = new Product();
                    p.ProductId = Convert.ToInt32(row["product_id"]);
                    p.ProductName = row["product_name"].ToString();
                    p.StartDate = Convert.ToDateTime(row["start_date"]);
                    p.EndDate = Convert.ToDateTime(row["end_date"]);
                    p.LicenseCnt = Convert.ToInt32(row["license_cnt"]);

                    Products.Add(p);
                }
            }
        }
    }
}
