namespace NewInterlex.Api.Models.Request
{
    public class ReportRequest
    {
        public ReportPairRequest[] Pairs { get; set; }

        public ConclusionRequest Conclusion { get; set; }
        
        public string CaseReportTranslation { get; set; }

        public string AboutCaseTranslation { get; set; }

        public string ConclusionTranslation { get; set; }

        public string LegalBasisTranslation { get; set; }
    }

    public class ReportPairRequest
    {
        public int Index { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }

    public class ConclusionRequest
    {
        public string Title { get; set; }

        public string ReportDisplay { get; set; }

        public string LegalBasis { get; set; }
    }
    
}