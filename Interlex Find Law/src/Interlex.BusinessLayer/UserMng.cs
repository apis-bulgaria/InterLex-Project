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
using Interlex.BusinessLayer.Models.Folders;
using Newtonsoft.Json;
using System.IO;
using UnidecodeSharpFork;
using NetTools;
using System.Net;
using System.Net.Sockets;
//using Interlex.BusinessLayer.Models.Folders;

namespace Interlex.BusinessLayer
{
    public class SessionData
    {
        public int SessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LastAccess { get; set; }
        public string IPAddress { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserType { get; set; }
        public string ClientName { get; set; }
    }

    public class UserMng
    {
        public static UserData Login(string username, string password, bool passHashed, string ipAddr)
        {
            UserData ud = null;

            // Check username/password
            DataRow u = DB.GetUser(username, password, passHashed);
            if (u != null)
            {
                ud = new UserData(u);
                // Clear all users expired sessions
                DB.ClearOldSessions();
                if (!ud.EmailValid)
                    ud.SessionId = -3;
                else if (!ud.Active)
                    ud.SessionId = -4;
                else if (!IsAllowedIp(ipAddr, ud.AllowedIps))
                    ud.SessionId = -5;
                else
                {
                    // Start new session
                    ud.SessionId = DB.AddSession(ud.UserId, ipAddr);
                }
            }

            return ud;
        }

        public static bool IsAllowedIp(string currentIp, List<string> allowedIps)
        {
            if(allowedIps == null || allowedIps.Count == 0)
            {
                return true;
            }

            return IsInIpRange(currentIp, allowedIps);
        }

        public static bool IsAllowedIpUsernameLogin(string currentIp, List<string> allowedIps)
        {
            if(allowedIps == null || allowedIps.Count == 0)
            {
                return false;
            }

            return IsInIpRange(currentIp, allowedIps);
        }

        private static bool IsInIpRange(string currentIp, List<string> allowedIps)
        {
            var currentIpRange = IPAddressRange.Parse(currentIp);
            foreach (string ip in allowedIps)
            {
                var range = IPAddressRange.Parse(ip);
                if (range.Contains(currentIpRange))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidIP(string ip)
        {
            IPAddress address;
            if (IPAddress.TryParse(ip, out address))
            {
                return true;
            }

            return false;
        }

        private static string CreateTOKEN()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 30)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }

        public static UserData GetUser(string username, string password, bool passHashed)
        {
            UserData ud = null;
            // Check username/password
            DataRow u = DB.GetUser(username, password, passHashed);
            if (u != null)
            {
                ud = new UserData(u);
            }
            return ud;
        }

        public static UserData GetUser(string email)
        {
            UserData ud = null;
            // Check username/password
            DataRow u = DB.GetUser(email);
            if (u != null)
            {
                ud = new UserData(u);
            }
            return ud;
        }

        public static UserData GetUserByUsername(string username)
        {
            UserData ud = null;
            // Check username/password
            DataRow u = DB.GetUserByUsername(username);
            if (u != null)
            {
                ud = new UserData(u);
            }
            return ud;
        }

        public static UserData GetUser(int userId)
        {
            UserData ud = null;
            DataRow u = DB.GetUser(userId);
            if (u != null)
            {
                ud = new UserData(u);
            }
            return ud;
        }

        public static ClientData GetClient(int clientId)
        {
            ClientData d = null;
            DataRow u = DB.GetClient(clientId);
            if (u != null)
            {
                d = new ClientData(u);
            }
            return d;
        }

        public static int GetClientProductLicenseCount(int clientId, int productId)
        {
            return DB.GetClientProductLicenseCount(clientId, productId);
        }

        public static int GetClientProductAvaiableLicenseCount(int clientId, int productId, int userId)
        {
            return DB.GetClientProductAvaiableLicenseCount(clientId, productId, userId);
        }

