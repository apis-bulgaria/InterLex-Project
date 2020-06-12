using Newtonsoft.Json;

namespace Interlex.BusinessLayer.Models
{
    public class DocLink
    {
        [JsonProperty(PropertyName = "id")]
        public int DocLangId { get; set; }
        [JsonProperty(PropertyName = "dt")]
        public int DocType { get; set; }
        public int ParId { get; set; }
        [JsonProperty(PropertyName = "t")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "ol")]
        public string OriginalLink { get; set; }
        [JsonProperty(PropertyName = "publisher")]
        public string Publisher { get; set; }
    }
}
