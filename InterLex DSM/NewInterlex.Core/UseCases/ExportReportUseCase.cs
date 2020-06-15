namespace NewInterlex.Core.UseCases
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Dto.UseCaseRequests;
    using Dto.UseCaseResponses;
    using Enumerations;
    using Interfaces.Services;
    using Interfaces.UseCases;

    public class ExportReportUseCase : IExportReportUseCase
    {
        private readonly IHtmlConverter converter;

        public ExportReportUseCase(IHtmlConverter converter)
        {
            this.converter = converter;
        }

        public async Task<UcExportReportResponse> Handle(UcExportReportRequestComposite message)
        {
            var html = new StringBuilder();
            html.Append("<!DOCTYPE html lang=\"en\"><head><meta charset=\"utf-8\"><title>Interlex Decision Support</title>" +
                        $"<style>{InsertCss()}</style></head><body>");
            html.Append("<div class=\"report-dialog\">");
            html.Append("<div class=\"ui-dialog-titlebar\">");
            html.Append($"<span class=\"ui-dialog-title\">{message.Reports[0].CaseReportTranslation}</span>");
            html.Append("</div>");
            html.Append("<div class=\"ui-dialog-content\">"); // closed in the end
            
            foreach (var report in message.Reports)
            {
                //ABOUT
                html.Append("<div class=\"ui-card-body\">");
                html.Append($"<div class=\"ui-card-title\">{report.AboutCaseTranslation}</div>");
                html.Append("<div class=\"ui-card-content\">");
                foreach (var qaPair in report.Pairs)
                {
                    html.Append("<div>");
                    html.Append($"<p style=\"font-weight: bold;\">{qaPair.Index}. {qaPair.Question}</p>");
                    html.Append($"<div style=\"margin-left: 20px;\">- {qaPair.Answer}</div>");
                    html.Append("</div>");
                }
                html.Append("</div>");  //closes ui-card-content
                html.Append("</div>"); // closes ui-card-body
                //ABOUT
            
                //CONCLUSION
                html.Append("<div class=\"ui-card-body\">");
                html.Append($"<div class=\"ui-card-title\">{report.ConclusionTranslation}</div>");
                html.Append("<div class=\"ui-card-content\">");
                html.Append($"<p>{report.Conclusion.Title}</p><br><p>{report.Conclusion.ReportDisplay}</p>");
            
                html.Append("</div>");  //closes ui-card-content
                html.Append("</div>"); // closes ui-card-body
                //CONCLUSION
            
                // LEGAL BASIS
                html.Append("<div class=\"ui-card-body\">");
                html.Append($"<div class=\"ui-card-title\">{report.LegalBasisTranslation}</div>");
                html.Append("<div class=\"ui-card-content\">");
                html.Append($"<p>{report.Conclusion.LegalBasis}</p>");
                html.Append("</div>");  //closes ui-card-content
                html.Append("</div>"); // closes ui-card-body
                // LEGAL BASIS
            }
            
            html.Append("</div></div></body></html>");
            // File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "reportexport.html"), html.ToString());

            var result = await this.converter.ConvertHtml(html.ToString(), message.ExportType);
            return new UcExportReportResponse(result, true);
        }

        private string InsertCss()
        {
            return ".report-dialog {\n    box-sizing: border-box;\n    width: 700px} " +
                   ".ui-dialog-titlebar { margin-bottom: 10px;\n      color:#333333 !important;\n      padding: 5px 5px 5px 20px !important;\n      font-size: 18px;\n      text-align: center;\n      font-weight: 700;\n    }" +
                   "     .ui-dialog-content {\n        background-color: #ffffff;\n        color: #333333;\n        border: 0 none;\n        padding: 0 !important;\n}" +
                   " .ui-card-body {padding:0;\n        margin-bottom: 15px;}" +
                   " .ui-card-title {\n          background-color: #e6f3ff;\n        color: #4B6485;\n          border-radius: 0 !important;\n          text-align: center;\n          font-size: 1em;\n          padding: .25em;\n  font-weight: bold; margin-bottom: .5em;      }" +
                   " .ui-card-body p {line-height: 1.5;\n    margin: 0;}";
        }
    }
}