        public static int GetClientProductAvaiableLicenseCountExcludeUP(int clientId, int productId)
        {
            return DB.GetClientProductAvaiableLicenseCount(clientId, productId, -1);
        }

        public static void EndUserSession(int sessionId, string sessionTempFolder)
        {
            DB.DelSession(sessionId);
            UserMng.DeleteSessionTempFolder(sessionTempFolder);
        }

        public static bool UpdateUserSession(int sessionId, int timeoutSeconds)
        {
            return DB.UpdateSession(sessionId, timeoutSeconds);
        }

        public static List<SessionData> GetSessions(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, bool showDemoSessions)
        {
            List<SessionData> l = new List<SessionData>();
            foreach (var r in DB.GetSessions(searchText, startFrom, pageSize, sortBy, sortDir, showDemoSessions))
            {
                SessionData item = new SessionData();

                item.SessionId = Convert.ToInt32(r["session_id"]);
                item.StartDate = (DateTime)r["start_date"];
                item.LastAccess = (DateTime)r["last_access"];
                item.IPAddress = r["ip_addr"].ToString();
                item.UserId = Convert.ToInt32(r["user_id"]);
                item.Username = r["username"].ToString();
                item.UserType = r["usertype"].ToString();
                item.ClientName = r["client_name"].ToString();

                l.Add(item);
            }
            return l;
        }

        public static int GetTotalSessionsCount(string searchText, bool showDemoSessions)
        {
            return DB.GetTotalSessionsCount(searchText, showDemoSessions);
        }

        //Used in administration's real users' list with enabled editing
        public static List<UserData> GetUsers(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int userTypeId, int sellerId, string note, int minLoginCount, int maxLoginCount, int originId, DateTime from, DateTime to)
        {
            List<UserData> l = new List<UserData>();
            foreach (var r in DB.GetUsers(searchText, startFrom, pageSize, sortBy, sortDir, userTypeId, sellerId, note, minLoginCount, maxLoginCount, originId, from, to))
            {
                UserData item = new UserData();

                item.UserId = Convert.ToInt32(r["user_id"]);
                item.Username = r["username"].ToString();
                item.Email = r["email"].ToString();
                item.Fullname = r["fullname"].ToString();
                item.UserTypeId = int.Parse(r["usertype_id"].ToString());
                item.UserType = (UserTypes)int.Parse(r["usertype_id"].ToString());
                item.UserTypeText = r["usertype_text"].ToString();
                //item.UserTypeName = r["usertype"].ToString();
                item.ClientId = Convert.ToInt32(r["client_id"]);
                item.ClientName = r["client_name"].ToString();
                item.PushSessions = Convert.ToBoolean(r["push_session"]);
                item.MaxLoginCount = Convert.ToInt32(r["max_login_count"]);
                item.SessionTimeout = Convert.ToInt32(r["session_timeout"]);
                item.CreateDate = Convert.ToDateTime(r["create_date"].ToString());
                item.Code = r["code"].ToString();
                item.Phone = r["code"].ToString();

                item.Note = r["note"].ToString();

                item.TotalQueryCount = int.Parse(r["total_query_count"].ToString());
                item.OriginId = int.Parse(r["origin_id"].ToString());
                item.Active = Convert.ToBoolean(r["active"]);

                l.Add(item);
            }
            return l;
        }

