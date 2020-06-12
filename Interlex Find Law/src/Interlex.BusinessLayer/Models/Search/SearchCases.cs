namespace Interlex.BusinessLayer.Models
{
    using System;
    using Interlex.BusinessLayer.Enums;

    public class SearchCases : AdvSearchTab
    {
        //public string SearchText { get; set; }

        public bool OnlyInTitles { get; set; }

        public bool TranslateSearchText { get; set; }

        public string NatID_ECLI { get; set; }
        public string Parties { get; set; }

        public string CaseNumber { get; set; }
        public string Year { get; set; }

        //public int? YearFrom { get; set; }
        //public int? YearTo { get; set; }

        public CheckTreeView DocumentTypes { get; set; }

        public bool? IsIndexEnactmentSearch { get; set; } = false;

        public string EnactmentText { get; set; }
        public int? EnactmentDocLangId { get; set; }

        public string EnactmentCelex { get; set; }

        public string ProvisionText { get; set; }
        public string ProvisionId { get; set; }

        public string ProvisionParOriginal { get; set; }

        public string Applicant { get; set; }
        public string ApplicationNumber { get; set; }

        public DatePeriodType DatePeriodType { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public CheckTreeView CourtsFolders { get; set; }
      //  public CheckTreeView DirectoryCaseLaw { get; set; }
        public bool DirectoryCaseLawFull { get; set; }
    //    public CheckTreeView EuroCases { get; set; }
      //  public CheckTreeView Eurovoc { get; set; }
        public CheckTreeView Syllabus { get; set; }
      //  public CheckTreeView SubjectMatter { get; set; }
      //  public CheckTreeView AdvocateGeneral { get; set; }
     //   public CheckTreeView JudgeRapporteur { get; set; }
     //   public CheckTreeView ProcedureType { get; set; }

        // ECHR
     //   public CheckTreeView States { get; set; } // Respondent state - 1021
      //  public CheckTreeView HudocImportance { get; set; } // Importance level - 1020
      //  public CheckTreeView HudocArticleViolation { get; set; } // Violation / non-violation - 1013
     //   public CheckTreeView Hudoc { get; set; } // Hudoc classifier - 1012
    //    public CheckTreeView HudocApplicability { get; set; } // Hudoc applicability - 1018
   //     public CheckTreeView Courts { get; set; } // Courts

       public CheckTreeView RulesOfTheCourt { get; set; } // Rules of the court - 1025

   //     public CheckTreeView HudocArticles { get; set; } // Hudoc Articles - 1019

        public ECHRReferedActType ECHRReferedActType { get; set; }

        //  public string ReferedActECHRClassifierGUID { get; set; } // refered act classifier guid if options 1 & 2

        public int? ReferedActECHRDocLangId { get; set; } // refered act docLangId if selected from search list from option 3

        public string ReferedActTitle { get; set; } // holding act title for a later visualization in my searches

        public CaseLawType CaseLawType { get; set; }

        // public string CaseLawType { get; set; }


        public SearchMultiDict MultiDict { get; set; }

        public SearchCases(int langId)
            : base(langId)
        {
            DocumentTypes = new CheckTreeView(langId);
            CourtsFolders = new CheckTreeView(langId);
          //  DirectoryCaseLaw = new CheckTreeView(langId);
          //  EuroCases = new CheckTreeView(langId);
          //  Eurovoc = new CheckTreeView(langId);
            Syllabus = new CheckTreeView(langId);
         //   SubjectMatter = new CheckTreeView(langId);
          //  AdvocateGeneral = new CheckTreeView(langId);
          //  JudgeRapporteur = new CheckTreeView(langId);
         //   ProcedureType = new CheckTreeView(langId);
         //   States = new CheckTreeView(langId);
         //   HudocImportance = new CheckTreeView(langId);
         //   HudocArticleViolation = new CheckTreeView(langId);
         //   Hudoc = new CheckTreeView(langId);
         //   HudocApplicability = new CheckTreeView(langId);
         //   Courts = new CheckTreeView(langId);
           RulesOfTheCourt = new CheckTreeView(langId);
         //   HudocArticles = new CheckTreeView(langId);
            CaseLawType = CaseLawType.All;
        }

    }
}
