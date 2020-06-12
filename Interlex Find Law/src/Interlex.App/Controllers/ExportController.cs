namespace Interlex.App.Controllers
{
    using Codaxy.WkHtmlToPdf;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Enums;
    using Interlex.App.Filters;
    using Interlex.BusinessLayer.Models;
    using System.Configuration;
    using Ionic.Zip;
    using System.Web.Configuration;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.IO;
    using Interlex.App.Helpers;
    using System.Security.Cryptography.X509Certificates;
    using System.Net.Security;
    using System.Net;

    public enum ExportType
    {
        PDF = 0,
        RTF = 1
    }

    [UserAuthorize]
    public class ExportController : BaseController
    {
        #region ControllerInvokedMethods

        public ActionResult ExportDocToPDF(string docId)
        {
            //if (UserData.Username.ToLower() == "sysdemo")
            //{
            //    if (ProductId == 1) // EuroCases
            //    {
            //        return RedirectToAction("ProductFeaturesInfo", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
            //    }
            //    else // Tax & Financial Standarts
            //    {
            //        return RedirectToAction("ProductFeaturesInfoFinances", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
            //    }
            //}


            // Console.InputEncoding = Encoding.UTF8;
            PdfConvert.Environment.Debug = true;

            var content = this.GetDocHTML(int.Parse(docId), true);

            var docNumber = Doc.GetDocNumberByDocLangId(int.Parse(docId));
            docNumber = docNumber.Replace('/', '_');

            this.Export(content, ExportType.PDF, docId);

            var FileToDownload = this.Download(this.GetExportDir() + "\\" + docId + ".pdf", docNumber + ".pdf", "application/pdf");

            return FileToDownload;
        }

        public ActionResult ExportDocToRTF(string docId)
        {
            //if (UserData.Username.ToLower() == "sysdemo")
            //{
            //    if (ProductId == 1) // EuroCases
            //    {
            //        return RedirectToAction("ProductFeaturesInfo", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
            //    }
            //    else // Tax & Financial Standarts
            //    {
            //        return RedirectToAction("ProductFeaturesInfoFinances", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
            //    }
            //}


            var content = this.GetDocHTML(int.Parse(docId), false);

            var docNumber = Doc.GetDocNumberByDocLangId(int.Parse(docId));
            docNumber = docNumber.Replace('/', '_');

            this.Export(content, ExportType.RTF, docNumber);

            var FileToDownload = this.Download(this.GetExportDir() + "\\" + docNumber + ".rtf", docNumber + ".rtf", "text/rtf");

            return FileToDownload;
        }

        //ids imported splitted with "-", ex: "id1-id2-id3" and so on
        public ActionResult ExportMultiDocs(string idsString, string type)
        {
            //if (UserData.Username.ToLower() == "sysdemo")
            //{
            //    if (ProductId == 1) // EuroCases
            //    {
            //        return RedirectToAction("ProductFeaturesInfo", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
            //    }
            //    else // Tax & Financial Standarts
            //    {
            //        return RedirectToAction("ProductFeaturesInfoFinances", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
            //    }
            //}


            var ids = idsString.Split('-');

            if (ids.Length == 0)
            {
                return new EmptyResult();
            }
            else if (ids.Length == 1)
            {
                if (type == "pdf")
                {
                    return this.ExportDocToPDF(ids[0]);
                }
                else //RTF
                {
                    return this.ExportDocToRTF(ids[0]);
                }
            }
            else
            {
                var now = DateTime.Now;
                var year = now.Year.ToString();
                var month = now.Month.ToString();
                if (month.ToString().Length == 1)
                {
                    month = "0" + month;
                }
                var day = now.Day.ToString();
                if (day.ToString().Length == 1)
                {
                    day = "0" + day;
                }

                string totalDocsCount = ids.Length.ToString();
                if (totalDocsCount.Length == 1)
                {
                    totalDocsCount = "0" + totalDocsCount;
                }

                var zipLabel = "EuroCases_" + totalDocsCount + "_docs_" + year + month + day;

                //populating
                using (ZipFile zip = new ZipFile())
                {
                    foreach (var id in ids)
                    {
                        var content = String.Empty;

                        if (type == "pdf")
                        {
                            content = this.GetDocHTML(int.Parse(id), true);
                        }
                        else
                        {
                            content = this.GetDocHTML(int.Parse(id), false);
                        }


                        var docNumber = Doc.GetDocNumberByDocLangId(int.Parse(id));
                        docNumber = docNumber.Replace('/', '_');

                        if (type == "pdf")
                        {
                            this.Export(content, ExportType.PDF, docNumber);
                        }
                        else //RTF
                        {
                            this.Export(content, ExportType.RTF, docNumber);
                        }

                        zip.AddFile(this.GetExportDir() + "\\" + docNumber + "." + type, zipLabel);
                    }

                    zip.Save(this.GetExportDir() + "\\" + zipLabel + ".zip");
                }

                foreach (var id in ids)
                {
                    var docNumber = Doc.GetDocNumberByDocLangId(int.Parse(id));
                    docNumber = docNumber.Replace('/', '_');

                    System.IO.File.Delete(this.GetExportDir() + "\\" + docNumber + "." + type);
                }

                return this.Download(this.GetExportDir() + "\\" + zipLabel + ".zip", zipLabel + ".zip", "application/zip");
            }
        }
        #endregion

        #region ExportAndDownloadHelpers
        private void Export(string content, ExportType type, string fileName)
        {
            var exportDirectory = this.GetExportDir();

            var htmlPath = this.SaveTempHtmlFile(content, exportDirectory, fileName);

            string contentType = String.Empty;

            if (type == ExportType.PDF)
            {
                this.ExportFileToPDF(htmlPath, exportDirectory, fileName);
                contentType = "application/pdf";
            }
            else if (type == ExportType.RTF)
            {
                //specify to use TLS 1.2 as default connection
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(ConfigurationManager.AppSettings["ExportServiceBasePath"])
                };

                var requestContent = new MultipartFormDataContent
            {
                { new StringContent(content), "html" },
                { new StringContent(ConfigurationManager.AppSettings["ExportServiceUsername"]), "username" },
                { new StringContent(ConfigurationManager.AppSettings["ExportServicePassword"]), "password" }
            };

                var result = httpClient.PostAsync("/api/Export/HtmlToRtf", requestContent).Result;
                var contentAsString = result.Content.ReadAsStringAsync().Result;
                var binaryContent = Convert.FromBase64String(contentAsString.Trim('"'));
                System.IO.File.WriteAllBytes(Path.Combine(exportDirectory, fileName + ".rtf"), binaryContent);

            }
            else
            {
                throw new ArgumentException("Not valid export type");
            }

            System.IO.File.Delete(htmlPath);
        }

        private FileResult Download(string path, string nameWhenDownloaded, string contentType)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);
            return File(fileBytes, contentType, nameWhenDownloaded);
        }