        //Used in administration statistics
        public static List<UserData> GetUsersStat(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int userTypeId, int sellerId, string note, int minLoginCount, int maxLoginCount, int originId, int currentSellerId)
        {
            List<UserData> l = new List<UserData>();
            foreach (var r in DB.GetUsersStat(searchText, startFrom, pageSize, sortBy, sortDir, userTypeId, sellerId, note, minLoginCount, maxLoginCount, originId, currentSellerId))
            {
                UserData item = new UserData();

                item.UserId = Convert.ToInt32(r["user_id"]);
                item.Username = r["username"].ToString();
                item.Email = r["email"].ToString();
                item.Fullname = r["fullname"].ToString();
                item.UserTypeId = int.Parse(r["usertype_id"].ToString());
                item.UserType = (UserTypes)int.Parse(r["usertype_id"].ToString());
                item.UserTypeText = r["usertype_text"].ToString();
                //item.UserTypeName = r["usertype"].ToString();
                item.ClientId = Convert.ToInt32(r["client_id"]);
                item.ClientName = r["client_name"].ToString();
                item.PushSessions = Convert.ToBoolean(r["push_session"]);
                item.MaxLoginCount = Convert.ToInt32(r["max_login_count"]);
                item.SessionTimeout = Convert.ToInt32(r["session_timeout"]);

                item.Code = r["code"].ToString();
                item.Phone = r["code"].ToString();

                item.Note = r["note"].ToString();

                item.TotalQueryCount = int.Parse(r["total_query_count"].ToString());
                item.OriginId = int.Parse(r["origin_id"].ToString());
                item.Active = Convert.ToBoolean(r["active"]);

                l.Add(item);
            }
            return l;
        }

        public static List<string> GetUsers(int currentSellerId)
        {
            var items = new List<string>();

            foreach (var r in DB.GetUsers(currentSellerId))
            {
                items.Add(r["username"].ToString());
            }

            return items;
        }

        public static List<UsersDataSimple> GetUsersWithId(int currentSellerId)
        {
            var items = new List<UsersDataSimple>();

            foreach (var r in DB.GetUsers(currentSellerId))
            {
                var item = new UsersDataSimple();
                item.Username = r["username"].ToString();
                item.Id = int.Parse(r["id"].ToString());

                items.Add(item);
            }

            return items;
        }

        public static int GetUsersCount(int userTypeId)
        {
            return DB.GetUsersCount(userTypeId);
        }

        //Used in real clients list with enabled editing
        public static List<ClientData> GetClients(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int sellerId, int originId)
        {
            List<ClientData> l = new List<ClientData>();
            foreach (var r in DB.GetClients(searchText, startFrom, pageSize, sortBy, sortDir, sellerId, originId))
            {
                ClientData item = new ClientData();

                item.ClientId = Convert.ToInt32(r["client_id"]);
                item.ClientName = r["client_name"].ToString();

                if (r["seller_id"] != null && r["seller_id"].ToString() != "" && r["seller_id"].ToString().ToUpper() != "NULL")
                {
                    item.SellerId = int.Parse(r["seller_id"].ToString());
                }

                if (r["seller_name"] != null && r["seller_name"].ToString() != "" && r["seller_name"].ToString().ToUpper() != "NULL")
                {
                    item.SellerName = r["seller_name"].ToString();
                }

                if (r["seller_user_id"] != null && r["seller_user_id"].ToString() != "" && r["seller_user_id"].ToString().ToUpper() != "NULL")
                {
                    item.SellerUserId = int.Parse(r["seller_user_id"].ToString());
                }

                if (r["country_id"] != null && r["country_id"].ToString() != "")
                {
                    item.CountryId = int.Parse(r["country_id"].ToString());
                }

                if (r["country_name"] != null && r["country_name"].ToString() != "" && r["country_name"].ToString().ToUpper() != "NULL")
                {
                    item.CountryName = r["country_name"].ToString();
                }

                if (r["total_query_count"] != DBNull.Value)
                {
                    item.TotalQueryCount = Convert.ToInt32(r["total_query_count"]);
                }

                if (r["origin_id_out"] != DBNull.Value && r["origin_id_out"] != null)
                {
                    item.OriginId = int.Parse(r["origin_id_out"].ToString());
                }

                item.Products = UserMng.GetClientProducts(item.ClientId);

                l.Add(item);
            }
            return l;
        }

