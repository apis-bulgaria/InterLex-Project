namespace Interlex.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web;
    using System.Text.RegularExpressions;
    using Interlex.BusinessLayer;
    using System.Net;
    using System.Configuration;

    [AllowAnonymous]
    public class BlogController : Controller
    {
        private string TARGET_URL = ConfigurationManager.AppSettings["BlogDocsUrl"];

        private const string SUCCESS_MESSAGE = "Documents successfuly marked as demo docs";
        private const string ERROR_MESSAGE_PARSE = "Unable to parse input";
        private const string ERROR_MESSAGE_DB = "Unable to update demo docs in database";

        private const string REGEX_PATTERN_IDS = @"(?<!\d)\d{7,8}(?!\d)";
        
        [HttpGet]
        public ActionResult AddDemoDocs()
        {
            var wc = new WebClient();
            var docLinks = wc.DownloadString(TARGET_URL);

            var linksSplitted = new List<string>();
            var docLangIds = new List<int>();
            string ip = Request.UserHostAddress;
            string basePath = HttpRuntime.AppDomainAppPath;

            // parse
            try
            {
                linksSplitted = docLinks.Split(';').ToList();
                foreach (var link in linksSplitted)
                {
                    if (link.Contains("LegalAct") || link.Contains("CourtAct"))
                    {
                        var regexObj = new Regex(REGEX_PATTERN_IDS);
                        var matches = regexObj.Matches(link);
                        docLangIds.Add(int.Parse(matches[0].Value));
                    }
                }
            }
            catch (Exception)
            {
                Logger.LogDemoDocs(basePath, ERROR_MESSAGE_PARSE, ip, new List<int>(), new List<string>());
                return Content(ERROR_MESSAGE_PARSE);
            }

            // db update
            int documentsUpdated = 0;
            try
            {
                documentsUpdated = Doc.AddDemoDocs(docLangIds.ToArray());
                Logger.LogDemoDocs(basePath, String.Empty, ip, docLangIds, new List<string>());
            }
            catch (Exception)
            {
                Logger.LogDemoDocs(basePath, ERROR_MESSAGE_DB, ip, new List<int>(), new List<string>());
                return Content(ERROR_MESSAGE_DB);
            }
            
            
            // TODO: Return doc identifiers and pass them here
            return Content(SUCCESS_MESSAGE);
        }
    }
}