namespace Interlex.BusinessLayer.Models
{
    using System;
    using Interlex.BusinessLayer.Enums;
    using ApisLucene.Classes.Common;

    /// <summary>
    ///  /// <summary>
    /// Used for Law tab details when showing / loading previously done search. Used via deserialization.
    /// </summary>
    public class SearchDetailsLaw
    {
        public LegislationType LegislationType { get; set; }

        public bool OnlyInTitles { get; set; }

        public bool TranslateSearchText { get; set; }

        public bool OnlyInActualActs { get; set; }

        public bool OnlyInBasicActs { get; set; }

        public string NatID_ECLI { get; set; }

        public LawDateTypeEnum LawDateType { get; set; }

        public string DocNumber { get; set; }
        public string Year { get; set; }

        public OJTypeEnum OJSeries { get; set; }
        public string OJYear { get; set; }

        public DateTime DateFrom { get; set; }

        public string DateFromShort { get; set; }

        public DateTime DateTo { get; set; }

        public string DateToShort { get; set; }

        //helps later on client side for filtering and corectly parsing dates
        public string DateDefaultByCultureShort { get; set; }

        public string Number { get; set; }

        public string PageNumber { get; set; }

        public SearchDetailsClassifier DocumentTypes { get; set; }

        public SearchDetailsClassifier DirectoryLegislation { get; set; }

        public SearchDetailsClassifier Eurovoc { get; set; }

        public SearchDetailsClassifier Jurisdictions { get; set; }

        public SearchDetailsClassifier Syllabus { get; set; }

        public SearchDetailsClassifier SubjectMatter { get; set; }

        public SearchDetailsClassifier ActJurisdictions { get; set; }

        public SearchDetailsMultiDict MultiDict { get; set; }
    }
}