        //Used in statistics in administration
        public static List<ClientData> GetClientsStat(string searchText, int startFrom, int pageSize, string sortBy, string sortDir, int sellerId, int currentSellerId, int originId)
        {
            List<ClientData> l = new List<ClientData>();
            foreach (var r in DB.GetClientsStats(searchText, startFrom, pageSize, sortBy, sortDir, sellerId, currentSellerId, originId))
            {
                ClientData item = new ClientData();

                item.ClientId = Convert.ToInt32(r["client_id"]);
                item.ClientName = r["client_name"].ToString();

                if (r["seller_id"] != null && r["seller_id"].ToString() != "" && r["seller_id"].ToString().ToUpper() != "NULL")
                {
                    item.SellerId = int.Parse(r["seller_id"].ToString());
                }

                if (r["seller_name"] != null && r["seller_name"].ToString() != "" && r["seller_name"].ToString().ToUpper() != "NULL")
                {
                    item.SellerName = r["seller_name"].ToString();
                }

                if (r["seller_user_id"] != null && r["seller_user_id"].ToString() != "" && r["seller_user_id"].ToString().ToUpper() != "NULL")
                {
                    item.SellerUserId = int.Parse(r["seller_user_id"].ToString());
                }

                if (r["country_id"] != null && r["country_id"].ToString() != "")
                {
                    item.CountryId = int.Parse(r["country_id"].ToString());
                }

                if (r["country_name"] != null && r["country_name"].ToString() != "" && r["country_name"].ToString().ToUpper() != "NULL")
                {
                    item.CountryName = r["country_name"].ToString();
                }

                if (r["total_query_count"] != DBNull.Value)
                {
                    item.TotalQueryCount = Convert.ToInt32(r["total_query_count"]);
                }

                if (r["origin_id_out"] != DBNull.Value && r["origin_id_out"] != null)
                {
                    item.OriginId = int.Parse(r["origin_id_out"].ToString());
                }

                item.Products = UserMng.GetClientProducts(item.ClientId);

                l.Add(item);
            }
            return l;
        }


        public static List<SellerData> GetSellers(int currentSellerId)
        {
            List<SellerData> l = new List<SellerData>();
            foreach (var r in DB.GetSellers(currentSellerId))
            {
                SellerData item = new SellerData();

                item.SellerId = int.Parse(r["seller_id"].ToString());
                item.Code = r["code"].ToString();
                item.UserId = int.Parse(r["user_id"].ToString());
                item.Username = r["username"].ToString();
                item.FullName = r["fullname"].ToString();
                item.Email = r["email"].ToString();

                if (r["parent_seller_id"].ToString() != "")
                {
                    item.ParentSellerId = int.Parse(r["parent_seller_id"].ToString());
                }

                l.Add(item);
            }

            return l;
        }

        public static int GetSellerIdByName(string sellerName)
        {
            return DB.GetSellerIdByName(sellerName);
        }

        public static List<object> GetUserTypes()
        {
            List<object> l = new List<object>();
            foreach (var r in DB.GetUserTypes())
            {
                l.Add(new
                {
                    Id = Convert.ToInt32(r["usertype_id"]),
                    Name = r["name"].ToString()
                });
            }
            return l;
        }

        public static List<object> GetClients(int currentSellerId)
        {
            List<object> l = new List<object>();
            foreach (var r in DB.GetClients(currentSellerId))
            {
                l.Add(new
                {
                    Id = Convert.ToInt32(r["client_id"]),
                    Name = r["client_name"].ToString()
                });
            }
            return l;
        }

        public static List<Product> GetClientProducts(int clientId)
        {
            List<Product> l = new List<Product>();
            foreach (var product in DB.GetClientProducts(clientId))
            {
                l.Add(new Product
                {
                    ProductId = Convert.ToInt32(product["product_id"]),
                    ProductName = product["product_name"].ToString(),
                    LicenseCnt = Convert.ToInt32(product["license_cnt"]),
                    StartDate = Convert.ToDateTime(product["start_date"]),
                    EndDate = Convert.ToDateTime(product["end_date"]),
                    Selected = true,
                    Demo = Convert.ToBoolean(product["demo"])
                });
            }
            return l;
        }

