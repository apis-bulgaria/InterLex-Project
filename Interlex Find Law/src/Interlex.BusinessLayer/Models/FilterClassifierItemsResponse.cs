using Interlex.BusinessLayer.Enums;
using System.Collections.Generic;

namespace Interlex.BusinessLayer.Models
{
    public class FilterClassifierItemsResponse
    {
        public List<ClassifierItem> Data { get; set; } = new List<ClassifierItem>();

        public SearchSources SearchSource { get; set; }

        public bool IsCaseLawFolder { get; set; }
    }
}
