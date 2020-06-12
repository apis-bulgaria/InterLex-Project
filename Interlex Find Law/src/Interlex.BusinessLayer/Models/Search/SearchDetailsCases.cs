namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interlex.BusinessLayer.Enums;
    using Newtonsoft.Json;

    /// <summary>
    /// Used for Cases tab details when showing / loading previously done search. Used via deserialization.
    /// </summary>
    public class SearchDetailsCases
    {
        public bool OnlyInTitles { get; set; }

        public bool TranslateSearchText { get; set; }

        public string CaseNumber { get; set; }

        public string Year { get; set; }

        public string Parties { get; set; }

        public string NatID_ECLI { get; set; }

        public DateTime DateFrom { get; set; }

        public string DateFromShort { get; set; }

        public DateTime DateTo { get; set; }

        public string DateToShort { get; set; }

        public string Applicant { get; set; }
        
        //helps later on client side for filtering and corectly parsing dates
        public string DateDefaultByCultureShort { get; set; }

        public string EnactmentText { get; set; }
        public int? EnactmentDocLangId { get; set; }

        public string EnactmentCelex { get; set; }
        public string ProvisionText { get; set; }
        public string ProvisionId { get; set; }

        public string ProvisionParOriginal { get; set; }

        public SearchDetailsClassifier AdvocateGeneral { get; set; }

        public SearchDetailsClassifier CourtsFolders { get; set; }

        public SearchDetailsClassifier DocumentTypes { get; set; }

        public SearchDetailsClassifier DirectoryCaseLaw { get; set; }

        public SearchDetailsClassifier EuroCases { get; set; }

        public SearchDetailsClassifier Eurovoc { get; set; }

        public SearchDetailsClassifier JudgeRapporteur { get; set; }

        public SearchDetailsClassifier ProcedureType { get; set; }

        public SearchDetailsClassifier Syllabus { get; set; }

        public SearchDetailsClassifier SubjectMatter { get; set; }

        public SearchDetailsClassifier States { get; set; } // Respondent state - 1021
        public SearchDetailsClassifier HudocImportance { get; set; } // Importance level - 1020
        public SearchDetailsClassifier HudocArticleViolation { get; set; } // Violation / non-violation - 1013
        public SearchDetailsClassifier Hudoc { get; set; } // Hudoc classifier - 1012
        public SearchDetailsClassifier HudocApplicability { get; set; } // Hudoc applicability - 1018
        public SearchDetailsClassifier Courts { get; set; } // Courts
        
        public SearchDetailsClassifier HudocArticles { get; set; } // Used in provision (if ECHRReferedActType: convention and protocols)

        public SearchDetailsClassifier RulesOfTheCourt { get; set; } // Used in provision (if ECHRReferedActType: court rules)

        public CaseLawType CaseLawType { get; set; }

        public ECHRReferedActType ECHRReferedActType { get; set; }

        public int? ReferedActECHRDocLangId { get; set; } // null if ECHRReferedActType is not 3 (caselaw)

        public string ReferedActTitle { get; set; }

        public string ApplicationNumber { get; set; }

        public SearchDetailsMultiDict MultiDict { get; set; }


        /*** ***/
        // The following items are only temporarily present in order for old entries to be loaded correctly. Remove them at some point when entries are pre-structured

        /// <summary>
        /// Builded text for the multidictionary that is passed as a search param
        /// </summary>
        [JsonIgnore]
        public string MultiDictionaryText { get; set; }

        [JsonProperty("MultiDictionaryText")]
        private string MultiDictionaryTextSetter
        {
            // get is intentionally omitted here
            set { MultiDictionaryText = value; }
        }

        /// <summary>
        /// False for OR; True for AND;
        /// </summary>
        [JsonIgnore]
        public bool MultiDictionaryLogicalType { get; set; }

        [JsonProperty("MultiDictionaryLogicalType")]
        private bool MultiDictionaryLogicalTypeSetter
        {
            // get is intentionally omitted here
            set { MultiDictionaryLogicalType = value; }
        }


        /// <summary>
        /// JSON holding selected ids that is later populated in a cookie on the client
        /// </summary>
        [JsonIgnore]
        public string MultiDictionarySelectedIds { get; set; }

        [JsonProperty("MultiDictionarySelectedIds")]
        private string MultiDictionarySelectedIdsSetter
        {
            // get is intentionally omitted here
            set { MultiDictionarySelectedIds = value; }
        }
        
        /// <summary>
        /// JSON holding selected ids with corresponding texts that are later to be populated in a cookie on the client
        /// </summary>
        [JsonIgnore]
        public string MultiDictionarySelectedQueryTexts { get; set; }

        [JsonProperty("MultiDictionarySelectedQueryTexts")]
        private string MultiDictionarySelectedQueryTextsSetter
        {
            // get is intentionally omitted here
            set { MultiDictionarySelectedQueryTexts = value; }
        }

        // END of prorperties for removal
        /*** ***/
    }
}
