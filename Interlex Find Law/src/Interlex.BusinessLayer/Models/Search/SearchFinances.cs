namespace Interlex.BusinessLayer.Models
{
    using Enums;
    using System;

    public class SearchFinances : AdvSearchTab
    {
        public bool OnlyInTitles { get; set; }

        public bool Keywords { get; set; }

        public bool SearchInSummaries { get; set; }

        // public bool ConceptsAndDefinitions { get; set; }

        public DatePeriodType DatePeriodType { get; set; } = Interlex.BusinessLayer.Enums.DatePeriodType.Date;

        public DateTime? DateFrom { get; set; } = default(DateTime);

        public DateTime? DateTo { get; set; } = default(DateTime);

        public string EnactmentText { get; set; }
        public int? EnactmentDocLangId { get; set; }

        public string EnactmentCelex { get; set; }

        public string ProvisionText { get; set; }
        public string ProvisionId { get; set; }

        public string ProvisionParOriginal { get; set; }

        public CheckTreeView DocumentTypes { get; set; } // not used; remove later

        public CheckTreeView EuroFinance { get; set; }

        #region DocumentType checkboxes

        public bool DocTypeStandarts { get; set; }

        public bool DocTypeReglaments { get; set; }

        public bool DocTypeDirectives { get; set; }

        public bool DocTypeEuCaseLaw { get; set; }

        public bool DocTypeNationalCaseLaw { get; set; }

        public bool DocTypeSummaries { get; set; }

        #endregion

        public SearchFinances(int langId) : base(langId)
        {
            this.DocumentTypes = new CheckTreeView(langId); // remove later
            this.EuroFinance = new CheckTreeView(langId);
        }
    }
}
