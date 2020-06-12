namespace Interlex.BusinessLayer.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ArticleCorrelation
    {
        public string DocNumber { get; set; }

        public int DocLangId { get; set; }

        public HashSet<ArticleData> ArticleData { get; set; } = new HashSet<ArticleData>();

        public ArticleCorrelation()
        {
        }

        public ArticleCorrelation(string docNumber, int docLangId, string articleData)
        {
            this.DocNumber = docNumber;
            this.DocLangId = docLangId;
            JArray json = JsonConvert.DeserializeObject(articleData) as JArray;
            if (json != null && json.Count > 0)
            {
                this.ArticleData = new HashSet<ArticleData>(json.Select(x => new ArticleData(x["eid"].ToString(), x["num"].ToString())));
            }
        }
    }
}