        public static List<Product> GetUserProducts(int userId)
        {
            List<Product> l = new List<Product>();
            foreach (var product in DB.GetUserProducts(userId))
            {
                l.Add(new Product
                {
                    ProductId = Convert.ToInt32(product["product_id"]),
                    ProductName = product["product_name"].ToString(),
                    LicenseCnt = Convert.ToInt32(product["license_cnt"]),
                    Selected = true,
                    EndDate = Convert.ToDateTime(product["end_date"]),
                    StartDate = Convert.ToDateTime(product["start_date"]),
                    Demo = Convert.ToBoolean(product["demo"]),
                    IsActive = Convert.ToBoolean(product["is_active"])
                });
            }
            return l;
        }

        public static List<Product> GetProducts(int clientId)
        {
            List<Product> l = new List<Product>();
            foreach (var product in DB.GetProducts(clientId))
            {
                l.Add(new Product
                {
                    ProductId = Convert.ToInt32(product["product_id"]),
                    ProductName = product["product_name"].ToString(),
                    LicenseCnt = (product["license_cnt"] == DBNull.Value) ? 0 : Convert.ToInt32(product["license_cnt"]),
                    StartDate = (product["start_date"] == DBNull.Value) ? (DateTime?)DateTime.Now : Convert.ToDateTime(product["start_date"]),
                    EndDate = (product["end_date"] == DBNull.Value) ? (DateTime?)DateTime.Now.AddYears(1) : Convert.ToDateTime(product["end_date"]),
                    Selected = Convert.ToBoolean(product["selected"]),
                    Demo = (product["demo"] == DBNull.Value) ? (false) : Convert.ToBoolean(product["demo"])
                });
            }
            return l;
        }

        public static List<Product> GetAllProducts()
        {
            List<Product> l = new List<Product>();
            foreach (var product in DB.GetProducts())
            {
                l.Add(new Product
                {
                    ProductId = Convert.ToInt32(product["product_id"]),
                    ProductName = product["product_name"].ToString()
                });
            }
            return l;
        }

        public static void DelUser(int userId)
        {
            DB.DelUser(userId);
        }

        public static void DelSession(int sessionId)
        {
            DB.DelSession(sessionId);
        }

        public static void DelClient(int clientId)
        {
            DB.DelClient(clientId);
        }

        public static int SetUser(UserDataAdm ud, int mngUserId)
        {
            List<Product> userProducts = ud.Products.FindAll(p => p.Selected == true);
            int[,] products = new int[userProducts.Count, 2];
            for (int i = 0; i < userProducts.Count; i++)
            {
                Product p = userProducts[i];
                products[i, 0] = userProducts[i].ProductId;
                products[i, 1] = userProducts[i].LicenseCnt;
            }

            return DB.SetUser(mngUserId, ud.UserId, ud.ClientId, ud.Username, ud.Email, ud.Password, ud.UserTypeId, ud.Fullname, ud.PushSessions, ud.MaxLoginCount, ud.SessionTimeout, ud.Code, ud.Phone, ud.SkypeName, ud.CountryId, products, ud.Note, ud.SellerParentId, ud.Active, ud.OriginId, ud.EmailValid, ud.AllowedIps);
        }

        public static bool UserExists(int? userId, string username)
        {
            return DB.UserExists(userId, username);
        }

        public static bool ExistsUsername(string username)
        {
            return DB.ExistsUsername(username);
        }

        public static bool ExistsEmail(string email)
        {
            return DB.ExistsEmail(email);
        }

        public static bool ExistsEmail(string email, int userId)
        {
            return DB.ExistsEmail(email, userId);
        }

        public static bool ExistsSellerCode(string code, int userId)
        {
            return DB.ExistsSellerCode(code, userId);
        }

        public static string GetSellerNameBySellerId(int sellerId)
        {
            return DB.GetSellerNameBySellerId(sellerId);
        }

