namespace Interlex.BusinessLayer.Models
{
    using System;
    
    /// <summary>
    /// Used to hold an object with info for a related document to another document (ex: consolidated version for caselaw or associated doc for legislation)
    /// </summary>
    public class RelatedDoc
    {
        public string DocNumber { get; set; }

        public string DocLangId { get; set; }

        public string DateAsString { get; set; }

        public RelatedDoc(string docNumber, string docLangId, string dateAsString)
        {
            this.DocNumber = docNumber;
            this.DocLangId = docLangId;
            this.DateAsString = dateAsString;
        }
    }
}