#endregion

#region GettingHTML
        private string GetDocHTML(int docId, bool isPdf)
        {
            CourtAct courtAct = CourtActBL.GetCourtAct(
                docId,
                this.Language.Id,
                UserData.UserId,
                new DocHighlightSearchParams(null, null, false, null, null),
                ProductId,
                this.UserData.ShowFreeDocuments);

            var content = GetViewString("~/Views/Doc/Act.cshtml", courtAct, true);

            StringBuilder html = new StringBuilder();
            html.Append("<html><head><meta charset=\"UTF-8\">");

            if (isPdf == true)
            {
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/doc.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/eurocases.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/layout.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/doc-list.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/doc-top-bar.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/search.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/spritecrops.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/xslt/styleActGeneral.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/xslt/styleActEuroLexJudgment.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/xslt/styleGeneral.css\">");
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/xslt/styleJudgmentGeneral.css\">");
            }
            else
            {
                html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Content/Styles/export-rtf.css\">");
            }

            html.Append("</head><body>");
            content = content.Replace("section", "div");
            content = content.Replace("header", "div");
            html.Append(content);
            html.Append("</body></html>");

            html = html.Replace("/DocImages", ConfigurationManager.AppSettings["BaseDocImagesPath"]);

            return html.ToString();
        }

#endregion

#region Utility
        private string GetExportDir()
        {
            var sessionId = HttpContext.Session.SessionID;
            var exportDirectory = HttpRuntime.AppDomainAppPath;
            exportDirectory = exportDirectory + "Session_Files\\" + sessionId;

            System.IO.Directory.CreateDirectory(exportDirectory);

            return exportDirectory;
        }

        private string SaveTempHtmlFile(string content, string exportDir, string fileName)
        {
            var pathToSave = exportDir + "\\" + fileName + ".html";

            System.IO.File.WriteAllText(exportDir + "\\" + fileName + ".html", content);

            return pathToSave;
        }

        private void ExportFileToPDF(string htmlUrl, string exportDirectory, string fileName)
        {
            exportDirectory = exportDirectory + "\\";

            PdfConvert.ConvertHtmlToPdf(new PdfDocument { Url = htmlUrl }, new PdfOutput
            {
                OutputFilePath = exportDirectory + fileName + ".pdf",
            });
        }

#endregion
    }
}