        public static int SetClient(int mngUserId, ClientData d)
        {
            int prodCount = 0;
            foreach (Product p in d.Products)
            {
                if (p.LicenseCnt < 1 || p.Selected == false)
                    continue;
                prodCount++;
            }
            List<int> prodIds = new List<int>();
            List<int> prodLic = new List<int>();
            List<bool> prodDemo = new List<bool>();

            DateTime[,] prodDates = new DateTime[prodCount, 2];
            //List<DateTime[]> prodDates = new List<DateTime[]>();

            int cnt = 0;
            for (int i = 0; i < d.Products.Count; i++)
            {
                try
                {
                    Product p = d.Products[i];
                    if (p.LicenseCnt < 1 || p.Selected == false)
                        continue;
                    prodIds.Add(p.ProductId);
                    prodLic.Add(p.LicenseCnt);
                    prodDemo.Add(p.Demo);

                    prodDates[cnt, 0] = p.StartDate.Value;
                    prodDates[cnt, 1] = p.EndDate.Value;
                    cnt++;
                }
                catch (Exception)
                {
                    File.AppendAllText("C:/testdates.txt", JsonConvert.SerializeObject(d));
                }
               
            }

            return DB.SetClient(mngUserId, d.ClientId, d.ClientName, d.SellerId, prodIds.ToArray(), prodLic.ToArray(), prodDates, prodDemo.ToArray(), d.Note, d.CountryId);
        }

        public static IEnumerable<Product> GetProductsList(int userId)
        {
            var products = new List<Product>();

            foreach (var r in DB.GetProductsList(userId))
            {
                var product = new Product();
                product.ProductId = int.Parse(r["product_id"].ToString());
                product.ProductName = r["product_name"].ToString();
                product.IsActive = Convert.ToBoolean(r["is_active"].ToString());

                products.Add(product);
            }

            return products;
        }

