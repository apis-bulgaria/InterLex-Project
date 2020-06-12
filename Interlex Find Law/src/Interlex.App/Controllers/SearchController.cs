namespace Interlex.App.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Configuration;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Entities;
    using Interlex.BusinessLayer.Models;
    using Interlex.BusinessLayer.Enums;
    using Interlex.App.Filters;
    using Interlex.App.Resources;
    using System.Diagnostics;
    using ClassificationService;
    using Newtonsoft.Json;
    //using Newtonsoft.Json;

    [UserAuthorize]
    public class SearchController : BaseSearchResultController
    {
        /// <summary>
        /// Called from DoSearch() javascript function by ajax post and other controller methods
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult SetLocalSearchText(string siteSearchId, string docLangId, string searchText, bool exactMatch)
        {
            /*  SearchResult sr = SearchResult.FindSearchResult(int.Parse(siteSearchId), Session["SearchResults"]);
              if (sr != null)
              {
                  sr.LocalSearchTexts[int.Parse(docLangId)] = searchText;
                  return Json("OK");
              }*/

            Session.Add("local-search-" + docLangId, searchText);
            Session.Add("local-search-exactmatch-" + docLangId, exactMatch);

            if (!(String.IsNullOrEmpty(siteSearchId)))
            {
                Session.Add("local-search-" + docLangId + "-" + siteSearchId, searchText);
                Session.Add("local-search-exactmatch-" + docLangId + "-" + siteSearchId, exactMatch);
            }

            return Json("OK");
        }

        [HttpPost]
        public ActionResult PalProvisions(int docLangId, bool? isIndexSearch)
        {
            List<PalProvision> l = Doc.GetActProvisionsLinkedByCases(docLangId, Res.GetParTitles(typeof(Resources)), null);
            if (isIndexSearch.HasValue)
            {
                ViewBag.IsIndexSearch = isIndexSearch.Value;
            }
            else
            {
                ViewBag.IsIndexSearch = false;
            }

            return View("~/Views/SearchResult/PalProvisions.cshtml", l);
        }

        [HttpPost]
        public ActionResult GetDocArticleProvisions(int docLangId, string eid, string toParId, string toDocNumber)
        {
            List<PalProvision> l = Doc.GetDocArticleProvisions(docLangId, eid, Res.GetParTitles(typeof(Resources)), this.ProductId, this.UserData.ShowFreeDocuments);
            l = l.OrderBy(i => i.Title).ToList();

            ViewBag.ToDocLangId = docLangId;
            ViewBag.ToParId = toParId;
            ViewBag.ToDocNumber = toDocNumber;
            ViewBag.EID = eid;

            return PartialView("~/Views/Doc/_DocArticleProvisionsList.cshtml", l);

            //  return Json(l, JsonRequestBehavior.AllowGet);
        }

        private string PrepairReferedActECHRSearchQuery(string applicant, string applicationNumber, string ecli)
        {
            string searchQuery = "publisher_id:hudoc";

            if (!String.IsNullOrEmpty(applicant))
            {
                searchQuery += " caption:" + applicant;
            }

            if (!String.IsNullOrEmpty(applicationNumber))
            {
                searchQuery += " docnumber:" + applicationNumber.Replace("/", "");
            }

            if (!String.IsNullOrEmpty(ecli))
            {
                searchQuery += " eliecli:" + ecli;
            }

            return searchQuery;
        }

        public ActionResult ReferedActECHRSearch(string applicant, string applicationNumber, string ecli, int page)
        {
            if (page == 0)
            {
                page = 1;
            }

            var searchModel = new ReferedActECHRSearchModel();
            var count = this.ReferedActECHRSearchCount(applicant, applicationNumber, ecli);

            if (Session["ReferedActECHRDocs"] != null)
            {
                int[] docs = ((int[])Session["ReferedActECHRDocs"]).Skip((page - 1) * 20).Take(20).ToArray();

                searchModel.Documents = Doc.GetReferedActECHRDocList(docs);
            }

            return PartialView("~/Views/SearchResult/_ReferedActECHRSearch.cshtml", searchModel);
        }

        public int ReferedActECHRSearchCount(string applicant, string applicationNumber, string ecli)
        {
            //  var searchModel = new ReferedActECHRSearchModel();
            var searchQuery = this.PrepairReferedActECHRSearchQuery(applicant, applicationNumber, ecli);

            int[] wsRes = null;

            SearchResult sr = null;

            string searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
            sr = new SearchResult(SearchSources.HomePage, HttpContext.Application["SearchWrapper"],
                     searchWrapperUrl,
                     HttpContext.Application["FilterDocsStruct"],
                     HttpContext.Application["FilterDocsClassifiers"],
                     HttpContext.Application["ClassifiersMap"],
                     HttpContext.Application["ResultsGroupper"],
                     HttpRuntime.AppDomainAppPath,
                     this.Language.Id, 20, 10);

            int[] langPref = UserMng.GetUserLangPrefForSearch(UserData.UserId, Language.Id);

            sr.SearchFTQuery(searchQuery, ref wsRes, langPref);
            wsRes = Interlex.BusinessLayer.Search.GetReferedActECHRSearchResult(wsRes);

            Session["ReferedActECHRDocs"] = wsRes;

            return wsRes.Length;
        }

        private string PrepairPalSearchQuery(string classifierId, string searchText, string searchNumber, string searchYear, string searchDocNumber)
        {
            string searchQuery = "props:(dtact) props:(cEU)";
            if (!String.IsNullOrEmpty(classifierId))
                searchQuery += " classificators:(" + classifierId.Replace("-", "") + ")";
            if (!String.IsNullOrEmpty(searchText))
                searchQuery += " text:(" + searchText + ")";
            if (!String.IsNullOrEmpty(searchDocNumber))
                searchQuery += " docnumber:(" + searchDocNumber + ")";
            if (!String.IsNullOrEmpty(searchNumber) || !String.IsNullOrEmpty(searchYear))
            {
                //searchQuery += " (docnumber:(" + searchYear + "?" + searchNumber + ") || docnumber:(" + searchYear + "??" + searchNumber + "))";
                string[] prefix = new string[4] { "1", "2", "3", "4" };
                string numberMask = "";
                if (!String.IsNullOrEmpty(searchYear) && searchYear.Length == 4)
                    numberMask += searchYear;
                else
                    numberMask += "????";
                numberMask += "?"; // Letter
                if (!String.IsNullOrEmpty(searchNumber))
                    numberMask += searchNumber.PadLeft(4, '0');
                else
                    numberMask += "????";

                string[] numberSearchQuery = new string[prefix.Length];
                for (int i = 0; i < prefix.Length; i++)
                    numberSearchQuery[i] = "docnumber:(" + prefix[i] + numberMask + ")";

                searchQuery += " (" + String.Join(" || ", numberSearchQuery) + ")";
            }
            return searchQuery;
        }

        // query is for fins legal act
        [HttpGet]
        public int PalSearchTotalCount(string classifierId, string searchText, string searchNumber, string searchYear, string searchDocNumber, string query)
        {
            Session["PalDocs"] = null;
            string searchQuery;
            int totalCount = 0;
            if (!String.IsNullOrEmpty(query))
            {
                searchQuery = query;
            }
            else
            {
                searchQuery = PrepairPalSearchQuery(classifierId, searchText, searchNumber, searchYear, searchDocNumber);
            }

            if (!String.IsNullOrEmpty(searchQuery))
            {
                string searchWrapperUrl = null;
                if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
                }
                else
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath_cc"];
                }

                SearchResult sr = new SearchResult(SearchSources.HomePage, HttpContext.Application["SearchWrapper"],
                        searchWrapperUrl,
                        HttpContext.Application["FilterDocsStruct"],
                        HttpContext.Application["FilterDocsClassifiers"],
                        HttpContext.Application["ClassifiersMap"],
                        HttpContext.Application["ResultsGroupper"],
                        HttpRuntime.AppDomainAppPath,
                        this.Language.Id, 20, 10);

                int[] wsRes = null;
                int[] langPref = UserMng.GetUserLangPrefForSearch(UserData.UserId, Language.Id);

                sr.SearchFTQuery(searchQuery, ref wsRes, langPref);

                int curProductId = 1;
                if (Session["SelectedProductId"] != null)
                {
                    curProductId = int.Parse(Session["SelectedProductId"].ToString());
                }

                wsRes = Interlex.BusinessLayer.Search.GetPALSearchResult(wsRes, curProductId);
                Session["PalDocs"] = wsRes;
                totalCount = wsRes.Length;
            }

            return totalCount;
        }

        [HttpPost]
        public ActionResult PalSearch(string classifierId, string searchText, string searchNumber, string searchYear, string searchDocNumber, int page, bool? isIndexSearch)
        {
            PalSearch palSearch = new PalSearch();
            if (Session["PalDocs"] != null)
            {
                int[] docs = ((int[])Session["PalDocs"]).Skip((page - 1) * 20).Take(20).ToArray();
                palSearch.Documents = Doc.GetPALDocList(docs);
            }

            if (isIndexSearch.HasValue)
            {
                palSearch.IsIndexSearch = isIndexSearch.Value;
            }

            return View("~/Views/SearchResult/PalSearch.cshtml", palSearch);
        }

        [HttpPost]
        public ActionResult Search(SearchBox model)
        {
            if (!model.ShowFreeDocuments.HasValue)
            {
                model.ShowFreeDocuments = this.UserData.ShowFreeDocuments;
            }

            try
            {
                SearchSources searchSource = SearchSources.Search;
                if (model.ClassifierFilter != null)
                    searchSource = SearchSources.Classifier;
                else if (model.DocInLinks != null)
                    searchSource = SearchSources.InLinks;
                else if (model.HomeSearch != null)
                    searchSource = SearchSources.HomePage;

                string searchWrapperUrl = null;
                if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
                }
                else
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath_cc"];
                }

                //if (searchSource == SearchSources.Search)
                //{
                //    if ((model.Cases != null && model.Cases.TranslateSearchText) ||
                //        (model.Law != null && model.Law.TranslateSearchText))
                //    {
                //        string textLang = 
                //        model.SearchTextMultiLingual = (new BanTranslateService()).Translate(model.SearchText, "bg", "en");

                //    }
                //}


                SearchResult sr = new SearchResult(searchSource, HttpContext.Application["SearchWrapper"],
                    searchWrapperUrl,
                    HttpContext.Application["FilterDocsStruct"],
                    HttpContext.Application["FilterDocsClassifiers"],
                    HttpContext.Application["ClassifiersMap"],
                    HttpContext.Application["ResultsGroupper"],
                    HttpRuntime.AppDomainAppPath,
                    this.Language.Id, pageSize, visiblePagesCount);

                // searchbox model passing
                sr.SearchBoxFilters = model;

                // passing current product
                int curProductId = 1;
                if (Session["SelectedProductId"] != null)
                {
                    curProductId = int.Parse(Session["SelectedProductId"].ToString());
                }

                sr.ProductId = curProductId;

                //sr.DisplayContext = true;

                if (model.HomeSearch != null)
                {
                    if (model.HasRelSort)
                    {
                        sr.SortBy = "rel";
                        sr.SortDir = "asc";
                    }
                    else
                    {
                        sr.SortBy = "date";
                        sr.SortDir = "desc";
                    }
                }

                int? searchId = SearchResult.AddSearchResult(sr, Session["SearchResults"]);
                Session["search-global-" + sr.SearchBoxFilters.GlobalId] = searchId;

                if (Request.IsAjaxRequest())
                {
                    if (searchId.HasValue)
                        return Json(new { result = "Redirect", url = Url.Action("List", "Search", new { searchId = searchId }) });
                    else
                        return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
                }
                else
                {
                    if (searchId.HasValue)
                        return RedirectToAction("List", "Search", new { searchId = searchId });
                    else
                        return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                var logsDirectory = HttpRuntime.AppDomainAppPath;
                logsDirectory = logsDirectory + "Logs\\Searches\\";

                var timeOfLoging = DateTime.Now;
                var timePathParsed = "";
                timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_" + timeOfLoging.Day;

                var fullPath = logsDirectory + timePathParsed + ".txt";

                var searchDetails = "Search: " + JsonConvert.SerializeObject(model);
                var userId = "User: " + UserData.Username;

                var innerExceptionMessage = String.Empty;
                if (e.InnerException != null)
                {
                    innerExceptionMessage = e.InnerException.Message;
                }

                //File.Create(fullPath);
                System.IO.File.AppendAllText(fullPath, searchDetails +
                    Environment.NewLine + userId +
                    Environment.NewLine + "Exception message: " + e.Message +
                    Environment.NewLine + "InnerExceptionMessage: " + innerExceptionMessage +
                    Environment.NewLine + Environment.NewLine);

                return Json("Error");
            }
        }

        public ActionResult ByFilter(string classifierId)
        {
            SearchBox sb = new SearchBox(Language.Id);
            sb.ClassifierFilter = Guid.Parse(classifierId);

            Language lang = InterfaceLanguages.GetLanguageById(Convert.ToInt32(Session["LanguageId"]));
            String languageId = lang.Id.ToString();

            ClassificatorTreeModel treeModel = ClassifiersProvider.Service.GetTreeByGuid(classifierId);

            IEnumerable<ClassificatorModel> flatten = treeModel.FlattenTo();

            bool isCurrentLanguageAvailable = flatten.All(f => f.LanguageVariantsWithHints.ContainsKey(languageId));

            if (!isCurrentLanguageAvailable)
            {
                // Use english by default (Id = 4)
                lang = InterfaceLanguages.GetLanguageById(4);
            }

            String filter = String.Join(" / ", flatten.Select(x => x.LanguageVariantsWithHints[languageId].Title));

            sb.ClassifierFilterTitle = filter;

            return Search(sb);
        }

        public ActionResult HomeSearch(int folderId)
        {
            HomeSearchData hsd = Home.GetSearchFolder(folderId);

            if (hsd.QueryType == 3) // redirect link
            {
                //Response.Redirect(hsd.Query, true);
                return Redirect(hsd.Query);
            }
            else
            {
                if (hsd.QueryType == 1) //FT query
                    hsd.Query = "props:pr" + Session["SelectedProductId"].ToString() + " (" + hsd.Query + ")";
                SearchBox sb = new SearchBox(Language.Id);


                if (!sb.ShowFreeDocuments.HasValue)
                {
                    sb.ShowFreeDocuments = this.UserData.ShowFreeDocuments;
                }

                sb.HomeSearch = hsd;
                return Search(sb);
            }
        }

        public ActionResult DocInLinksFromApi(int docPrefLangId, int siteLangId, string domain, string docCelex, string toPar)
        {
            // site language force refresh
            Session["LanguageId"] = siteLangId;
            var langModelCookie = new HttpCookie("sitelang");
            langModelCookie.Value = InterfaceLanguages.GetLanguageById(siteLangId).Code;
            langModelCookie.Expires.AddDays(365);
            Response.Cookies.Set(langModelCookie);

            // search params
            SearchBox sb = new SearchBox(siteLangId);
            sb.DocInLinks = new DocLinksFilter();
            sb.DocInLinks.IsOriginApi = true;
            sb.DocInLinks.ToDocNumber = docCelex;
            sb.DocInLinks.ToParOriginal = toPar;
            sb.DocInLinks.Domain = domain.ToLower();
            sb.DocInLinks.SiteLangRequestingId = siteLangId;
            sb.DocInLinks.UserRequestingId = UserData.UserId;

            // title
            string[] titleParts = Doc.GetDocLinksTitle(docCelex, toPar, siteLangId, UserData.UserId);
            sb.DocInLinks.Title = String.Format("{0}{1}", titleParts[0], (String.IsNullOrEmpty(titleParts[1]) ? "" : "/" + titleParts[1]))
                + " - " + Resources.UI_IncomingLinks;

            return Search(sb);
        }

        public ActionResult SearchByXmlId(string xmlId, string domain, int langId, int siteLangId)
        {
            // site language force refresh
            Session["LanguageId"] = siteLangId;
            var langModelCookie = new HttpCookie("sitelang");
            langModelCookie.Value = InterfaceLanguages.GetLanguageById(siteLangId).Code;
            langModelCookie.Expires.AddDays(365);
            Response.Cookies.Set(langModelCookie);

            // search params
            SearchBox sb = new SearchBox(langId);
            string classifierGuid = Classifiers.GetClassifierIdByXmlId(xmlId);
            sb.ClassifierFilter = new Guid(classifierGuid);
            sb.ByXmlId = true;

            switch (domain.ToLower())
            {
                case "eucl": sb.Cases = new SearchCases(langId); sb.Cases.CaseLawType = CaseLawType.EU; break;
                case "natcl": sb.Cases = new SearchCases(langId); sb.Cases.CaseLawType = CaseLawType.National; break;
                case "eul": sb.Law = new SearchLaw(langId); break;
                case "natl": sb.Law = new SearchLaw(langId); break;
                default: break;
            }

            // search title
            var classifierModel = ClassificatorService.Current.TryGetTreeByGuid(sb.ClassifierFilter.ToString());
            if (classifierModel != null) // S.P.: correct classifiers population should always have returned a value but I don't trust it :D
            {
                sb.ClassifierFilterTitle = classifierModel.LanguageVariantsWithHints[siteLangId.ToString()].Title;
            }

            return Search(sb);
        }

        public ActionResult DocInLinks(int toDocLangId, int? toParId, int? docLangId, string linkIdsString, string subTitle, string toDocNumber, string toParOriginal)
        {
            SearchBox sb = new SearchBox(Language.Id);
            sb.DocInLinks = new DocLinksFilter();
            sb.DocInLinks.ToDocLangId = toDocLangId;
            sb.DocInLinks.ToParId = toParId;
            sb.DocInLinks.LinkIdsString = linkIdsString;
            sb.DocInLinks.SubTitle = subTitle;
            if (!String.IsNullOrEmpty(sb.DocInLinks.LinkIdsString))
            {
                // populating pars from DB
                sb.DocInLinks.PopulateLinksToPars();
            }

            if (!sb.ShowFreeDocuments.HasValue)
            {
                sb.ShowFreeDocuments = this.UserData.ShowFreeDocuments;
            }

            // still no pars?
            if (sb.DocInLinks.ToPars == null || sb.DocInLinks.ToPars.Count == 0)
            {
                if (String.IsNullOrEmpty(subTitle))
                {
                    if (sb.DocInLinks.ToParId.HasValue)
                    {
                        // getting title if not present (common in the GET variant of the method)
                        subTitle = DocLinksFilter.GetParNumByParId(sb.DocInLinks.ToParId.Value);
                    }
                }

                if (!String.IsNullOrEmpty(subTitle))
                {
                    // populating fictive artN toPar
                    var curToParsCollection = new List<string>();
                    var curToPar = String.Empty;
                    if (subTitle.Contains(".") && subTitle.Contains(" "))
                    {
                        curToPar = subTitle.Split(' ')[1];
                    }
                    else
                    {
                        curToPar = "art" + subTitle[subTitle.Length - 1];
                    }

                    curToParsCollection.Add(curToPar);

                    sb.DocInLinks.ToPars = curToParsCollection;
                }
            }

            // populating docNumber if not present
            if (String.IsNullOrEmpty(toDocNumber))
            {
                toDocNumber = Doc.GetDocumentNumberByDocLangId(toDocLangId);
            }

            string[] title = Doc.GetDocLinksTitle(toDocLangId, toParId);
            if (!String.IsNullOrEmpty(subTitle))
            {
                title[1] = subTitle;
            }

            sb.DocInLinks.Title = String.Format("{0}{1}", title[0], (String.IsNullOrEmpty(title[1]) ? "" : "/" + title[1])) + " - " + Resources.UI_IncomingLinks;

            sb.DocInLinks.ToDocNumber = toDocNumber;
            // sb.DocInLinks.ToParOriginal = toParOriginal;

            return Search(sb);
        }

        public ActionResult DocList(int searchId, int? page, string sort, string sortDir)
        {
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);
            // searchId not found
            if (sr == null)
                return RedirectToAction("Index", "Home");

            sr.CurrentPage = (page.HasValue) ? page.Value : 1;
            sr.SortBy = sort;
            sr.SortDir = sortDir;
            return PartialView("~/Views/Shared/_DocList.cshtml", sr);
        }

        public ActionResult List(int searchId, int? page)
        {
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);
            // searchId not found
            if (sr == null)
                return RedirectToAction("Index", "Home");
            else
            {
                //if (sr.LanguageId != this.Language.Id) // Interface language changed. Re-search to show appropriate document language versions
                //{
                //    sr.ChangeLanguage(this.Language.Id);
                //}

                if (!sr.SearchCreated)
                {
                    sr.CreateSearch(this.UserData.UserId, this.UserData.SessionId);

                    // Check if it is real user search
                    if (sr.SearchSource == SearchSources.Search)
                    {
                        string searchText = (String.IsNullOrWhiteSpace(sr.SearchBoxFilters.SearchText)) ? "" : sr.SearchBoxFilters.SearchText.Trim();
                        int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["UserSearchCount"]);
                        int curProductId = 1;
                        if (Session["SelectedProductId"] != null)
                        {
                            curProductId = int.Parse(Session["SelectedProductId"].ToString());
                        }

                        UserMng.AddUserSearch(this.UserData.UserId, searchText, sr.SearchBoxFilters, maxCount, curProductId);
                    }
                }
                if (page.HasValue)
                    sr.CurrentPage = page.Value;

                //if (Request.IsAjaxRequest())
                //    return Json(new { result = "Redirect", url = Url.Action("_SearchList", "Search", new { searchId = searchId }) });
                //else                
                return View("~/Views/SearchResult/List.cshtml", sr);
            }
        }

        //public ActionResult GetFilterClassifier(int searchId, int classifierTypeId)
        //{
        //    Stopwatch stopwatch = null;
        //    stopwatch = Stopwatch.StartNew();
        //    SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);
        //    // searchId not found
        //    if (sr == null)
        //    {
        //        return null;
        //    }

        //    Guid? parent = null;
        //    if (sr.ClassifierFilters.ContainsKey(classifierTypeId))
        //        parent = sr.ClassifierFilters[classifierTypeId].Last().Id;

        //    List<ClassifierItem> l = BusinessLayer.Search.GetFilterClassifier(sr.DbSearchId, parent, (ClassifierTypes)classifierTypeId, this.Language.Id, sr.ClassifierFilterIds.ToArray());

        //    stopwatch.Stop();
        //    string perf = "GetFilterClassifier: " + stopwatch.ElapsedMilliseconds.ToString() + "ms" + Environment.NewLine;
        //    System.IO.File.AppendAllText(Server.MapPath("~\\Logs\\perf.log"), perf);
        //    return Json(l);
        //}

        public ActionResult GetFilterClassifiers(int searchId)
        {
            //  Stopwatch stopwatch = null;
            //   stopwatch = Stopwatch.StartNew();
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);
            // searchId not found
            if (sr == null)
            {
                return null;
            }

            FilterClassifierItemsResponse model = sr.GetFilterClassifiers(this.Language.Id);

            //   stopwatch.Stop();
            //   string perf = "GetFilterClassifiers: " + stopwatch.ElapsedMilliseconds.ToString() + "ms" + Environment.NewLine;
            //   System.IO.File.AppendAllText(Server.MapPath("~\\Logs\\perf.log"), perf);
            return Json(model);
        }

        [HttpPost]
        public ActionResult ChangeLinkedDocumentsOnlyState(int searchId, bool state)
        {
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);

            if (sr == null)
            {
                return new EmptyResult(); // consider something more user-friendly
            }

            sr.SearchBoxFilters.ShowFreeDocuments = !state;

            return Search(sr.SearchBoxFilters);
        }

        public void SetFilterClassifier(int searchId, int classifierTypeId, Guid id)
        {
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);
            // searchId not found
            if (sr == null)
            {
                return;
            }

            string name = Request.Form["name"].ToString().Substring(0, Request.Form["name"].ToString().LastIndexOf('(') - 1);
            sr.AddFilterClassifier(id, classifierTypeId, name, this.Language.Id);
        }

        public void RemoveFilterClassifier(int searchId, int classifierTypeId, Guid id)
        {
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);
            // searchId not found
            if (sr == null)
            {
                return;
            }

            sr.RemoveFilterClassifier(id, classifierTypeId);
        }


        public ActionResult MultiDictSearch(int langId, string searchText, string leadingCharacter)
        {
            if ((String.IsNullOrEmpty(searchText) || searchText.Length < 2) && leadingCharacter == "all")
            {
                return PartialView("~/Views/Shared/_SearchBoxFormMultiDictionary.cshtml", new List<MultiDictItem>());
            }
            else
            {
                ViewBag.ShowInitialLabel = false;
            }

            if (leadingCharacter == null || leadingCharacter == "all")
            {
                leadingCharacter = "";
            }

            var model = new List<MultiDictItem>();

            model = MultiDictItem.GetMultiDictSearchItems(langId, searchText, leadingCharacter);

            // showing initial selected functionality
            var multiDictCookie = Request.Cookies["selectedMultiDictIds"];
            var multiDictValues = new Dictionary<string, int[]>();
            if (multiDictCookie != null)
            {
                multiDictValues = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(multiDictCookie.Value);
            }

            foreach (var item in model)
            {
                if (multiDictValues.ContainsKey("item-" + item.ItemId))
                {
                    item.Selected = true;
                }
            }

            return PartialView("~/Views/Shared/_SearchBoxFormMultiDictionary.cshtml", model);
            // return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MultiDictGetTranslations(string id)
        {
            var model = new List<MultiDictTranslation>();

            model = MultiDictTranslation.GetMultiDictItemTranslations(id).ToList();

            // showing initial selected functionality
            var multiDictCookie = Request.Cookies["selectedMultiDictIds"];
            var multiDictValues = new Dictionary<string, int[]>();
            if (multiDictCookie != null)
            {
                multiDictValues = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(multiDictCookie.Value);
            }

            foreach (var item in model)
            {
                if (multiDictValues.ContainsKey("item-" + id) && multiDictValues["item-" + id].Contains(item.LangId))
                {
                    item.Selected = true;
                }
            }

            ViewBag.ItemId = id;
            return PartialView("~/Views/Shared/_SearchBoxFormMultiDictionaryTranslations.cshtml", model);
        }

        public ActionResult MultiDictGetLanguageAlphabet(int langId)
        {
            var model = Languages.GetAlphabetByLangId(langId);

            return PartialView("~/Views/Shared/_SearchBoxFormCasesMultiDictionaryAlphabet.cshtml", model);
        }

        public void RemoveAllFilterClassifiers(int searchId)
        {
            SearchResult sr = SearchResult.FindSearchResult(searchId, Session["SearchResults"]);

            if (sr == null)
            {
                return;
            }

            sr.RemoveAllFilterClassifiers();
        }

    }

}
