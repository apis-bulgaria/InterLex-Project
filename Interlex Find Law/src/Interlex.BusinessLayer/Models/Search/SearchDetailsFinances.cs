using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class SearchDetailsFinances
    {
        public bool OnlyInTitles { get; set; }

        public bool Keywords { get; set; }

        public bool SearchInSummaries { get; set; }

        #region DocType booleans
        public bool DocTypeStandarts { get; set; }

        public bool DocTypeReglaments { get; set; }

        public bool DocTypeDirectives { get; set; }

        public bool DocTypeEuCaseLaw { get; set; }

        public bool DocTypeNationalCaseLaw { get; set; }

        public bool DocTypeSummaries { get; set; }

        #endregion

        public DateTime DateFrom { get; set; }

        public string DateFromShort { get; set; }

        public DateTime DateTo { get; set; }

        public string DateToShort { get; set; }

        //helps later on client side for filtering and corectly parsing dates
        public string DateDefaultByCultureShort { get; set; }

        //enactment 
        public string EnactmentText { get; set; }

        public int? EnactmentDocLangId { get; set; }

        public string EnactmentCelex { get; set; }

        public string ProvisionText { get; set; }

        public string ProvisionId { get; set; }

        public string ProvisionParOriginal { get; set; }

        public SearchDetailsClassifier DocumentTypes { get; set; }

        public SearchDetailsClassifier EuroFinance { get; set; }
    }
}