        public static void AddUserSearch(int userId, string searchText, SearchBox searchBoxFilters, int maxCount, int productId)
        {
            Common.FixSearchBoxFiltersDatesForJSON(searchBoxFilters);
            string searchBoxFiltersJSON = JsonConvert.SerializeObject(searchBoxFilters, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
            DB.AddUserSearch(userId, searchText, searchBoxFiltersJSON, maxCount, productId);
        }

        public static void SetUserLinksInNewTab(int userId, bool linksInNewTab)
        {
            DB.SetUserLinksInNewTab(userId, linksInNewTab);
        }

        public static void UpdateUserCommonSettings(int userId, bool linksInNewTab, bool showFreeDocuments)
        {
            DB.UpdateUserCommonSettings(userId, linksInNewTab, showFreeDocuments);
        }

        public static bool HasAcceptedCookies(int? userId, string ip)
        {
            return DB.GetCookiesAgreement(userId, ip);
        }

        public static void AddCookiesAgreement(int? userId, string ip)
        {
            DB.AddCookiesAgreement(userId, ip);
        }

        public static void AddUserDoc(int userId, int docLangId, int productId, int? folderId)
        {
            DB.AddUserDoc(userId, docLangId, productId, folderId);
        }

        public static void DelUserDoc(int userId, int docLangId, int productId)
        {
            DB.DelUserDoc(userId, docLangId, productId);
        }

        public static void DelAllUserDocs(int userId, int? folderId)
        {
            DB.DelAllUserDocs(userId, folderId);
        }

        public static int GetUserDocsCount(int userId, int productId)
        {
            return DB.GetUserDocsCount(userId, productId);
        }

        public static bool GetUserHasDocument(int userId, int docLangId, int productId)
        {
            return DB.GetUserHasDocument(userId, docLangId, productId);
        }

        public static IEnumerable<Document> GetUserDocs(int userId, int siteLangId)
        {
            var docsFromDB = DB.GetUserDocs(userId, siteLangId);

            foreach (var row in docsFromDB)
            {
                FavouriteDoc doc = new FavouriteDoc();
                doc.DocLangId = int.Parse(row["doc_lang_id"].ToString());
                doc.DocType = int.Parse(row["doc_type"].ToString());
                doc.LangId = int.Parse(row["lang_id"].ToString());
                doc.Title = row["full_title"].ToString();
                doc.DocNumber = row["doc_number"].ToString();
                doc.Country = row["country"].ToString();
                doc.AddedDate = row["add_date"].ToString();

                // Keywords
                Type keywordsT = row["keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    doc.SetKeywords((string[,])row["keywords"], siteLangId);

                // Summaries
                Type summariesT = row["summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    doc.SetSummaries((string[,])row["summaries"], siteLangId);

                yield return doc;
            }
        }

        public static IEnumerable<Document> GetUserDocsFolder(int userId, int siteLangId, string orderBy, string orderDir, int productId, int? folderId)
        {
            var docsFromDB = DB.GetUserDocsFolder(userId, siteLangId, orderBy, orderDir, productId, folderId);

            foreach (var row in docsFromDB)
            {
                FavouriteDoc doc = new FavouriteDoc();
                doc.DocLangId = int.Parse(row["doc_lang_id"].ToString());
                doc.DocType = int.Parse(row["doc_type"].ToString());
                doc.DocDate = Convert.ToDateTime(row["doc_date"]);
                doc.LangId = int.Parse(row["lang_id"].ToString());
                doc.Title = row["full_title"].ToString();
                doc.DocNumber = row["doc_number"].ToString();
                doc.Country = row["country"].ToString();
                doc.AddedDate = row["add_date"].ToString();

                // Keywords
                Type keywordsT = row["keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    doc.SetKeywords((string[,])row["keywords"], siteLangId);

                // Summaries
                Type summariesT = row["summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    doc.SetSummaries((string[,])row["summaries"], siteLangId);

                doc.Publisher = row["publisher"].ToString();

                yield return doc;
            }
        }

        private static void DeleteSessionTempFolder(string sessionFolder)
        {
            if (Directory.Exists(sessionFolder))
            {
                System.IO.Directory.Delete(sessionFolder);
            }
        }

        public static int[] GetUserLangPrefForSearch(int userId, int siteLangId)
        {
            return DB.GetUserLangPrefForSearch(userId, siteLangId);
        }

        public static int AddAuthToken(int userId, string token, DateTime expire)
        {
            return DB.AddAuthToken(userId, token, expire);
        }

        public static int CheckAuthToken(int authTokenId, string token)
        {
            return DB.CheckAuthToken(authTokenId, token);
        }

        public static string CreateToken(int userId, int expireDays)
        {
            string token = UserMng.CreateTOKEN();
            DateTime expire = DateTime.Now.AddDays(expireDays);
            int authTokenId = UserMng.AddAuthToken(userId, token, expire);

            return authTokenId.ToString() + ":" + token;
        }

        public static void DelAuthToken(int id)
        {
            DB.DelAuthToken(id);
        }

        public static void SetUserValidEMail(int userId)
        {
            DB.SetUserValidEMail(userId);
        }

        public static SellerData GetSeller(int id)
        {
            SellerData seller = null;
            var row = DB.GetSeller(id);
            if (row != null)
            {
                seller = new SellerData();
                //seller.SellerId = (row["id"] != DBNull.Value) ? (int?)row["id"] : null;
                seller.SellerId = id;
                seller.UserId = Convert.ToInt32(row["user_id"]);
                seller.UserType = (UserTypes)row["usertype_id"];
                seller.ParentSellerId = (row["parent_seller_id"] != DBNull.Value) ? (int?)row["parent_seller_id"] : (int?)null;
                seller.Phone = row["phone"].ToString();
                seller.Email = row["email"].ToString();
                seller.Code = row["code"].ToString();
                seller.CountryId = (row["country_id"] != DBNull.Value) ? (int?)row["country_id"] : null;
                seller.CountryName = row["country_name"].ToString();
                seller.FullName = row["fullname"].ToString();
                seller.FullNameEN = row["fullname"].ToString().Unidecode();
            }
            return seller;
        }

        public static void AddDemoProducts(int clientId, int userId, int days, int[] productIds)
        {
            // DB.AddDemoProducts(clientId, userId, days, productIds);
        }

        public static IEnumerable<UserFolderData> GetUserFoldersParent(int userId, int productId, int? parentFolderId)
        {
            var foldersFromDb = DB.GetUserFoldersParent(userId, productId, parentFolderId);
            ICollection<UserFolderData> ufData = new List<UserFolderData>();
            foreach (var row in foldersFromDb)
            {
                int id = Convert.ToInt32(row["folder_id"]);
                string name = row["folder_name"].ToString();
                bool isEmpty = Convert.ToBoolean(row["is_empty"]);
                int documentsCount = Convert.ToInt32(row["documents_count"]);

                ufData.Add(new UserFolderData(id, name, isEmpty, documentsCount));
            }

            return ufData;
        }

        public static bool AddUserDocToFolder(int userDocId, int folderId, int userId)
        {
            return DB.AddUserDocToFolder(userDocId, folderId, userId);
        }

        public static UserFolderData AddUserFolder(int userId, int productId, string folderName, int? parentId)
        {
            var row = DB.AddUserFolder(userId, productId, folderName, parentId);
            int newFolderId = Convert.ToInt32(row["new_folder_id"]);
            string newName = row["new_folder_name"].ToString();
            var ufData = new UserFolderData(newFolderId, folderName, true);

            return ufData;
        }

        public static bool RenameUserFolder(int userId, int folderId, string newFolderName)
        {
            return DB.RenameUserFolder(userId, folderId, newFolderName);
        }

        public static void DeleteUserFolder(int userId, int folderId)
        {
            DB.DeleteUserFolder(userId, folderId);
        }

        public static int GetUserDocsCountFolder(int userId, int productId, int? folderId)
        {
            return DB.GetUserDocsCountFolder(userId, productId, folderId);
        }

        public static void MoveDocsFolderToFolder(int userId, int productId, int? fromId, int? toId)
        {
            DB.MoveDocsFolderToFolder(userId, productId, fromId, toId);
        }

        public static IEnumerable<string> GetUserSearchesMultiDictObjects()
        {
            var entriesFromDB = DB.GetUserSearchesMultiDictObjects();

            return entriesFromDB.Select(i => i.ToString()).ToList();
        }

        public static string[,] TransformUserSearchesMultiDictObjects()
        {
            var initListAsString = DB.GetUserSearchesMultiDictObjects();
            var res = new string[initListAsString.Count(), 2];
            //var res = new Dictionary<int, string>();
            var curIndex = 0;

            foreach (var item in initListAsString)
            {
                var curModel = JsonConvert.DeserializeObject<SearchDetails>(item[0].ToString());
                var dictModel = new SearchDetailsMultiDict();
                dictModel.Text = curModel.Cases.MultiDictionaryText;
                dictModel.LogicalType = curModel.Cases.MultiDictionaryLogicalType;
                dictModel.SelectedIds = curModel.Cases.MultiDictionarySelectedIds;
                dictModel.SelectedQueryTexts = curModel.Cases.MultiDictionarySelectedQueryTexts;

                curModel.Cases.MultiDict = dictModel;

                res[curIndex, 0] = item[1].ToString();
                res[curIndex, 1] = JsonConvert.SerializeObject(curModel);

                curIndex++;
            }
            
            return res;
        }

        public static void UpdateUserSearchesMultiDictObjects(string[,] updateModel) // the model will need to be more complex
        {
            DB.UpdateUserSearchesMultiDictObjects(updateModel);
        }

        public static string AddUservalidateToken(int userId)
        {
            return DB.AddUserValidateToken(userId);
        }

        public static bool ActivateUser(Guid token)
        {
            return DB.ActivateUser(token);
        }
    }
}
