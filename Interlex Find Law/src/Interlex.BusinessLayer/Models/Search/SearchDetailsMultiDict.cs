namespace Interlex.BusinessLayer.Models
{
    public class SearchDetailsMultiDict
    {
        public string Text { get; set; }

        /// <summary>
        /// False for OR; True for AND;
        /// </summary>
        public bool LogicalType { get; set; } = false; // Default - OR

        public string SelectedIds { get; set; }

        public string SearchText { get; set; }

        public string SelectedQueryTexts { get; set; }
    }
}
