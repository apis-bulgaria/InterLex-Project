namespace Interlex.BusinessLayer.Models
{
    using System;
    using ApisLucene.Classes.Common;
    using Enums;

    public class SearchLaw : AdvSearchTab
    {
        public LegislationType LegislationType { get; set; }

        public bool OnlyInTitles { get; set; }

        public bool TranslateSearchText { get; set; }

        public bool OnlyInActualActs { get; set; }

        public bool OnlyInBasicActs { get; set; }

        public string NatID_ELI { get; set; }

        public DatePeriodType DatePeriodType { get; set; }
        //public LawDateType LawDateType { get; set; }

        public LawDateTypeEnum LawDateType { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // public int? Year { get; set; }
        //  public int? Month { get; set; }
        // public int? Day { get; set; }
        public int? Number { get; set; }
        public int? PageNumber { get; set; }

        public SearchLaw(int langId)
            : base(langId)
        {
            DocumentTypes = new CheckTreeView(langId);
            Jurisdictions = new CheckTreeView(langId);
          //  DirectoryLegislation = new CheckTreeView(langId);
         //   Eurovoc = new CheckTreeView(langId);
            Syllabus = new CheckTreeView(langId);
         //   SubjectMatter = new CheckTreeView(langId);
            ActJurisdictions = new CheckTreeView(langId);
            LegislationType = LegislationType.EU; // default
        }

        //change this
        public string DocNumber { get; set; }
        public string Year { get; set; }

        public OJTypeEnum OJSeries { get; set; }
        public string OJYear { get; set; }

        public CheckTreeView DocumentTypes { get; set; }
        public CheckTreeView Jurisdictions { get; set; }
       // public CheckTreeView DirectoryLegislation { get; set; }
    //    public CheckTreeView Eurovoc { get; set; }
        public CheckTreeView Syllabus { get; set; }
   //     public CheckTreeView SubjectMatter { get; set; }

        public CheckTreeView ActJurisdictions { get; set; }
        
        public SearchMultiDict MultiDict { get; set; }
    }
}
