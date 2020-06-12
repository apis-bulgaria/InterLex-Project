namespace Interlex.DataLayer.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class LawRelationUpdateModel
    {
        [JsonProperty("from_celex")]
        public string FromCelex { get; set; }

        [JsonProperty("from_article")]
        public string FromArticle { get; set; }

        [JsonProperty("to_celex")]
        public string ToCelex { get; set; }

        [JsonProperty("to_article")]
        public string ToArticle { get; set; }

        [JsonProperty("link_ids")]
        public IEnumerable<int> LinkIds { get; set; } = new List<int>();

        [JsonProperty("to_doc_par_id")]
        public int ToDocParId { get; set; }
    }
}
