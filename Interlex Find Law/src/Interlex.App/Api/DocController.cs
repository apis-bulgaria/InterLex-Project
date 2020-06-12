using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Enums;
using Interlex.BusinessLayer.Entities;
using Interlex.App.Resources;
using Newtonsoft.Json;
using System.Web.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Resources;
using AkomaNtosoXml.Xslt.Core.Classes.Resolver;
using Apis.Common.Extensions;
using Interlex.App.Api.Classes;
using Interlex.App.Api.Models;
using HtmlAgilityPack;
using Apis.Common.Html.Extension;
using Apis.Common.Classes.Common;
using Apis.Common.Classes.ReferencesBuilder;
using System.Web.Http.Cors;

namespace Interlex.App.Api
{
    public class CiteData
    {
        /// <summary>
        /// 1 - caselaw
        /// 2 - legislation
        /// </summary>
        public int DocType { get; set; }

        public string Text { get; set; }

    }

    public enum CiteType
    {
        ShortCite = 1,
        LongCite = 2
    }

    [AllowAnonymous]
    [EnableCors("*", "*", "*")]
    public class DocController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Cite(int langId, string docNumber, CiteType citeType, String domain = "")
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK);

            ChangeLangCulture(langId);

            var data = new CiteData
            {
                DocType = 1,
                Text = Resources.Resources.ApiDocCiteNotFound
            };

            if (domain.IsIn(StringComparison.InvariantCultureIgnoreCase, "bgcite"))
            {
                var parts = docNumber.Split('_');

                data.Text = WebApisRequest.Cite(code: parts[1], @base: parts[0], citeType: citeType);
            }
            else
            {
                Document doc = Doc.GetDocByDocNumber(docNumber, langId, null);
                if (doc != null)
                {
                    string docXml = Doc.GetDocText(doc.DocLangId, doc.DocType, 1, true); // this is temporary
                    if (citeType == CiteType.LongCite)
                        data.Text = AkomaNtosoPreProcessor.GetLongCitation(docXml);
                    else
                        data.Text = AkomaNtosoPreProcessor.GetShortCitation(docXml);
                }
            }

            response.Headers.Remove("Access-Control-Allow-Origin");
            response.Headers.Remove("Access-Control-Allow-Methods");
            response.Headers.Remove("Access-Control-Allow-Headers");

            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET");
            response.Headers.Add("Access-Control-Allow-Headers", "*");

            response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), Encoding.UTF8, "text/html");
            return response;
        }

        [HttpGet]
        public HttpResponseMessage ParHint(int langId, int siteLangId, string docNumber, string toPar = "", string domain = "")
        {
            var baseUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("/api"));

            var response = this.Request.CreateResponse(HttpStatusCode.OK);

            var content = String.Empty;

            if (domain.IsIn(StringComparison.InvariantCultureIgnoreCase, "bgtooltip"))
            {
                ChangeLangCulture(langId);

                var parts = docNumber.Split('_');

                content = WebApisRequest.Hint(code: parts[1], @base: parts[0], toPar: toPar);
                if (String.IsNullOrEmpty(content))
                {
                    content = WebApisDocumentLink.CreateApisUrl(code: parts[1], @base: parts[0]);
                }
            }
            else
            {
                var actualLanguageId = GetAvailableLanguageIdForDocument(docNumber, langId);

                ChangeLangCulture(actualLanguageId);

                var d = Doc.GetParText(
                    "celex",
                    docNumber,
                    toPar,
                    actualLanguageId,
                    null,
                    actualLanguageId,
                    false,
                    false,
                    true,
                    true);

                content = d.Text;
                content = RedirectCelexAnchorsToEurocases(content, baseUrl, actualLanguageId);
                if (!String.IsNullOrEmpty(content))
                {
                    content = $"<div class='eurocases-hint-wrapper'><p class='hint-header'>InterLex</p>{content}</div>";
                }

                if (String.IsNullOrEmpty(content))
                {
                    content = Doc.CreateEurLexLink(docNumber, actualLanguageId);
                }
            }

            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET");
            response.Headers.Add("Access-Control-Allow-Headers", "*");

            response.Content = new StringContent(content, Encoding.UTF8, "text/html");

            return response;
        }

        [HttpGet]
        public HttpResponseMessage ParHint(int langId, int siteLangId, int uniqueId, int dbIndex, String toPar = "")
        {
            var content = WebApisRequest.Hint(uniqueId: uniqueId, dbIndex: dbIndex, toPar: toPar);

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET");
            response.Headers.Add("Access-Control-Allow-Headers", "*");

            response.Content = new StringContent(content, Encoding.UTF8, "text/html");

            return response;
        }

        [HttpGet]
        public HttpResponseMessage DocInLinks(string domain, int langId, int siteLangId, int limit, string docNumber, string toPar)
        {
            // ChangeLangCulture(siteLangId);

            var baseUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("/api"));
            var response = this.Request.CreateResponse(HttpStatusCode.OK);

            var faultedOrEmpty = false;
            var documentTitleAsAnchor = docNumber;
            var documentTitle = docNumber;
            var documentLink = String.Empty;
            LinksToHtmlModel apisHtmlModel = null;
            LinksToHtmlModel eurocasesHtmlModel = null;
            try
            {
                if (domain.IsIn(StringComparison.InvariantCultureIgnoreCase, "bgl", "bgcl", "bgall"))
                {
                    ChangeLangCulture(langId);

                    apisHtmlModel = CreateApisNationalHtmlModel(docNumber, toPar, domain, limit, out documentTitle, out documentLink);
                }
                else
                {
                    var actualLanguageId = GetAvailableLanguageIdForDocument(docNumber: docNumber, langId: langId);
                    ChangeLangCulture(actualLanguageId);

                    eurocasesHtmlModel = CreateEuroCasesHtmlModel(docNumber, toPar, domain, baseUrl, limit, actualLanguageId, siteLangId, out documentTitle, out documentLink);

                    if (langId == 1) // only when the language is bulgarian
                        apisHtmlModel = CreateApisEuHtmlMode(docNumber, toPar, domain, limit);
                }

                documentTitleAsAnchor = $"<a target='blank' class='AnchorDocTitle' href='{documentLink}'>{documentTitle}</a>";
                if ((eurocasesHtmlModel?.Links?.Any() == true) || (apisHtmlModel?.Links?.Any() == true))
                {
                    // var title = $"{ResolveTitlePrefixForDocInLinks(domain)}<br/>{documentTitleAsAnchor}";
                    var title = $"{ResolveTitlePrefixForDocInLinks(domain)}";

                    var linksToHtmls = new List<LinksToHtmlModel>();

                    if (eurocasesHtmlModel?.Links?.Any() == true)
                    {
                        linksToHtmls.Add(eurocasesHtmlModel);
                    }

                    if (apisHtmlModel != null && apisHtmlModel.Links.Any())
                    {
                        linksToHtmls.Add(apisHtmlModel);
                    }

                    response.Content = new StringContent(DocLinks2Html(linksToHtmls, title, documentTitleAsAnchor, limit, baseUrl, true), Encoding.UTF8, "text/html");
                }
                else
                {
                    faultedOrEmpty = true;
                }

            }
            catch (Exception e)
            {
                faultedOrEmpty = true;
            }

            if (faultedOrEmpty)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><title>" + Resources.Resources.UI_NoAvailableInfo + " " + documentTitleAsAnchor + "</title>");
                sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                sb.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + baseUrl + "/Content/Styles/docapi.css\"></link>");
                sb.Append("</head><body>");
                sb.Append("<div id=\"DivTopTop\"></div>");
                sb.Append("<div id=\"DivTopGray\"><img src=\"" + baseUrl + "/Content/Images/EUCasesLogo.jpg\"  alt=\"\"/></div>");

                sb.Append("<div id=\"DivHeader\"><p>" + Resources.Resources.UI_NoAvailableInfo + " <br/>" + documentTitleAsAnchor + "</p></div>");
                sb.Append("</body></html>");
                response.Content = new StringContent(sb.ToString(), Encoding.UTF8, "text/html");
            }
            return response;
        }

        [HttpPost]
        public HttpResponseMessage SearchByTerm([FromBody] string searchText, string domain, int langId, int siteLangId)
        {
            string classifierId = Classifiers.GetClassifierIdByTitle(searchText);

            return SearchByClassifierId(classifierId, domain, langId, siteLangId, -1);
        }

        [HttpGet]
        public HttpResponseMessage SearchByXmlId(string xmlId, string domain, int langId, int siteLangId, int limit = 25)
        {
            try
            {
                // tmp fix for using terms with letters in the term service
                xmlId = new string(xmlId.Where(Char.IsDigit).ToArray());
            }
            catch
            {
            }

            var classifierId = Classifiers.GetClassifierIdByXmlId(xmlId);

            return SearchByClassifierId(classifierId, domain, langId, siteLangId, limit);
        }

        private HttpResponseMessage SearchByClassifierId(string classifierId, string domain, int langId, int siteLangId, int limitPerSource)
        {
            ChangeLangCulture(langId);

            List<int> l = new List<int>();

            string baseUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("/api"));

            ClassificationService.ClassificatorTreeModel ci = null;
            if (!String.IsNullOrEmpty(classifierId))
                ci = ClassifiersProvider.Service.GetTreeByGuid(classifierId);

            StringBuilder result = new StringBuilder();

            if (ci == null) // Eurovoc term is not found
            {
                string title = Resources.Resources.UI_SearchByTerm;
                result.Append("<html><head><title>" + title.Replace("<br/>", " ") + "</title>");
                result.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                result.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + baseUrl + "/Content/Styles/docapi.css\"></link>");
                result.Append("</head><body>");
                result.Append("<div id=\"DivTopTop\"></div>");
                result.Append("<div id=\"DivTopGray\"><img src=\"" + baseUrl + "/Content/Images/EUCasesLogo.jpg\"  alt=\"\"/></div>");
                result.Append("<div id=\"DivHeader\"><p>" + title + "<hr/>" + Resources.Resources.UI_EurovocTermNotFound + "</p></div>");
                result.Append("</body></html>");
            }
            else
            {
                string term = ci.LanguageVariantsWithHints[langId.ToString()].Title;
                string title = Resources.Resources.UI_SearchByTerm + " \"" + term + "\"<br/>";

                string searchQuery = "classificators:(" + classifierId.Replace("-", "") + ")";
                switch (domain.ToLower())
                {
                    case "natcl":
                        title += " " + Resources.Resources.UI_inNatCL;
                        searchQuery += " props:(dtjudgment) props:(noteudoc)";
                        break;
                    case "natl":
                        title += " " + Resources.Resources.UI_inNatL;
                        searchQuery += " props:(dtact) props:(noteudoc)";
                        break;
                    case "eucl":
                        title += " " + Resources.Resources.UI_inEUCL;
                        searchQuery += " props:(dtjudgment) props:(eudoc)";
                        break;
                    case "eul":
                        title += " " + Resources.Resources.UI_inEUL;
                        searchQuery += " props:(dtact) props:(eudoc)";
                        break;
                }

                int[] wsRes = null;
                //SearchBox sb = new SearchBox(langId);
                //sb.SearchText = term;
                int[] langPref = new int[] { langId, 4 };
                string searchWrapperUrl = null;

                if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
                }
                else
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath_cc"];
                }

                SearchResult sr = new SearchResult(SearchSources.Search, HttpContext.Current.Application["SearchWrapper"],
                        searchWrapperUrl,
                        HttpContext.Current.Application["FilterDocsStruct"],
                        HttpContext.Current.Application["FilterDocsClassifiers"],
                        HttpContext.Current.Application["ClassifiersMap"],
                        HttpContext.Current.Application["ResultsGroupper"],
                        HttpRuntime.AppDomainAppPath, langId, 0, 0
                      );
                //sr.SearchBoxFilters = sb;
                sr.SearchFTQuery(searchQuery, ref wsRes, langPref);

                // wsRes = wsRes.Take(limitPerSource).ToArray();

                var linksToHtmlModels = new List<LinksToHtmlModel>();
                if (!String.IsNullOrEmpty(domain)
                    && langId == 1 // for documents which are in bulgarian
                    && domain.IsIn(StringComparison.InvariantCultureIgnoreCase, "natl", "natcl", "all"))
                {
                    // only if the document is in bulgarian language

                    var bgTitle = String.Empty;
                    if (ci.LanguageVariantsWithHints.ContainsKey("1"))
                        bgTitle = ci.LanguageVariantsWithHints["1"]?.Title;

                    if (!string.IsNullOrEmpty(bgTitle))
                    {
                        var webApisDomain = WebApisHelper.MapNormalDomainToApisDomain(domain);
                        try
                        {
                            var apisSearchResponse = WebApisRequest.SearchByText(domain: webApisDomain, text: bgTitle, limit: limitPerSource);
                            if (apisSearchResponse != null)
                            {
                                // var apisSearchResponse = WebApisRequest.TestData();
                                var htmlModel = LinksToHtmlModel.Create(apisSearchResponse.Links, apisSearchResponse.Full_Count, limitPerSource, apisSearchResponse.All_Url, "Apis Web");
                                linksToHtmlModels.Add(htmlModel);
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                var docList = EurocasesDocumentLink.FromDocLinks(Doc.GetDocListByIds(wsRes.Take(limitPerSource).ToArray(), domain), baseUrl);
                if (docList.Any())
                {
                    var allUrl = EuroCasesHelper.BuildAllUrlForSearchByXmlId(baseUrl: baseUrl, xmlId: ci.XmlId, domain: domain, langId: langId, uiLangId: siteLangId);
                    var linsksToHtmlMode = LinksToHtmlModel.Create(docList, wsRes.Length, limitPerSource, allUrl, "EuroCases");
                    linksToHtmlModels.Insert(0, linsksToHtmlMode);
                }

                result.Append(DocLinks2Html(linksToHtmlModels, title, String.Empty, limitPerSource, baseUrl, true));
            }

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(result.ToString(), Encoding.UTF8, "text/html");

            return response;
        }

        private string DocLinks2Html(IReadOnlyCollection<LinksToHtmlModel> linksToHtmls, string title, String subTitle, int limit, string baseUrl, bool addHtmlHeader)
        {
            var sb = new StringBuilder();

            if (addHtmlHeader)
            {
                var tagFreeTitle = ApisStringHelper.StripTags(title.Replace("<br/>", " "));

                sb.Append("<html><head><title>" + tagFreeTitle + "</title>");
                sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                sb.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + baseUrl + "/Content/Styles/docapi.css\"></link>");
                sb.Append("</head><body>");
            }

            var eurocasesLogoUrl = "http://eurocases.eu";
            var apisLogoUrl = "http://apis.bg";

            var headerContent = $@"
                <table>
                    <tr>
                        <td width='20%'>
                            <div id='DivLogo'><a target='_blank' href='{eurocasesLogoUrl}'><img src='{baseUrl + "/Content/Images/logo.svg'"} /></a></div>
                            <div id='DivLogoApis'><a target='_blank' href='{apisLogoUrl}'><img src='{baseUrl + "/Content/Images/logo_apis.jpg'"} /></a></div>
                        </td>
                    </tr>
                    <tr><td></td></tr>
                    <tr><td><div id='DivHeader'><p>{title}</p></div></td></tr>
                    <tr><td><div id='DivHeader'><p>{subTitle}</p></div></td></tr>
                </table>
               ";
            sb.Append($"<div id='DivTopTop'></div>");
            sb.Append(headerContent);

            //sb.Append("<div id=\"DivTopTop\"></div>");
            //sb.Append("<div id=\"DivLogo\"><img src=\"" + baseUrl + "/Content/Images/logo.svg\"  alt=\"\"/></div>");
            //sb.Append("<div> </div>");

            // sb.Append("<div id=\"DivHeader\"><p>" + title + "</p></div>");
            sb.Append("<div id=\"Content\">");

            var hasTwoTables = linksToHtmls.HasAtleastItems(2);

            foreach (var linksToHtml in linksToHtmls)
            {
                var links = linksToHtml.Links;
                var totalCount = linksToHtml.TotalCount;
                var allLinksUrl = linksToHtml.AllLinksUrl;
                // var limit = linksToHtml.Limit;
                var sourceName = linksToHtml.SourceName;


                var classToAppend = "DivSingleContent";
                if (hasTwoTables)
                    classToAppend = "DivMultiContent";

                sb.Append($"<div class='DivLinksTableWrapper {classToAppend}'>");
                if (!String.IsNullOrEmpty(sourceName))
                {
                    sb.Append($"<div class='DivSource'>{sourceName}</div>");
                }

                var countContent = totalCount.ToString() + " " + Resources.Resources.UI_DocumentsFound;
                if (!String.IsNullOrEmpty(allLinksUrl))
                {
                    countContent = $"{countContent}<a class='AnchorAllLinks' target='_blank' href='{allLinksUrl}'> ({Resources.Resources.UI_All}) </a>";
                }

                sb.Append($"<div class=\"DivNumber\"><p>{countContent}</p></div>");


                sb.Append("<table cellpadding='10'>");

                sb.Append("<tr><td class=\"TdHeader\"></td><td class=\"TdHeader\">" + Resources.Resources.UI_Title + "</td>");

                sb.Append("</tr>");

                int rowNumber = 0;
                foreach (DocumentLink l in links)
                {
                    rowNumber++;

                    var imageName = String.Empty;
                    if (l.IsCase())
                    {
                        imageName = "icon-cases-big.png";
                    }
                    else if (l.IsLegislation())
                    {
                        imageName = "icon-law-big.png";
                    }
                    else
                    {
                        imageName = "icon-law-other-big.png";
                    }

                    sb.Append("<tr>");
                    sb.Append("<td class=\"TdImage\"><img src=\"" + baseUrl + "/Content/Images/" + imageName + "\" alt=\"\"/></td>");
                    sb.Append("<td><a href=\"" + l.GetUrl() + "\" target=\"_blank\">" + l.Title + "</a></td>");
                }
                sb.Append("</table>");
                sb.Append("</div>");


            }
            sb.Append("</div>");

            sb.Append("<div class=\"DivNumber\"><p>" + Resources.Resources.UI_ListFirstDocsDesc.Replace("{limit}", limit.ToString()) + "</p></div>");

            if (addHtmlHeader)
                sb.Append("</body></html>");

            return sb.ToString();
        }

        private int MapLangId(int inLangId)
        {
            int siteLangId = 4; // en default
            switch (inLangId)
            {
                case 7:
                    siteLangId = 2;
                    break;
                case 9:
                    siteLangId = 4;
                    break;
                case 12:
                    siteLangId = 3;
                    break;
                case 16:
                    siteLangId = 5;
                    break;
                case 2:
                    siteLangId = 1;
                    break;
                case 1026:
                    siteLangId = 1;
                    break;
            }
            return siteLangId;
        }

        private void ChangeLangCulture(int langId)
        {
            string code = null;
            Language siteLang = Languages.GetLang(langId);
            if (siteLang != null && siteLang.IsInterfaceLang)
                code = siteLang.Code;

            if (!string.IsNullOrEmpty(code))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}", code));
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(string.Format("{0}", code));
            }
        }

        private static int GetAvailableLanguageIdForDocument(String docNumber, int langId)
        {
            var tmp = Doc.GetDocByDocNumber(docNumber, langId, null);

            if (tmp != null && tmp.LangId != 0)
                return tmp.LangId;
            else
                return langId;
        }

        private static String ResolveTitlePrefixForDocInLinks(String domain)
        {
            var result = Resources.Resources.ApiDocRefs;

            var map = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase)
            {
                ["eucl"] = Resources.Resources.ApiDocRefsFromEuCaseLaw,
                ["eul"] = Resources.Resources.ApiDocRefsFromEuLegislation,
                ["natcl"] = Resources.Resources.ApiDocRefsFromNatCaseLaw,
                ["natl"] = Resources.Resources.ApiDocRefsFromNatLegislation,
                ["bgcl"] = Resources.Resources.ApiDocRefsFromApisDbCaseLaw,
                ["bgl"] = Resources.Resources.ApiDocRefsFromApisDbLegislation,
                ["all"] = Resources.Resources.ApiDocRefs,
                ["bgall"] = Resources.Resources.ApiDocRefsFromApisDb
            };

            if (map.ContainsKey(domain))
                result = map[domain];

            return result;
        }

        private static String RedirectCelexAnchorsToEurocases(String html, String baseUrl, int langId)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var anchors = htmlDocument.DocumentNode.SafeSelectNodes(".//a[@href]");

            foreach (var anchor in anchors)
            {
                anchor.SetAttributeValue("target", "_blank");

                // is celex url
                if (anchor.Attributes["href"].Value.StartsWithAny(StringComparison.OrdinalIgnoreCase, "./celex", "celex"))
                {
                    var celexAndToPar =
                        anchor.Attributes["href"].Value.SafeReplaceAtStart(oldValue: "./celex=", newValue: string.Empty, comparison: StringComparison.OrdinalIgnoreCase)
                        .Split("&amp;ToPar=");

                    var path = ApisPathHelper.Combine(baseUrl, "Doc", "Act", "LastCons", langId.ToString(), celexAndToPar[0]);
                    if (celexAndToPar.Length == 2)
                        path = ApisPathHelper.Combine(path, celexAndToPar[1]);

                    path = path.Replace("\\", "/");

                    anchor.Attributes["href"].Value = path;
                }
            }

            return htmlDocument.DocumentNode.OuterHtml;
        }

        private static String AppendToPartTitleFromCelexAndToPar(String titleOfDocument, String docNumber, String toPar, int langId, String toParDefault)
        {
            if (String.IsNullOrEmpty(toPar))
                return titleOfDocument;

            var toParText = toParDefault;

            try
            {
                var reference = ReferenceBuilder.Parse(docNumber, toPar);
                if (langId == 1)
                {
                    toParText = reference.GetDisplayTextInBulgarian(ReferenceDisplayOption.DisplayText, ReferenceFormOption.Long).SafeReplaceAtStart(",", String.Empty).Trim();
                }
                else if (langId == 2)
                {
                    toParText = reference.GetDisplayTextInGerman(ReferenceDisplayOption.DisplayText, ReferenceFormOption.Long).SafeReplaceAtStart(",", String.Empty).Trim();
                }
                else if (langId == 3)
                {
                    toParText = reference.GetDisplayTextInFrench(ReferenceDisplayOption.DisplayText, ReferenceFormOption.Long).SafeReplaceAtStart(",", String.Empty).Trim();
                }
                else if (langId == 4)
                {
                    toParText = reference.GetDisplayTextInEnglish(ReferenceDisplayOption.DisplayText, ReferenceFormOption.Long).SafeReplaceAtStart(",", String.Empty).Trim();
                }
                else if (langId == 5)
                {
                    toParText = reference.GetDisplayTextInItalian(ReferenceDisplayOption.DisplayText, ReferenceFormOption.Long).SafeReplaceAtStart(",", String.Empty).Trim();
                }
            }
            catch
            {

            }

            return $"{titleOfDocument} ({toParText})";
        }

        private static LinksToHtmlModel CreateApisNationalHtmlModel(String docNumber, String toPar, String domain, int limit, out String documentTitle, out String documentLink)
        {
            documentTitle = String.Empty;
            documentLink = String.Empty;

            // request web.apis.bg for links related to bg legislation or case law

            LinksToHtmlModel result = null;
            var parts = docNumber.Split('_');
            if (parts.Length == 2) // only when the base and the code are provider (by convention they are splited with '_')
            {
                var apisResponse = WebApisRequest.GetDocLinks(domain: domain, limit: limit, code: parts[1], @base: parts[0], toPar: toPar);
                // var apisResponse = WebApisRequest.TestData();

                if (apisResponse != null)
                {
                    documentTitle = apisResponse.Caption;
                    if (!String.IsNullOrEmpty(toPar))
                    {
                        documentTitle = AppendToPartTitleFromCelexAndToPar(documentTitle, "fake number", toPar, 1, String.Empty);
                    }

                    documentLink = WebApisDocumentLink.CreateApisUrl(code: parts[1], @base: parts[0]);

                    result = LinksToHtmlModel.Create(
                      links: apisResponse.Links,
                      totalCount: apisResponse.Full_Count,
                      limit: limit,
                      allLinksUrl: apisResponse.All_Url,
                      sourceName: "Apis Web");
                }
            }

            return result;
        }

        private static LinksToHtmlModel CreateApisEuHtmlMode(String docNumber, String toPar, String domain, int limit)
        {
            LinksToHtmlModel result = null;

            var apisDomain = WebApisHelper.MapNormalDomainToApisForEuAct(domain);
            var apisResponse = WebApisRequest.GetEuDocLinks(domain: apisDomain, limit: limit, celex: docNumber, toPar: toPar);
            if (apisResponse != null)
            {
                result = LinksToHtmlModel.Create(
                    links: apisResponse.Links,
                    totalCount: apisResponse.Full_Count,
                    limit: limit,
                    allLinksUrl: apisResponse.All_Url,
                    sourceName: "Apis Web");
            }

            return result;
        }

        private static LinksToHtmlModel CreateEuroCasesHtmlModel(String docNumber, String toPar, String domain, String baseUrl, int limit, int langId, int siteLangId, out String documentTitle, out String documentLink)
        {
            documentTitle = String.Empty;
            documentLink = String.Empty;

            LinksToHtmlModel result = null;
            var linkInfo = Doc.GetLinkInfo(docNumber, toPar, null, langId);
            if (linkInfo != null)
            {
                int docLangId = Convert.ToInt32(linkInfo["doc_lang_id"]);
                var docType = Convert.ToInt32(linkInfo["doc_type"]);
                documentTitle = linkInfo["title"];
                int? parId = null;
                bool isParsed = int.TryParse(linkInfo["doc_par_id"], out int currentParId);
                if (isParsed)
                {
                    parId = new Nullable<int>(currentParId);
                }
                if (!string.IsNullOrEmpty(linkInfo["par_title"]))
                {
                    documentTitle = AppendToPartTitleFromCelexAndToPar(documentTitle, docNumber, toPar, langId, linkInfo["par_title"]);
                }

                documentLink = new EurocasesDocumentLink(title: documentTitle,
                   originalUrl: String.Empty,
                   publisher: String.Empty,
                   baseUrl: baseUrl,
                   docLangId: docLangId,
                   documentType: docType).GetUrl();

                var eurocasesDocLinks = EurocasesDocumentLink.FromDocLinks(Doc.GetDocInLinks(docLangId, parId, domain, null, langId, limit, out int totalCount), baseUrl);

                var allLinksUrl = EuroCasesHelper.BuildAllUrlForDocInLinks(baseUrl: baseUrl, langId: langId, uiLangId: siteLangId, domain: domain, docNumber: docNumber, toPar: toPar);

                result = LinksToHtmlModel.Create(eurocasesDocLinks, totalCount, limit, allLinksUrl, "EuroCases");
            }
            else
            {
                linkInfo = Doc.GetLinkInfo(docNumber, null, null, langId);
                if (linkInfo != null)
                {
                    documentTitle = linkInfo["title"];
                    if (!string.IsNullOrEmpty(toPar))
                    {
                        documentTitle = AppendToPartTitleFromCelexAndToPar(documentTitle, docNumber, toPar, langId, String.Empty);
                    }

                    int docLangId = Convert.ToInt32(linkInfo["doc_lang_id"]);
                    var docType = Convert.ToInt32(linkInfo["doc_type"]);

                    documentLink = new EurocasesDocumentLink(title: documentTitle,
                        originalUrl: String.Empty,
                        publisher: String.Empty,
                        baseUrl: baseUrl,
                        docLangId: docLangId,
                        documentType: docType).GetUrl();
                }
            }

            return result;
        }
    }
}
