namespace NewInterlex.Core.Dto.UseCaseRequests
{
    using Enumerations;
    using Interfaces;
    using UseCaseResponses;

    public class UcExportReportRequestComposite : IUseCaseRequest<UcExportReportResponse>
    {
        public UcExportReportRequest[] Reports { get; set; }
        public HtmlExportTypes ExportType { get; set; }

    }
    public class UcExportReportRequest : IUseCaseRequest<UcExportReportResponse>
    {
        public ReportPair[] Pairs { get; set; }

        public Conclusion Conclusion { get; set; }

        public string CaseReportTranslation { get; set; }

        public string AboutCaseTranslation { get; set; }

        public string ConclusionTranslation { get; set; }

        public string LegalBasisTranslation { get; set; }
    }
    
    public class ReportPair
    {
        public ReportPair(int index, string question, string answer)
        {
            this.Index = index;
            this.Question = question;
            this.Answer = answer;
        }

        public int Index { get; }

        public string Question { get; }

        public string Answer { get; }
    }

    public class Conclusion
    {
        public Conclusion(string title, string reportDisplay, string legalBasis)
        {
            this.Title = title;
            this.ReportDisplay = reportDisplay;
            this.LegalBasis = legalBasis;
        }

        public string Title { get; }

        public string ReportDisplay { get; }

        public string LegalBasis { get; }
    }
}