namespace Interlex.BusinessLayer.Models
{
    public class SearchMultiDict
    {
        public string Text { get; set; }

        /// <summary>
        /// False for OR; True for AND;
        /// </summary>
        public bool LogicalType { get; set; } = false; // Default - OR

        public string SelectedIds { get; set; } // Cookie is stored here

        public string SearchText { get; set; } // Used only for later loading of the search result

        public string SelectedQueryTexts { get; set; } // Used only for later loading of the search result;
    }
}
