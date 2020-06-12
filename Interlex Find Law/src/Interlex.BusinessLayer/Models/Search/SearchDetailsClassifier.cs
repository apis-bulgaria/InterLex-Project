namespace Interlex.BusinessLayer.Models
{
    /// <summary>
    /// Used for any classifier details when showing / loading a previously done search
    /// </summary>
    public class SearchDetailsClassifier
    {
      //  public string KeyPaths { get; set; }

        public string[] TitlePaths { get; set; }

       // public string SelectedIds { get; set; }

        public SearchDetailsClassifier() 
        {
            this.TitlePaths = new string[50];
        }
    }
}
