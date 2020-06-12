namespace Interlex.BusinessLayer.Models
{
    using System.Collections.Generic;
    public class DocHighlightSearchParams
    {
        public string SearchText { get; set; }
        public string MultilingualSearchedText { get; set; }

        public bool ExactMatch { get; set; }

        public string SearchedCelex { get; set; }

        public List<string> SearchedPars { get; set; }

        public DocHighlightSearchParams(string searchText, string multilingualSearchedText, bool exactMatch, string searchedCelex, List<string> searchedPars)
        {
            this.SearchText = searchText;
            this.MultilingualSearchedText = multilingualSearchedText;
            this.ExactMatch = exactMatch;
            this.SearchedCelex = searchedCelex;
            this.SearchedPars = searchedPars;
        }

        public DocHighlightSearchParams() { }
    }
}
