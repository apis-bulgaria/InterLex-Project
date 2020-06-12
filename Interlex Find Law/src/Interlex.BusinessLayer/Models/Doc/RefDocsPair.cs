namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class RefDocsPair
    {
        public Document MainDocument { get; set; }

        public Document Document { get; set; }

        public HashSet<ArticleData> ArticleData { get; set; } = new HashSet<ArticleData>();

        public int ProductOrder { get; set; }

        public int ArticleOrder { get; set; }

        public RefDocsPair()
        {
        }

        public RefDocsPair(Document mainDocument, Document document, string refArticles, int productOrder, int articleOrder)
        {
            this.MainDocument = mainDocument;
            this.Document = document;
            JArray json = JsonConvert.DeserializeObject(refArticles) as JArray;
            if (json != null && json.Count > 0)
            {
                var articleData = new List<ArticleData>(json.Select(x => new ArticleData(x["eid"].ToString(), x["num"].ToString())))
                    .OrderBy(x => Convert.ToInt32(x.Num.Split(new char[] { ' ' })[1])).ToList();
                this.ArticleData = new HashSet<ArticleData>(articleData);
            }

            this.ProductOrder = productOrder;
            this.ArticleOrder = articleOrder;
        }
    }
}
