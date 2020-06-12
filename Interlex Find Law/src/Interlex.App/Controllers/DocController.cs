namespace Interlex.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Enums;
    using Interlex.BusinessLayer.Models;
    using Interlex.App.Filters;
    using Interlex.App.Helpers;
    using ViewModels;

    [UserAuthorize]
    public class DocController : BaseController
    {
        public ActionResult TestDocText()
        {
            Doc.TestDocText();

            return new EmptyResult();
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            string str = filterContext.Result.ToString();


        }

        public ActionResult Act(int docPrefLangId, string docNumber, string toPar, bool lastCons)
        {
            docNumber = docNumber.Replace("_", "/");

            // Временно всички hudoc сочат към външния адрес!
            if (docNumber.IndexOf("HUDOC-") > -1 && docNumber != "HUDOC-39954/08")
            {
                if (!this.Response.IsRequestBeingRedirected)
                {
                    this.Response.Redirect("http://hudoc.echr.coe.int/eng#{\"appno\":[\"" + docNumber.Replace("HUDOC-", "") + "\"]}", true);
                }
            }

            Document doc = Doc.GetDocByDocNumber(docNumber, docPrefLangId, this.UserData.UserId);
            string eid = null;
            string num = null;
            if (!String.IsNullOrEmpty(toPar) && doc != null)
            {
                Dictionary<string, string> d = Doc.GetEIDfromToPar(doc.DocLangId, toPar);
                if (d != null)
                {
                    eid = d["eid"];
                    num = d["num"];
                }
            }

            if (lastCons)
            {
                int? docLangId = Doc.GetLastConsDocLangId(docNumber, docPrefLangId, this.Language.Id, this.UserData.UserId, true);
                if (docLangId.HasValue)
                    doc = Doc.GetDocument(docLangId.Value);
                if (!String.IsNullOrEmpty(eid))
                    eid = Doc.CheckEID(doc.DocLangId, eid, num);
            }


            if (doc != null)
            {
                string url = WebAppHelper.AppRootFolder + "/Doc/" + ((doc.DocType == 1) ? "CourtAct" : "LegalAct") + "/" + doc.DocLangId;

                if (!String.IsNullOrEmpty(eid))
                    url += "#" + eid;

                if (!this.Response.IsRequestBeingRedirected)
                {
                    this.Response.Redirect(url);
                }

                return null;
            }
            else
            {
                if (docNumber.IndexOf("HUDOC-") > -1)
                {
                    if (!this.Response.IsRequestBeingRedirected)
                    {
                        this.Response.Redirect("http://hudoc.echr.coe.int/eng#{\"appno\":[\"" + docNumber.Replace("HUDOC-", "") + "\"]}", true);
                    }
                }

                else // CELEX
                {
                    if (!this.Response.IsRequestBeingRedirected)
                    {
                        this.Response.Redirect(Doc.CreateEurLexLink(docNumber, docPrefLangId), true);
                    }
                }

                return null;
            }
        }

        public ActionResult ActByIdentifier(string guid, string toPar)
        {
            var doc = Doc.GetDocByDocIdentifier(guid);
            if (doc != null)
            {
                string url = WebAppHelper.AppRootFolder + "/Doc/" + ((doc.DocType == 1) ? "CourtAct" : "LegalAct") + "/" + doc.DocLangId;
                if (!String.IsNullOrEmpty(toPar))
                {
                    url += "/" + toPar;
                }

                this.Response.Redirect(url);
                return null;
            }
            return null;
        }

        public ActionResult CourtAct(int id, int? siteSearchId)
        {
            CourtAct courtAct = null;

            if (this.CheckRights)
            {
                int[] productIds = Doc.GetDocProducts(id);
                IEnumerable<int> prod = this.UserData.Products.Select(p => p.ProductId).ToArray().Intersect(productIds);
                if (prod.Count() == 0)
                {
                    string[] products = ((List<Product>)this.Session["ProductsList"]).Where(p => productIds.Contains(p.ProductId) == true).Select(p => p.ProductName).ToArray();
                    string errorMsg = "To open this document you need valid license for '" + String.Join("' or '", products) + "'";

                    this.ViewBag.DocError = errorMsg;
                    return this.View(new CourtAct());
                }

            }

            bool showFreeDocs = this.UserData.ShowFreeDocuments;

            #region Search and stats creatings
            if (siteSearchId.HasValue)
            {
                ViewBag.SiteSearchId = siteSearchId.Value;
                //int searchId = int.Parse(siteSearchId.Substring(siteSearchId.LastIndexOf('-') + 1));

                SearchResult sr = SearchResult.FindSearchResult(siteSearchId.Value, this.Session["SearchResults"]);

                if (sr != null)
                {
                    // if text is searched - add to statistic
                    if (sr.StatSearchId.HasValue)
                    {
                        Stat.SetStatSearchDoc(sr.StatSearchId.Value, id);
                    }

                    if (sr.SearchSource == SearchSources.Search)
                    {
                        if (this.Session["local-search-" + id + "-" + siteSearchId] != null && this.Session["local-search-" + id + "-" + siteSearchId].ToString() != "")
                        {
                            var highLightParams = new DocHighlightSearchParams(
                                this.Session["local-search-" + id + "-" + siteSearchId].ToString(),
                                null,
                                Convert.ToBoolean(this.Session["local-search-exactmatch-" + id + "-" + siteSearchId]),
                                null,
                                null);

                            courtAct = CourtActBL.GetCourtAct(
                                id,
                                this.Language.Id,
                                this.UserData.UserId,
                                highLightParams,
                                this.ProductId,
                                showFreeDocs);
                        }
                        else
                        {
                            var complexSearchText = sr.SearchBoxFilters.SearchText;
                            if (sr.SearchBoxFilters.Cases != null && !String.IsNullOrEmpty(sr.SearchBoxFilters.Cases.MultiDict.Text))
                            {
                                if (!String.IsNullOrEmpty(sr.SearchBoxFilters.SearchText))
                                {
                                    complexSearchText = complexSearchText + " AND (";
                                }
                                else
                                {
                                    complexSearchText = "(";
                                }

                                complexSearchText = complexSearchText + sr.SearchBoxFilters.Cases.MultiDict.Text; // combining search text with multilingial dictionary if present
                                complexSearchText = complexSearchText.Replace("OR", "[OR]").Replace("AND", "[AND]"); // trimming logical operators
                                complexSearchText = complexSearchText + ")";
                            }
                            else if (sr.SearchBoxFilters.Law != null && !String.IsNullOrEmpty(sr.SearchBoxFilters.Law.MultiDict.Text))
                            {
                                if (!String.IsNullOrEmpty(sr.SearchBoxFilters.SearchText))
                                {
                                    complexSearchText = complexSearchText + " AND (";
                                }
                                else
                                {
                                    complexSearchText = "(";
                                }

                                complexSearchText = complexSearchText + sr.SearchBoxFilters.Law.MultiDict.Text; // combining search text with multilingial dictionary if present
                                complexSearchText = complexSearchText.Replace("OR", "[OR]").Replace("AND", "[AND]"); // trimming logical operators
                                complexSearchText = complexSearchText + ")";
                            }
                            else if (sr.SearchBoxFilters.MultiDict != null && !String.IsNullOrEmpty(sr.SearchBoxFilters.MultiDict.Text))
                            {
                                if (!String.IsNullOrEmpty(sr.SearchBoxFilters.SearchText))
                                {
                                    complexSearchText = complexSearchText + " AND (";
                                }
                                else
                                {
                                    complexSearchText = "(";
                                }

                                complexSearchText = complexSearchText + sr.SearchBoxFilters.MultiDict.Text; // combining search text with multilingial dictionary if present
                                complexSearchText = complexSearchText.Replace("OR", "[OR]").Replace("AND", "[AND]"); // trimming logical operators
                                complexSearchText = complexSearchText + ")";
                            }

                            var highLightParams = new DocHighlightSearchParams();

                            if (!String.IsNullOrEmpty(sr.SearchBoxFilters.SearchTextMultiLingual))
                            {
                                highLightParams.SearchText = complexSearchText;
                                highLightParams.MultilingualSearchedText = sr.SearchBoxFilters.SearchTextMultiLingual;
                                highLightParams.ExactMatch = sr.SearchBoxFilters.ExactMatch;

                                if (sr.SearchBoxFilters.Cases != null && !String.IsNullOrEmpty(sr.SearchBoxFilters.Cases.NatID_ECLI))
                                {
                                    highLightParams.SearchedCelex = sr.SearchBoxFilters.Cases.NatID_ECLI;
                                }
                                else

                                    courtAct = CourtActBL.GetCourtAct(
                                        id,
                                        this.Language.Id,
                                        this.UserData.UserId,
                                        highLightParams,
                                        this.ProductId,
                                        showFreeDocs);
                            }
                            else
                            {
                                highLightParams.SearchText = complexSearchText;
                                highLightParams.ExactMatch = sr.SearchBoxFilters.ExactMatch;

                                if (sr.SearchBoxFilters.Cases != null)
                                {
                                    if (!String.IsNullOrEmpty(sr.SearchBoxFilters.Cases.EnactmentCelex))
                                    {
                                        highLightParams.SearchedCelex = sr.SearchBoxFilters.Cases.EnactmentCelex;
                                    }

                                    if (!String.IsNullOrEmpty(sr.SearchBoxFilters.Cases.ProvisionParOriginal))
                                    {
                                        var searchedPars = new List<string>();
                                        searchedPars.Add(sr.SearchBoxFilters.Cases.ProvisionParOriginal);
                                        highLightParams.SearchedPars = searchedPars;
                                    }
                                }

                                courtAct = CourtActBL.GetCourtAct(
                                    id,
                                    this.Language.Id,
                                    this.UserData.UserId,
                                    highLightParams,
                                    this.ProductId,
                                    showFreeDocs);
                            }
                        }
                    }
                    else if (sr.SearchSource == SearchSources.InLinks)
                    {
                        var highlightParams = new DocHighlightSearchParams();
                        if (!String.IsNullOrEmpty(sr.SearchBoxFilters.DocInLinks.ToDocNumber))
                        {
                            highlightParams.SearchedCelex = sr.SearchBoxFilters.DocInLinks.ToDocNumber;
                        }

                        if (sr.SearchBoxFilters.DocInLinks.ToPars != null && sr.SearchBoxFilters.DocInLinks.ToPars.Count > 0)
                        {
                            highlightParams.SearchedPars = sr.SearchBoxFilters.DocInLinks.ToPars;
                        }

                        /* Highlight for inlinks should go here */

                        courtAct = CourtActBL.GetCourtAct(
                             id,
                             this.Language.Id,
                             this.UserData.UserId,
                             highlightParams,
                             this.ProductId,
                             showFreeDocs);
                    }
                }
            }
            else if (this.Session["local-search-" + id + "-" + siteSearchId] != null && this.Session["local-search-" + id + "-" + siteSearchId].ToString() != "")
            {
                var highLightParams = new DocHighlightSearchParams();
                highLightParams.SearchText = this.Session["local-search-" + id + "-" + siteSearchId].ToString();
                highLightParams.ExactMatch = Convert.ToBoolean(this.Session["local-search-exactmatch-" + id + "-" + siteSearchId]);

                courtAct = CourtActBL.GetCourtAct(
                    id,
                    this.Language.Id,
                    this.UserData.UserId,
                    highLightParams,
                    this.ProductId,
                    showFreeDocs);
            }
            else if (this.Session["local-search-" + id] != null && this.Session["local-search-exactmatch-" + id] != null)
            {
                var highLightParams = new DocHighlightSearchParams();
                highLightParams.SearchText = this.Session["local-search-" + id].ToString();
                highLightParams.ExactMatch = Convert.ToBoolean(this.Session["local-search-exactmatch-" + id + "-" + siteSearchId]);

                courtAct = CourtActBL.GetCourtAct(
                    id,
                    this.Language.Id,
                    this.UserData.UserId,
                    highLightParams,
                    this.ProductId,
                    showFreeDocs);
            }

            if (courtAct == null)
            {
                var highLightParams = new DocHighlightSearchParams(null, null, false, null, null);

                courtAct = CourtActBL.GetCourtAct(
                    id,
                    this.Language.Id,
                    this.UserData.UserId,
                    highLightParams,
                    this.ProductId,
                    showFreeDocs);
            }

            #endregion

            if (courtAct != null)
            {
                // Rewriting inline links before visualization
                var linkRewriter = new DocLinksRewriter(courtAct.LangId, WebAppHelper.AppRootFolder, this.UserData.OpenDocumentsInNewTab, this.NationalLegislationMapPath);
                courtAct.RewriteInLineLinks(linkRewriter);

                this.ViewBag.Title = courtAct.HtmlModel.Title.Value.ToString();
                //only limited number of docs are avaiable in sysdemo
                //List<string> avaiableDocsCelexes = ConfigurationManager.AppSettings["SysDemoDocumentsCelex"].ToString().Split(',').ToList();
                //if (this.UserData.Username.ToLower() == "sysdemo")
                //{
                //    Regex reglamentsAndDirectivesRegex = new Regex("3.{4}[RrLl](.*)");

                //    if (!((courtAct.DocType == 2 && !courtAct.DocNumber.StartsWith("7")) ||
                //        avaiableDocsCelexes.Any(c => c == courtAct.DocNumber) ||
                //        (courtAct.DocType == 1 && courtAct.DocNumber.Substring(0, 1) == "6" && courtAct.Country.ToLower() == "eu") ||
                //        reglamentsAndDirectivesRegex.IsMatch(courtAct.DocNumber) ||
                //        Doc.IsInternationalStandartDoc(courtAct.DocLangId) ||
                //        Doc.IsDemoDoc(courtAct.DocLangId)))
                //    {
                //        if (Common.CheckRequestOriginIsBotSoft(this.Request.UserAgent) || this.DemoBonusDocumentsCheck(courtAct.DocNumber) == false)
                //        {
                //            var productFeaturesInfo = new ProductFeaturesInfo(FunctionalityTypes.DocumentOpen, courtAct);
                //            string viewName = this.ProductId == 1 ? "~/Views/Shared/_ProductFeaturesInfo_Wrapper.cshtml" : "~/Views/Shared/_ProductFeaturesInfo_Finances.cshtml";
                //            this.ViewBag.UseLayout = true;
                //            return this.View(viewName, productFeaturesInfo);
                //        }
                //        else
                //        {
                //            // refresh bonus docs cookie
                //            var currentBonusDocsCookie = this.Request.Cookies["dmbnsdcs"];
                //            var newCookie = this.ComputeDemoBonusDocsCookie(currentBonusDocsCookie, courtAct.DocNumber);
                //            this.Response.Cookies.Set(newCookie);
                //        }
                //    }
                //}

                // add ro recent documents
                int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["RecentDocumentsCount"]);
                int curProductId = 1;
                if (this.Session["SelectedProductId"] != null)
                {
                    curProductId = int.Parse(this.Session["SelectedProductId"].ToString());
                }

                if (this.UserData.Username.ToLower() != "sysdemo" ||
                    (this.UserData.Username.ToLower() == "sysdemo" && !Common.CheckRequestOriginIsBotSoft(this.Request.UserAgent)))
                {
                    Doc.AddRecentDoc(this.UserData.UserId, id, maxCount, curProductId); // add to recent documents
                    Doc.AddOpenedDoc(this.UserData.UserId, id, curProductId); // add to stat
                }

                if (courtAct.BloblInfo.IsBlob)
                {
                    var contentType = MimeMapping.GetMimeMapping(courtAct.BloblInfo.Path);

                    return this.File(fileName: courtAct.BloblInfo.Path, contentType: contentType);
                }
                else
                {
                    return this.View(courtAct);
                }
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        private bool DemoBonusDocumentsCheck(string docNumber)
        {
            var documentsOpenedCookie = this.Request.Cookies["dmbnsdcs"];
            var maxBonusDocumentsCount = Convert.ToInt32(ConfigurationManager.AppSettings["DemoBonusDocsCount"]);

            if (documentsOpenedCookie == null)
            {
                return true;
            }
            else
            {
                var bonusDocumentsOpened = documentsOpenedCookie.Value.Split('&').ToList();
                if (bonusDocumentsOpened.Count > maxBonusDocumentsCount)
                {
                    return false; // cookie is manually forged or incorect
                }

                if (bonusDocumentsOpened.Any(bdn => bdn == docNumber))
                {
                    return true; // is previously added
                }
                else if (bonusDocumentsOpened.Count < maxBonusDocumentsCount)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private HttpCookie ComputeDemoBonusDocsCookie(HttpCookie currentCookie, string newlyOpenedDocNumber)
        {
            var newDocNumbersList = new List<string>();
            newDocNumbersList.Add(newlyOpenedDocNumber);

            if (currentCookie != null)
            {
                var previouslyOpenedDocNumbers = currentCookie.Value.Split('&').ToList();

                // don't add new document if count exceedes config, cookie is manually forged or incorect
                if (previouslyOpenedDocNumbers.Count < Convert.ToInt32(ConfigurationManager.AppSettings["DemoBonusDocsCount"]))
                {
                    newDocNumbersList.AddRange(previouslyOpenedDocNumbers);
                    newDocNumbersList = newDocNumbersList.Distinct().ToList();
                }
            }

            var newCookie = new HttpCookie("dmbnsdcs");
            newCookie.Value = String.Join("&", newDocNumbersList);
            newCookie.Expires.AddDays(365);

            return newCookie;
        }

        [HttpGet]
        public ActionResult Vat32006L0112(int? id, string docNum)
        {
            var userData = this.Session["UserData"] as UserData;
            bool hasFinancesProduct = userData.Products.Select(x => x.ProductId == 2).ToList().Count > 0 ? true : false;
            if (!hasFinancesProduct)
            {
                return this.RedirectToAction("Index", "Home");
            }

            if (this.ProductId == 1)
            {
                this.Session["SelectedProductId"] = "2";

                var updatedCookie = new System.Web.HttpCookie("SelectedProductId")
                {
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTime.Now.AddDays(365)
                };
                updatedCookie.Value = "2";
                this.Response.Cookies.Set(updatedCookie);
            }

            Document courtAct = null;
            if (id == null)
            {
                string docNumber = docNum ?? "32006L0112";
                var document = Doc.GetDocByDocNumber(docNumber, this.Language.Id, this.UserData.UserId);
                id = document.DocLangId;
            }

            bool showFreeDocs = this.UserData.ShowFreeDocuments;
            //if (Session["local-search-" + id + "-" + siteSearchId] != null && Session["local-search-" + id + "-" + siteSearchId].ToString() != "")
            //{
            //    var highLightParams = new DocHighlightSearchParams();
            //    highLightParams.SearchText = Session["local-search-" + id + "-" + siteSearchId].ToString();
            //    highLightParams.ExactMatch = Convert.ToBoolean(Session["local-search-exactmatch-" + id + "-" + siteSearchId]);

            //    courtAct = CourtActBL.GetCourtAct(
            //        docLangId,
            //        this.Language.Id,
            //        Languages.GetXsltRubrics(typeof(Resources.Resources)),
            //        c => Languages.GetXsltRubricsByCulture<Resources.Resources>(c),
            //        UserData.UserId,
            //        highLightParams,
            //        ProductId,
            //        showFreeDocs);
            //}
            if (this.Session["local-search-" + id] != null && this.Session["local-search-exactmatch-" + id] != null)
            {
                var highLightParams = new DocHighlightSearchParams();
                highLightParams.SearchText = this.Session["local-search-" + id].ToString();
                highLightParams.ExactMatch = false;

                courtAct = CourtActBL.GetCourtAct(
                    id.Value,
                    this.Language.Id,
                    this.UserData.UserId,
                    highLightParams,
                    this.ProductId,
                    showFreeDocs);
            }

            if (courtAct == null)
            {
                courtAct = CourtActBL.GetCourtAct(
                        id.Value,
                        this.Language.Id,
                        this.UserData.UserId,
                        new DocHighlightSearchParams(),
                        this.ProductId,
                        showFreeDocs);
            }

            var docConts = Doc.GetDocContents(courtAct.DocLangId);
            var vm = new Vat32006L0112(courtAct, docConts);
            return this.View("~/Views/Doc/Vat32006L0112.cshtml", vm);
        }

        [HttpGet]
        public ActionResult TFEU(int? id)
        {
            return this.Vat32006L0112(id, "12012E");
        }

        [Obsolete]
        public ActionResult LegalAct(int id, int? siteSearchId)
        {
            return this.CourtAct(id, siteSearchId);
        }

        [HttpGet]
        public ActionResult ParHint(string linkType, int LangIdFromDoc, string toDocNumber, string toPar, bool lastConsWithText)
        {
            toDocNumber = toDocNumber.Replace("_", "/");
            Document d = Doc.GetParText(
                linkType,
                toDocNumber,
                toPar,
                LangIdFromDoc,
                this.UserData.UserId,
                this.Language.Id,
                lastConsWithText,
                true,
                false,
                false);

            d.CountryName = Resources.Res.GetCountryNameByCode(d.Country);
            return this.Json(d, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetNewRefDocsListArticle(int docLangId, int[] docParIds, int langId)
        {
            var refDocsArticle = Doc.GetNewArticleRefDocs(this.UserData.UserId, docLangId, this.Language.Id, docParIds, langId);
            this.ViewBag.DocLangId = docLangId;
            string linksHtml = this.GetViewString("_RefDocsListArticle", refDocsArticle, false);
            var linkRewriter = new DocLinksRewriter(this.Language.Id, WebAppHelper.AppRootFolder, this.UserData.OpenDocumentsInNewTab, this.NationalLegislationMapPath);
            linksHtml = linkRewriter.RewriteDocInLinks(linksHtml, "inline_link", true);

            return this.Content(linksHtml);
        }

        public ActionResult GetOldEuRefDocsListArticle(int docLangId, int[] docParIds, int langId)
        {
            var refDocsArticle = Doc.GetOldArticleEuRefDocs(this.UserData.UserId, docLangId, this.Language.Id, docParIds, langId);
            this.ViewBag.DocLangId = docLangId;

            return this.Content(this.RewriteHtmlOldRefDocs(refDocsArticle));
        }

        public ActionResult GetOldFinsRefDocsListArticle(int docLangId, int[] docParIds, int langId)
        {
            var refDocsArticle = Doc.GetOldArticleFinsRefDocs(this.UserData.UserId, docLangId, this.Language.Id, docParIds, langId);
            this.ViewBag.DocLangId = docLangId;

            return this.Content(this.RewriteHtmlOldRefDocs(refDocsArticle));
        }

        private string RewriteHtmlOldRefDocs(IEnumerable<RefDocsPair> refDocsArticle)
        {
            string linksHtml = this.GetViewString("_RefDocsPartialList", refDocsArticle, false);
            var linkRewriter = new DocLinksRewriter(this.Language.Id, WebAppHelper.AppRootFolder, this.UserData.OpenDocumentsInNewTab, this.NationalLegislationMapPath);
            linksHtml = linkRewriter.RewriteDocInLinks(linksHtml, "inline_link", true);

            return linksHtml;
        }

        [HttpGet]
        public ActionResult ParHintById(int id, string articleEid = null)
        {
            Document d = Doc.ParHint(id, this.ProductId, this.Language.Id, WebAppHelper.AppRootFolder);
            d.CountryName = Resources.Res.GetCountryNameByCode(d.Country);
            if (!String.IsNullOrEmpty(articleEid))
            {
                int docParId = Doc.GetDocParIdByEid(articleEid, id);
                d.Text = Doc.GetDocParByParId(d.DocLangId, docParId, this.Language.Id,
                                d.LangId,
                                null,
                                WebAppHelper.AppRootFolder,
                                true);

                d.ArticlePathJSON = Doc.GetPathToArticle(docParId);
            }

            return this.Json(d, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ParTextByDocParId(int docLangId, int langIdFromDoc, int docParId, string searchText, bool? isArticle)
        {
            string content;

            if (isArticle.HasValue && isArticle.Value)
            {
                content = Doc.GetDocArtTextByDocParId(docLangId, docParId, this.Language.Id,
                                                langIdFromDoc,
                                                searchText);
            }
            else
            {
                content = Doc.GetDocParByParId(docLangId, docParId, this.Language.Id,
                                                langIdFromDoc,
                                                searchText,
                                                WebAppHelper.AppRootFolder);
            }


            var linkRewriter = new DocLinksRewriter(this.Language.Id, WebAppHelper.AppRootFolder, this.UserData.OpenDocumentsInNewTab, this.NationalLegislationMapPath);
            content = linkRewriter.RewriteDocInLinks(content, "inline_link", true);

            return this.Json(new { articleContent = content });
        }

        [HttpPost]
        public JsonResult ArticlesCorrelation(int docParId, int langId)
        {
            var articlesCorrelation = Doc.GetArticlesCorrelation(this.UserData.UserId, this.Language.Id, docParId, langId).ToList();
            var linkRewriter = new DocLinksRewriter(this.Language.Id, WebAppHelper.AppRootFolder, this.UserData.OpenDocumentsInNewTab, this.NationalLegislationMapPath);
            string content = this.GetViewString("_ArticlesCorrelation", articlesCorrelation, false);
            content = linkRewriter.RewriteDocInLinks(content, "inline_link", true);
            return this.Json(new { articlesCorrelation = content });
        }

        [HttpGet]
        public ActionResult ModHint(string fromDocNumber, int langIdFromDoc, string toDocNumber, string modType)
        {
            toDocNumber = toDocNumber.Replace("_", "/");
            fromDocNumber = fromDocNumber.Replace("_", "/");

            //Document d = Doc.GetParText(toDocNumber, toPar, LangIdFromDoc, this.UserData.UserId, this.Language.Id, false, false, Languages.GetXsltRubrics(typeof(Resources.Resources)));
            //d.CountryName = Resources.Res.GetCountryNameByCode(d.Country);
            Document d = new Document();
            d.Text = Doc.GetModHint(fromDocNumber, toDocNumber, langIdFromDoc, this.Language.Id, this.UserData.UserId, modType);
            d.DocNumber = toDocNumber;
            d.DocType = 2; // legal act
            return this.Json(d, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DocHintByIdentifier(string identifier, string toPar)
        {
            var doc = Doc.GetDocHintByDocIdentifier(identifier, toPar, this.Language.Id);
            if (doc == null)
            {
                doc = new Document();
            }
            else
            {
                doc.CountryName = Resources.Res.GetCountryNameByCode(doc.Country);
            }

            return this.Json(doc, JsonRequestBehavior.AllowGet);
        }

        public string GetLangShortCodeMT()
        {
            string viewName = "../Home/MachineTranslation";
            string shortCode = this.Language.ShortCode;

            if (this.ViewExists(viewName + shortCode))
            {
                return shortCode;
            }

            var prefs = new LanguagePreferences(this.UserData.UserId);
            var languages = prefs.Languages;
            foreach (var pref in languages)
            {
                if (this.ViewExists(viewName + pref.lang.ShortCode))
                {
                    return pref.lang.ShortCode;
                }
            }

            return "en";
        }
    }
}
