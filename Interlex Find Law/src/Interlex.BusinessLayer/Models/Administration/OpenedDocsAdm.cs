namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct OpenedDocsFilters 
    {
       public string DocumentTitle { get; set; }

       public DateTime? DateFrom { get; set; }

       public DateTime? DateTo { get; set; }

       public int? UserId { get; set; }

       public string Username { get; set; }

       public int? ClientId { get; set; }

       public string ClientName { get; set; }

       public string Note { get; set; }

       public int? SellerId { get; set; }

       public string SellerName { get; set; }
    }

    public struct OpenedDocsDetails 
    {
        public string DocumentTitle { get; set; }

        public string DateOpened { get; set; }

        public int DocType { get; set; }

        public int DocLangId { get; set; }

        public string Username { get; set; }

        public string ClientName { get; set; }

        public string SellerName { get; set; }

        public string Note { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int TotalQueryCount { get; set; }
    }

    public struct OpenedDocsCountsDetails 
    {
        public string Username { get; set; }

        public string ClientName { get; set; }

        public string SellerName { get; set; }

        public string Note { get; set; }

        public string Date { get; set; }

        public int Count { get; set; }

        public int TotalQueryCount { get; set; }
    }

    public class OpenedDocsAdm
    {
        OpenedDocsFilters Filters { get; set; }
        List<OpenedDocsDetails> Items { get; set; }

        public OpenedDocsAdm() 
        {
            this.Filters = new OpenedDocsFilters 
            {
                DocumentTitle = String.Empty,
                DateFrom = null,
                DateTo = null,
                UserId = null,
                Username = String.Empty,
                ClientId = null,
                ClientName = String.Empty,
                Note = String.Empty,
                SellerId = null,
                SellerName = String.Empty
            };

            this.Items = new List<OpenedDocsDetails>();
        }

        public OpenedDocsAdm(OpenedDocsFilters filters) 
        {
            this.Filters = filters;
            this.Items = new List<OpenedDocsDetails>();
        }

        public static List<OpenedDocsDetails> GetOpenedDocsData(string documentTitle, int userId, int clientId, int sellerId, string note, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir, int currentSellerId, int productId)
        {
            var items = new List<OpenedDocsDetails>();

            foreach (var r in DataLayer.DB.GetOpenedDocuments(documentTitle, userId, clientId, sellerId, note, dateFrom, dateTo, start, take, orderby, orderdir, currentSellerId, productId))
            {
                OpenedDocsDetails item = new OpenedDocsDetails();

                item.DocumentTitle = r["document_title"].ToString();
                item.DateOpened = r["date_opened"].ToString();
                item.Username = r["username"].ToString();
                item.ClientName = r["client_name"].ToString();
                //populate seller name;
               // item.SellerName = String.Empty;
                item.SellerName = r["seller_name"].ToString();
                item.Note = r["note"].ToString();
                item.DocType = int.Parse(r["doc_type"].ToString());
                item.DocLangId = int.Parse(r["doc_lang_id"].ToString());
                item.TotalQueryCount = int.Parse(r["total_query_count"].ToString());
                item.ProductId = int.Parse(r["product_id"].ToString());

                items.Add(item);
            }

            return items;
        }

        public static List<OpenedDocsCountsDetails> GetOpenedDocsCountsData(string username, string clientName, int sellerId, string note, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir, int currentSellerId, int productId) 
        {
            var items = new List<OpenedDocsCountsDetails>();

            foreach (var r in DataLayer.DB.GetOpenedDocumentsCounts(username, clientName, sellerId, note, dateFrom, dateTo, start, take, orderby, orderdir, currentSellerId, productId))
            {
                OpenedDocsCountsDetails item = new OpenedDocsCountsDetails();

                item.ClientName = r["client_name"].ToString();
                item.Count = int.Parse(r["count"].ToString());
                item.Date = r["date"].ToString();
                item.Note = r["note"].ToString();
                item.SellerName = r["seller_name"].ToString();
                item.Username = r["username"].ToString();
                item.TotalQueryCount = int.Parse(r["total_query_count"].ToString());

                items.Add(item);
            }

            return items;
        }

        public static List<OpenedDocsCountsDetails> GetOpenedDocsNullCountsData(string username, string clientName, string sellerName, string note, DateTime dateFrom, DateTime dateTo, int start, int take, string orderby, string orderdir)
        {
            var items = new List<OpenedDocsCountsDetails>();

            foreach (var r in DataLayer.DB.GetOpenedDocumentsNullCounts(username, clientName, sellerName, note, dateFrom, dateTo, start, take, orderby, orderdir))
            {
                OpenedDocsCountsDetails item = new OpenedDocsCountsDetails();

                item.ClientName = r["client_name"].ToString();
                item.Count = 0; //Always zero
                item.Date = r["date"].ToString();
                item.Note = r["note"].ToString();
                item.SellerName = r["seller_name"].ToString();
                item.Username = r["username"].ToString();
                item.TotalQueryCount = int.Parse(r["total_query_count"].ToString());

                items.Add(item);
            }

            return items;
        }

        public static List<OpenedDocsDetails> GetOpenedDocsData(OpenedDocsFilters filters)
        {
            var items = new List<OpenedDocsDetails>();

            return items;
        }

        public static int GetOpenedDocsCount(string documentTitle, string username, string clientName, string sellerName, string note, DateTime dateFrom, DateTime dateTo) 
        {
            return DataLayer.DB.GetOpenedDocumentsCount(documentTitle, username, clientName, sellerName, note, dateFrom, dateTo);
        }

        public static int GetOpenedDocsCountsCount(string username, string clientName, string sellerName, string note, DateTime dateFrom, DateTime dateTo)
        {
            return DataLayer.DB.GetOpenedDocumentsCountsCount(username, clientName, sellerName, note, dateFrom, dateTo);
        }

        public static List<PieChartData> GetOpenedDocumentsStatByLanguage(int currentSellerId) 
        {
            var items = new List<PieChartData>();

            var statsFromDB = Interlex.DataLayer.DB.GetOpenedDocumentsStatByLanguage(currentSellerId);

            foreach (var r in statsFromDB)
            {
                var item = new PieChartData();

                item.Label = r["lang_name"].ToString();
                item.Data = int.Parse(r["count"].ToString());

                items.Add(item);
            }

            return items;
        }

        public static List<PieChartData> GetOpenedDocumentsStatByProduct(int currentSellerId)
        {
            var items = new List<PieChartData>();

            var statsFromDB = Interlex.DataLayer.DB.GetOpenedDocumentsStatByProduct(currentSellerId);

            foreach (var r in statsFromDB)
            {
                var item = new PieChartData();

                item.Label = r["product_name"].ToString();
                item.Data = int.Parse(r["count"].ToString());
                item.Id = int.Parse(r["product_id"].ToString());

                items.Add(item);
            }

            return items;
        }

        public static List<PieChartData> GetOpenedDocumentsStatByType(int currentSellerId)
        {
            var items = new List<PieChartData>();

            var statsFromDB = Interlex.DataLayer.DB.GetOpenedDocumentsStatByType(currentSellerId);

            foreach (var r in statsFromDB)
            {
                var item = new PieChartData();

                if (r["type_id"].ToString() == "3")
                {
                    if (r["product_id"].ToString() == "1")
                    {
                        item.Label = "Legal Doctrine";
                    }
                    else if (r["product_id"].ToString() == "2")
                    {
                        item.Label = "Finance doc";
                    }

                    item.Data = int.Parse(r["count"].ToString());
                    items.Add(item);
                }
                else
                {
                    if (items.Any(i => i.Label == r["type_id"].ToString()))
                    {
                        var curMemorizedItem = items.Where(i => i.Label == r["type_id"].ToString()).FirstOrDefault();
                        items.Remove(curMemorizedItem);
                        curMemorizedItem.Data += int.Parse(r["count"].ToString());
                        curMemorizedItem.Label = r["type_id"].ToString();
                        items.Add(curMemorizedItem);
                    }
                    else
                    {
                        item.Data = int.Parse(r["count"].ToString());
                        item.Label = r["type_id"].ToString();
                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }
}
