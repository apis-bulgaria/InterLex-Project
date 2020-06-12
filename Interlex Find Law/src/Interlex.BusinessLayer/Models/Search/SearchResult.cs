namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Data;
    using System.Threading;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Enums;
    using Interlex.DataLayer;
    using ApisLucene.Classes.Eucases.Search;
    using ApisLucene.Classes.Common;
    using DocToClassifier.LangFilter;
    using DocToClassifier.Classifier;
    using Newtonsoft.Json;
    using IvanAkcheurov.NTextCat.Lib;
    using System.Text;
    using System.Globalization;
    using ApisLucene.Classes.Common.SearchClasses;
    using SearchResultsGrouper;
    using System.Collections.ObjectModel;
    using ClassificationService;
    public class SearchResult
    {
        private string searchWrapperBasePath;
        private string basePath;

        public string SortBy { get; set; }
        public string SortDir { get; set; }

        private SearchSources _searchSource;
        public SearchSources SearchSource
        {
            get
            {
                return this._searchSource;
            }
        }

        //private int userId;

        private SearchTypes _searchType;
        public SearchTypes SearchType
        {
            get
            {
                return this._searchType;
            }
        }

        private int dbSearchId = -1;
        public int DbSearchId
        {
            get
            {
                return this.dbSearchId;
            }
        }

        public bool SearchCreated
        {
            get
            {
                return (this.dbSearchId != -1);
            }
        }
        private int _siteSearchId;
        public int SiteSearchId
        {
            get
            {
                return this._siteSearchId;
            }
        }

        // Statistic Id
        private int? _statSearchid = null;
        public int? StatSearchId
        {
            get
            {
                return this._statSearchid;
            }
        }

        public int PageSize { get; set; }
        private int visiblePagesCount;
        public int LanguageId;

        public EUCasesSearchWrapper SearchWrapper { get; set; }
        public FilterDocsStruct FilterDocsStructObj { get; set; }
        public DocToClassifierWrapper FilterDocsClassifiers { get; set; }
        public ResultsGrouper ResultsGroupperObj { get; set; }

        public Dictionary<string, string[]> ClassifiersMap { get; set; }

        public SearchBox SearchBoxFilters { get; set; }

        public int ProductId { get; set; } = 1; // Default - EuroCases

        public Dictionary<int, string> LocalSearchTexts { get; set; }

        public Dictionary<int, List<ClassifierItem>> ClassifierFilters { get; set; }
        public List<Guid> ClassifierFilterIds
        {
            get
            {
                List<Guid> l = new List<Guid>();
                foreach (var filter in this.ClassifierFilters.Values)
                    l.Add(filter.Last().Id);
                return l;
            }
        }

        /// <summary>
        /// Глобален rel sort.
        /// </summary>
        public bool HasRelSort
        {
            get
            {
                if (this.SearchSource == SearchSources.Search)
                    return true;

                if (this.SearchSource == SearchSources.HomePage)
                {
                    if (this.SearchBoxFilters.HasRelSort ||
                        this.ClassifierFilters.Where(x => x.Value.Where(y => y.Id.ToString() == "e729ee04-2fed-48bc-a6c9-10ebaa85d14b").Any()).Any())
                        return true;
                }

                return false;
            }
        }

        public List<Guid> ClassifierFiltersSystemIds
        {
            get
            {
                List<Guid> l = new List<Guid>();
                foreach (var filter in this.ClassifierFiltersSystem.Values)
                {
                    l.Add(filter);
                }
                return l;
            }
        }

        public Dictionary<int, Guid> ClassifierFiltersSystem { get; set; }

        public List<Guid> ClassifierFilterIdsAll
        {
            get
            {
                return ClassifierFilterIds.Concat(ClassifierFiltersSystemIds).Distinct().ToList();
            }
        }

        public List<PagerPage> Pages { get; set; }

        private int _currentPage;
        public int CurrentPage
        {
            get
            {
                return this._currentPage;
            }
            set
            {
                this._currentPage = value;
            }
        }

        private int _pagesCount;
        public int PagesCount
        {
            get
            {
                return this._pagesCount;
            }
        }

        private int _recordCount;
        public int ResultCount
        {
            get
            {
                return this._recordCount;
            }
            set
            {
                this._recordCount = value;
                this._pagesCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(this._recordCount) / Convert.ToDecimal(PageSize)));
            }
        }

        //public bool DisplayContext { get; set; }

        public SearchResult(SearchSources searchSource, object searchWrapper, string searchWrapperFolder, object filterDocsStruct, object filterDocsClassifiers, object classifiersMap, object resultsGroupper, string basePath, int langId, int pageSize, int visiblePagesCount)
        {
            this._searchSource = searchSource;
            this.basePath = basePath;
            this.searchWrapperBasePath = Path.Combine(basePath, searchWrapperFolder);
            this.PageSize = pageSize;
            this.visiblePagesCount = visiblePagesCount;
            this.LanguageId = langId;

            this.SearchWrapper = (EUCasesSearchWrapper)searchWrapper;
            this.FilterDocsStructObj = (FilterDocsStruct)filterDocsStruct;
            this.FilterDocsClassifiers = (DocToClassifierWrapper)filterDocsClassifiers;
            this.ClassifiersMap = (Dictionary<string, string[]>)classifiersMap;
            this.ResultsGroupperObj = (ResultsGrouper)resultsGroupper;

            SearchBoxFilters = new SearchBox(this.LanguageId);
            ClassifierFilters = new Dictionary<int, List<ClassifierItem>>();
            LocalSearchTexts = new Dictionary<int, string>();

            ClassifierFiltersSystem = new Dictionary<int, Guid>();

            // Initial values
            CurrentPage = 1;

            // Default sort
            this.SortBy = "rel";
            this.SortDir = "asc"; // asc
        }

        public void SearchFTQuery(string query, ref int[] swRes, int[] langPref)
        {
            // Validation
            if (this.SearchWrapper == null)
            {
                throw new Exception("Application SearchWrapper is null!");
            }

            if (this.FilterDocsStructObj == null)
            {
                throw new Exception("Application FilterDocsStruct is null!");
            }
            //------------------
            swRes = this.SearchWrapper.SearchQuery(query);

            // filter document result by prefered languages
            if (swRes.Length > 0)
            {
                FilterDocsStructObj.FilterArray(ref swRes, langPref);
                // Grouping of consolidated versions
                var groupedResult = this.ResultsGroupperObj.FilterResults(swRes);
                var groupedRes = groupedResult.ToDictionary(k => k.DocId, v => v.ConsolidatedActIds);
                swRes = groupedRes.Select(r => r.Key).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swRes"></param>
        /// <param name="langPref"></param>
        /// <returns>Indicates if search is made. It is important to know if search return zero result or no search is performed at all.</returns>
        public bool SearchFT(ref int[] swRes, int[] langPref, ref Dictionary<int, List<int>> groupedRes)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            bool searchParamsFilled = false;
            // Validation
            if (this.SearchWrapper == null)
            {
                throw new Exception("Application SearchWrapper is null!");
            }

            if (this.FilterDocsStructObj == null)
            {
                throw new Exception("Application FilterDocsStruct is null!");
            }

            if (this.FilterDocsClassifiers == null)
            {
                throw new Exception("Application FilterDocsClassifiers is null!");
            }
            //------------------

            var paramsp = new MainSearch();
            var financeParams = new FinanceSearch();

            string searchText = (String.IsNullOrWhiteSpace(this.SearchBoxFilters.SearchText)) ? "" : this.SearchBoxFilters.SearchText.Trim();

            if (searchText != "")
            {
                if ((this.SearchBoxFilters.Cases != null && this.SearchBoxFilters.Cases.TranslateSearchText) ||
                    (this.SearchBoxFilters.Law != null && this.SearchBoxFilters.Law.TranslateSearchText))
                {
                    string textLang = RecognizeTextLanguage(searchText, Path.Combine(this.basePath, "IvanAkcheurov.profile.xml"));
                    if (string.IsNullOrEmpty(textLang))
                    {
                        var lang = Languages.GetLang(this.LanguageId);
                        textLang = lang.ShortCode;
                    }
                    if (textLang == "bg" || textLang == "en" || textLang == "it")
                    {
                        string fromLang = "bg";
                        string toLang = "en";
                        if (textLang == "en")
                        {
                            fromLang = "en";
                            toLang = "bg";
                        }
                        else if (textLang == "it")
                        {
                            fromLang = "it";
                        }

                        // this now returns two languages like an or query
                        this.SearchBoxFilters.SearchTextMultiLingual = (new BanTranslateService()).Translate(searchText, fromLang);
                    }
                }
                paramsp.SearchText = searchText;
                financeParams.SearchText = searchText;
                if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchTextMultiLingual))
                    paramsp.SearchText = "(" + paramsp.SearchText + ") || " + this.SearchBoxFilters.SearchTextMultiLingual;
                searchParamsFilled = true;
            }


            if (this.SearchBoxFilters.HomeSearch != null && this.SearchBoxFilters.HomeSearch.QueryType == 1)
            {
                if (!this.SearchBoxFilters.ShowFreeDocuments.Value)
                {
                    this.SearchBoxFilters.HomeSearch.Query += "-classificators:(2837414225974C8BB4D338D7F729A7D9)"; // excluding free documents
                }
                else
                {
                    this.SearchBoxFilters.HomeSearch.Query = this.SearchBoxFilters.HomeSearch.Query.Replace("-classificators:(2837414225974C8BB4D338D7F729A7D9)", String.Empty);
                }

                swRes = this.SearchWrapper.SearchQuery(this.SearchBoxFilters.HomeSearch.Query);
                searchParamsFilled = true;
            }
            else
            {
                paramsp.ClassificatorsGroups = new List<List<string>>();

                // Usage: When classifier in document metadata is clicked a new search is created
                if (this.SearchBoxFilters.ClassifierFilter != null)
                {
                    paramsp.ClassificatorsGroups.Add(new List<string>() { this.SearchBoxFilters.ClassifierFilter.ToString() });
                    searchParamsFilled = true;

                    // XmlId Search context
                    if (this.SearchBoxFilters.ByXmlId == true)
                    {
                        if (this.SearchBoxFilters.Cases != null)
                        {
                            paramsp.DocType = DocumentTypeEnum.Judgment;

                            if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.National)
                            {
                                paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.National;
                            }
                            else if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.EU)
                            {
                                paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.EU;
                            }
                        }
                        else if (this.SearchBoxFilters.Law != null)
                        {
                            paramsp.DocType = DocumentTypeEnum.Act;

                            // European legislation; Add another check if national legislation is added
                            paramsp.DocumentNumber = "1* || 2* || 3* || 4*";
                        }
                    }
                }
                else
                {
                    if (this.SearchBoxFilters.Cases != null) // Search in CaseLaw tab
                    {
                        paramsp.DocType = DocumentTypeEnum.Judgment;

                        if (this.SearchBoxFilters.Cases.IsIndexEnactmentSearch == false) // True if Index enactment search - should work independently
                        {
                            if (this.SearchBoxFilters.Cases.OnlyInTitles == true)
                            {
                                paramsp.SearchOnlyTitles = true;
                            }
                            else
                            {
                                paramsp.SearchOnlyTitles = false;
                            }

                            if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.All)
                            {
                                paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.All;
                            }
                            else if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.EU)
                            {
                                paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.EU;

                                var shouldSendComputedDocNumber = false;
                                var computedDocNumber = "6";

                                if (this.SearchBoxFilters.Cases.Year != null && this.SearchBoxFilters.Cases.Year != "" && String.IsNullOrEmpty(this.SearchBoxFilters.Cases.Year) == false
                                    && this.SearchBoxFilters.Cases.Year.Length == 4)
                                {
                                    shouldSendComputedDocNumber = true;
                                    computedDocNumber = computedDocNumber + this.SearchBoxFilters.Cases.Year;
                                }
                                else
                                {
                                    computedDocNumber = computedDocNumber + "????";
                                }

                                //TODO: Compute two characters
                                computedDocNumber = computedDocNumber + "??";

                                if (this.SearchBoxFilters.Cases.CaseNumber != null && this.SearchBoxFilters.Cases.CaseNumber != "" && String.IsNullOrEmpty(this.SearchBoxFilters.Cases.CaseNumber) == false)
                                {
                                    shouldSendComputedDocNumber = true;

                                    var docNumberFromBox = this.SearchBoxFilters.Cases.CaseNumber;
                                    if (docNumberFromBox.Length == 1)
                                    {
                                        docNumberFromBox = "000" + docNumberFromBox;
                                    }
                                    else if (docNumberFromBox.Length == 2)
                                    {
                                        docNumberFromBox = "00" + docNumberFromBox;
                                    }
                                    else if (docNumberFromBox.Length == 3)
                                    {
                                        docNumberFromBox = "0" + docNumberFromBox;
                                    }

                                    computedDocNumber = computedDocNumber + docNumberFromBox;
                                }
                                else
                                {
                                    //TODO: Check if doc number is always 4 digited
                                    computedDocNumber = computedDocNumber + "????";
                                }

                                if (shouldSendComputedDocNumber)
                                {
                                    paramsp.SecDocNumber = computedDocNumber;
                                    searchParamsFilled = true;
                                }
                            }
                            else if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.National)
                            {
                                paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.National;
                            }
                            else if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.ECHR)
                            {
                                paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.ECHR;

                                if (!String.IsNullOrEmpty(this.SearchBoxFilters.Cases.Applicant))
                                {
                                    paramsp.Applicant = this.SearchBoxFilters.Cases.Applicant;
                                    searchParamsFilled = true;
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid CaseLawType");
                            }

                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.Cases.NatID_ECLI))
                            {
                                paramsp.NationalIdOrEcli = this.SearchBoxFilters.Cases.NatID_ECLI;
                                searchParamsFilled = true;
                            }

                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.Cases.Parties))
                            {
                                paramsp.Parties = this.SearchBoxFilters.Cases.Parties;
                                searchParamsFilled = true;
                            }

                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.Cases.ApplicationNumber))
                            {
                                paramsp.DocumentNumber = this.SearchBoxFilters.Cases.ApplicationNumber;
                                searchParamsFilled = true;
                            }

                            if (this.SearchBoxFilters.Cases.CourtsFolders.SelectedIds != "")
                            {
                                string[] ids = this.SearchBoxFilters.Cases.CourtsFolders.SelectedIds.Split(',');
                                List<string> classifierIds = new List<string>();
                                foreach (string id in ids)
                                {
                                    classifierIds.AddRange(DB.GetClassifierChildrenIds(Guid.Parse(id)));
                                }
                                paramsp.ClassificatorsGroups.Add(Classifiers.MapClassifiers(classifierIds, this.ClassifiersMap).Distinct().ToList());
                                searchParamsFilled = true;
                            }

                            if (this.SearchBoxFilters.Cases.DateFrom != null || this.SearchBoxFilters.Cases.DateTo != null)
                            {
                                if (this.SearchBoxFilters.Cases.DateFrom != null)
                                {
                                    if (this.SearchBoxFilters.Cases.DatePeriodType == DatePeriodType.Date) //Exact date search
                                    {
                                        paramsp.PeriodFrom = this.SearchBoxFilters.Cases.DateFrom;
                                        paramsp.PeriodTo = this.SearchBoxFilters.Cases.DateFrom.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                        searchParamsFilled = true;
                                    }
                                    else if (this.SearchBoxFilters.Cases.DatePeriodType == DatePeriodType.Period) //Period search
                                    {
                                        if (this.SearchBoxFilters.Cases.DateTo.HasValue)
                                        {
                                            this.SearchBoxFilters.Cases.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                        }
                                        paramsp.PeriodFrom = this.SearchBoxFilters.Cases.DateFrom;
                                        paramsp.PeriodTo = this.SearchBoxFilters.Cases.DateTo;
                                        searchParamsFilled = true;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid DatePeriodType for Cases");
                                    }
                                }
                                else
                                {
                                    if (this.SearchBoxFilters.Cases.DatePeriodType == DatePeriodType.Period) //Ensuring we are not in date search; preventing fictive DateTo values;
                                    {
                                        paramsp.PeriodTo = this.SearchBoxFilters.Cases.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                        searchParamsFilled = true;
                                    }
                                }
                            }

                            if (this.SearchBoxFilters.Cases.DocumentTypes.SelectedIds != "")
                            {
                                paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.DocumentTypes.SelectedIds.Split(',')));
                                searchParamsFilled = true;
                            }

                            //if (this.SearchBoxFilters.Cases.DirectoryCaseLaw.SelectedIds != "")
                            //{
                            //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.DirectoryCaseLaw.SelectedIds.Split(',')));
                            //    searchParamsFilled = true;
                            //}

                            //if (this.SearchBoxFilters.Cases.EuroCases.SelectedIds != "")
                            //{
                            //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.EuroCases.SelectedIds.Split(',')));
                            //    searchParamsFilled = true;
                            //}

                            //if (this.SearchBoxFilters.Cases.Eurovoc.SelectedIds != "")
                            //{
                            //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.Eurovoc.SelectedIds.Split(',')));
                            //    searchParamsFilled = true;
                            //}

                            if (this.SearchBoxFilters.Cases.Syllabus.SelectedIds != "")
                            {
                                paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.Syllabus.SelectedIds.Split(',')));
                                searchParamsFilled = true;
                            }

                            //if (this.SearchBoxFilters.Cases.SubjectMatter.SelectedIds != "")
                            //{
                            //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.SubjectMatter.SelectedIds.Split(',')));
                            //    searchParamsFilled = true;
                            //}

                            //if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.EU)
                            //{
                            //    if (this.SearchBoxFilters.Cases.ProcedureType.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.ProcedureType.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.AdvocateGeneral.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.AdvocateGeneral.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.JudgeRapporteur.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.JudgeRapporteur.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }
                            //}

                            //if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.ECHR)
                            //{
                            //    if (this.SearchBoxFilters.Cases.States.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.States.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.HudocImportance.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.HudocImportance.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.Hudoc.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.Hudoc.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.HudocApplicability.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.HudocApplicability.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.HudocArticleViolation.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.HudocArticleViolation.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.Courts.SelectedIds != "")
                            //    {
                            //        paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.Courts.SelectedIds.Split(',')));
                            //        searchParamsFilled = true;
                            //    }

                            //    if (this.SearchBoxFilters.Cases.ECHRReferedActType == ECHRReferedActType.ConventionAndProtocols)
                            //    {
                            //        if (this.SearchBoxFilters.Cases.HudocArticles.SelectedIds != "")
                            //        {
                            //            paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.HudocArticles.SelectedIds.Split(',')));

                            //        }
                            //        else
                            //        {
                            //            paramsp.ClassificatorsGroups.Add(new List<string> { "8da600c2-52f2-48aa-b9a4-57d6936f4e1f" }); // HUDOC Articles fictive base classifier
                            //        }

                            //        searchParamsFilled = true;
                            //    }
                            //    else if (this.SearchBoxFilters.Cases.ECHRReferedActType == ECHRReferedActType.RulesOfTheCourt)
                            //    {
                            //        if (this.SearchBoxFilters.Cases.RulesOfTheCourt.SelectedIds != "")
                            //        {
                            //            paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Cases.RulesOfTheCourt.SelectedIds.Split(',')));
                            //        }
                            //        else
                            //        {
                            //            paramsp.ClassificatorsGroups.Add(new List<string> { "1abda806-23b9-41e7-b64a-793b6a402d4d" }); // Rules of the court fictive base classifier
                            //        }

                            //        searchParamsFilled = true;
                            //    }
                            //}

                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.Cases.MultiDict.Text))
                            {
                                var logicalTypeExpr = this.SearchBoxFilters.Cases.MultiDict.LogicalType ? "[AND]" : "[OR]";
                                if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText))
                                {
                                    paramsp.SearchText = paramsp.SearchText + " [AND] (";
                                }

                                paramsp.SearchText = paramsp.SearchText +
                                    this.SearchBoxFilters.Cases.MultiDict.Text.Replace("OR", logicalTypeExpr).Replace("AND", logicalTypeExpr);

                                if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText))
                                {
                                    paramsp.SearchText = paramsp.SearchText + ")";
                                }

                                searchParamsFilled = true;
                            }
                        }
                        else
                        {
                            this.SearchBoxFilters.SearchText = String.Empty;
                            paramsp.SearchText = null;
                            searchParamsFilled = false;
                        }

                        this._searchType = SearchTypes.CaseLaw;
                    }

                    else if (this.SearchBoxFilters.Law != null) // Legislation tab
                    {
                        paramsp.DocType = DocumentTypeEnum.Act;

                        if (this.SearchBoxFilters.Law.OnlyInTitles == true)
                        {
                            paramsp.SearchOnlyTitles = true;
                        }
                        else
                        {
                            paramsp.SearchOnlyTitles = false;
                        }

                        if (this.SearchBoxFilters.Law.OnlyInActualActs == true)
                        {
                            paramsp.OnlyInActualActs = true;
                        }
                        else
                        {
                            paramsp.OnlyInActualActs = false;
                        }

                        if (this.SearchBoxFilters.Law.OnlyInBasicActs == true)
                        {
                            paramsp.OnlyInBaseActs = true;
                        }
                        else
                        {
                            paramsp.OnlyInBaseActs = false;
                        }

                        var shouldSendComputedDocNumber = false;
                        var computedDocNumber = "1";

                        if (this.SearchBoxFilters.Law.Year != null && this.SearchBoxFilters.Law.Year != "" && String.IsNullOrEmpty(this.SearchBoxFilters.Law.Year) == false
                            && this.SearchBoxFilters.Law.Year.Length == 4)
                        {
                            shouldSendComputedDocNumber = true;
                            computedDocNumber = computedDocNumber + this.SearchBoxFilters.Law.Year;
                        }
                        else
                        {
                            computedDocNumber = computedDocNumber + "????";
                        }

                        //TODO: Compute two characters
                        computedDocNumber = computedDocNumber + "?";

                        if (this.SearchBoxFilters.Law.DocNumber != null && this.SearchBoxFilters.Law.DocNumber != "" && String.IsNullOrEmpty(this.SearchBoxFilters.Law.DocNumber) == false)
                        {
                            shouldSendComputedDocNumber = true;
                            var docNumberFromBox = this.SearchBoxFilters.Law.DocNumber;
                            if (docNumberFromBox.Length == 1)
                            {
                                docNumberFromBox = "000" + docNumberFromBox;
                            }
                            else if (docNumberFromBox.Length == 2)
                            {
                                docNumberFromBox = "00" + docNumberFromBox;
                            }
                            else if (docNumberFromBox.Length == 3)
                            {
                                docNumberFromBox = "0" + docNumberFromBox;
                            }

                            computedDocNumber = computedDocNumber + docNumberFromBox;
                        }
                        else
                        {
                            //TODO: Check if doc number is always 4 digited
                            computedDocNumber = computedDocNumber + "????";
                        }

                        if (shouldSendComputedDocNumber)
                        {
                            var firstComputedNumber = computedDocNumber;
                            var secondComputedBuilder = new StringBuilder(computedDocNumber);
                            secondComputedBuilder[0] = '2';
                            var secondComputedNumber = secondComputedBuilder.ToString();
                            var thirdComputedBuilder = new StringBuilder(computedDocNumber);
                            thirdComputedBuilder[0] = '3';
                            var thirdComputedNumber = thirdComputedBuilder.ToString();
                            var forthComputedBuilder = new StringBuilder(computedDocNumber);
                            forthComputedBuilder[0] = '4';
                            var forthComputedNumber = forthComputedBuilder.ToString();

                            var realComputedDocNumber = firstComputedNumber + " [OR] " + secondComputedNumber + " [OR] " + thirdComputedNumber + " [OR] " + forthComputedNumber;

                            paramsp.SecDocNumber = realComputedDocNumber;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.NatID_ELI != "" && this.SearchBoxFilters.Law.NatID_ELI != null)
                        {
                            paramsp.NationalIdOrEcli = this.SearchBoxFilters.Law.NatID_ELI;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.DateFrom != null || this.SearchBoxFilters.Law.DateTo != null)
                        {
                            if (this.SearchBoxFilters.Law.DateFrom != null)
                            {
                                if (this.SearchBoxFilters.Law.DatePeriodType == DatePeriodType.Date) //Exact date search
                                {
                                    paramsp.PeriodFrom = this.SearchBoxFilters.Law.DateFrom;
                                    paramsp.PeriodTo = this.SearchBoxFilters.Law.DateFrom.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                    searchParamsFilled = true;
                                }
                                else if (this.SearchBoxFilters.Law.DatePeriodType == DatePeriodType.Period) //Period search
                                {
                                    paramsp.PeriodFrom = this.SearchBoxFilters.Law.DateFrom;
                                    if (this.SearchBoxFilters.Law.DateTo.HasValue)
                                    {
                                        this.SearchBoxFilters.Law.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                    }
                                    paramsp.PeriodTo = this.SearchBoxFilters.Law.DateTo;
                                    searchParamsFilled = true;
                                }
                                else
                                {
                                    throw new ArgumentException("Invalid DatePeriodType for Law");
                                }
                            }
                            else
                            {
                                if (this.SearchBoxFilters.Law.DatePeriodType == DatePeriodType.Period) //Ensuring we are not in date search; preventing fictive DateTo values;
                                {
                                    paramsp.PeriodTo = this.SearchBoxFilters.Cases.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                    searchParamsFilled = true;
                                }
                            }

                            paramsp.LawDate = this.SearchBoxFilters.Law.LawDateType;
                        }

                        if (this.SearchBoxFilters.Law.OJSeries != OJTypeEnum.DEFAULT)
                        {
                            paramsp.OJType = this.SearchBoxFilters.Law.OJSeries;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.OJYear != null)
                        {
                            var yearAsInt = int.Parse(this.SearchBoxFilters.Law.OJYear);
                            var fictiveDateTime = new DateTime(yearAsInt, 1, 1);
                            paramsp.OJYear = fictiveDateTime;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.Number != null)
                        {
                            paramsp.Number = this.SearchBoxFilters.Law.Number;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.PageNumber != null)
                        {
                            paramsp.PageNumber = this.SearchBoxFilters.Law.PageNumber;
                            searchParamsFilled = true;
                        }

                        //if (this.SearchBoxFilters.Law.DirectoryLegislation.SelectedIds != "")
                        //{
                        //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.DirectoryLegislation.SelectedIds.Split(',')));
                        //    searchParamsFilled = true;
                        //}

                        if (this.SearchBoxFilters.Law.DocumentTypes.SelectedIds != "")
                        {
                            paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.DocumentTypes.SelectedIds.Split(',')));
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.Jurisdictions.SelectedIds != "")
                        {
                            paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.Jurisdictions.SelectedIds.Split(',')));
                            searchParamsFilled = true;
                        }

                        //if (this.SearchBoxFilters.Law.Eurovoc.SelectedIds != "")
                        //{
                        //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.Eurovoc.SelectedIds.Split(',')));
                        //    searchParamsFilled = true;
                        //}

                        if (this.SearchBoxFilters.Law.Syllabus.SelectedIds != "")
                        {
                            paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.Syllabus.SelectedIds.Split(',')));
                            searchParamsFilled = true;
                        }

                        //if (this.SearchBoxFilters.Law.SubjectMatter.SelectedIds != "")
                        //{
                        //    paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.SubjectMatter.SelectedIds.Split(',')));
                        //    searchParamsFilled = true;
                        //}

                        if (this.SearchBoxFilters.Law.ActJurisdictions.SelectedIds != "")
                        {
                            paramsp.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Law.ActJurisdictions.SelectedIds.Split(',')));
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Law.MultiDict != null && !String.IsNullOrEmpty(this.SearchBoxFilters.Law.MultiDict.Text))
                        {
                            var logicalTypeExpr = this.SearchBoxFilters.Law.MultiDict.LogicalType ? "[AND]" : "[OR]";
                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText))
                            {
                                paramsp.SearchText = paramsp.SearchText + " [AND] (";
                            }

                            paramsp.SearchText = paramsp.SearchText +
                                this.SearchBoxFilters.Law.MultiDict.Text.Replace("OR", logicalTypeExpr).Replace("AND", logicalTypeExpr);

                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText))
                            {
                                paramsp.SearchText = paramsp.SearchText + ")";
                            }

                            searchParamsFilled = true;
                        }

                        if (searchParamsFilled) // adding legislation type
                        {
                            if (this.SearchBoxFilters.Law.LegislationType == LegislationType.EU)
                            {
                                //paramsp.ClassificatorsGroups.Add(new List<string> { "af88ca51-7522-455a-aefe-ec0d3c2d6a37" }); // EU legislation
                            }
                            else
                            {
                                // paramsp.ClassificatorsGroups.Add(new List<string> { "987e4eef-3e55-43be-9052-bb98ef1dfd83" }); // National legislation
                            }
                        }

                        this._searchType = SearchTypes.Legislation;
                    }
                    else if (this.SearchBoxFilters.Finances != null)
                    {
                        // Finances checkboxes
                        if (this.SearchBoxFilters.Finances.OnlyInTitles == true)
                        {
                            financeParams.SearchOnlyTitles = true;
                        }
                        else
                        {
                            financeParams.SearchOnlyTitles = false;
                        }

                        if (this.SearchBoxFilters.Finances.Keywords == true)
                        {
                            financeParams.OnlyInKeywords = true;
                        }
                        else
                        {
                            financeParams.OnlyInKeywords = false;
                        }

                        if (this.SearchBoxFilters.Finances.SearchInSummaries == true)
                        {
                            financeParams.IsSummarySearch = true;
                        }

                        // Finances doc type checkboxes
                        if (this.SearchBoxFilters.Finances.DocTypeDirectives == true)
                        {
                            financeParams.IsDirectives = this.SearchBoxFilters.Finances.DocTypeDirectives;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Finances.DocTypeStandarts == true)
                        {
                            financeParams.IsFinanceStandards = this.SearchBoxFilters.Finances.DocTypeStandarts;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Finances.DocTypeReglaments == true)
                        {
                            financeParams.IsReglaments = this.SearchBoxFilters.Finances.DocTypeReglaments;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Finances.DocTypeEuCaseLaw == true)
                        {
                            financeParams.IsEuDoc = this.SearchBoxFilters.Finances.DocTypeEuCaseLaw;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Finances.DocTypeNationalCaseLaw == true)
                        {
                            financeParams.IsNationalCaseLaw = this.SearchBoxFilters.Finances.DocTypeNationalCaseLaw;
                            searchParamsFilled = true;
                        }

                        if (this.SearchBoxFilters.Finances.DocTypeSummaries == true)
                        {
                            financeParams.IsSummary = this.SearchBoxFilters.Finances.DocTypeSummaries;
                            searchParamsFilled = true;
                        }

                        // My searches deserializer fix
                        if (this.SearchBoxFilters.Finances.DateFrom == null)
                        {
                            this.SearchBoxFilters.Finances.DateFrom = default(DateTime);
                        }

                        // Finances dates
                        if (this.SearchBoxFilters.Finances.DateFrom != null || this.SearchBoxFilters.Finances.DateTo != null)
                        {
                            if (this.SearchBoxFilters.Finances.DateFrom != null)
                            {
                                if (this.SearchBoxFilters.Finances.DatePeriodType == DatePeriodType.Date) //Exact date search
                                {
                                    financeParams.PeriodFrom = this.SearchBoxFilters.Finances.DateFrom;
                                    financeParams.PeriodTo = this.SearchBoxFilters.Finances.DateFrom.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                    searchParamsFilled = true;
                                }
                                else if (this.SearchBoxFilters.Finances.DatePeriodType == DatePeriodType.Period) //Period search
                                {
                                    financeParams.PeriodFrom = this.SearchBoxFilters.Finances.DateFrom;
                                    if (this.SearchBoxFilters.Finances.DateTo.HasValue)
                                    {
                                        this.SearchBoxFilters.Finances.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                    }
                                    financeParams.PeriodTo = this.SearchBoxFilters.Finances.DateTo;
                                    searchParamsFilled = true;
                                }
                                else
                                {
                                    throw new ArgumentException("Invalid DatePeriodType for Finances");
                                }
                            }
                            else
                            {
                                if (this.SearchBoxFilters.Finances.DatePeriodType == DatePeriodType.Period) //Ensuring we are not in date search; preventing fictive DateTo values;
                                {
                                    financeParams.PeriodTo = this.SearchBoxFilters.Finances.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                                    searchParamsFilled = true;
                                }
                            }
                        }

                        if (this.SearchBoxFilters.Finances.EuroFinance.SelectedIds != "")
                        {
                            financeParams.ClassificatorsGroups.Add(new List<string>(this.SearchBoxFilters.Finances.EuroFinance.SelectedIds.Split(',')));
                            searchParamsFilled = true;
                        }

                        this._searchType = SearchTypes.Finances;
                    }
                    else
                    {
                        this._searchType = SearchTypes.Simple;

                        if (this.SearchBoxFilters.MultiDict != null && !String.IsNullOrEmpty(this.SearchBoxFilters.MultiDict.Text))
                        {
                            var logicalTypeExpr = this.SearchBoxFilters.MultiDict.LogicalType ? "[AND]" : "[OR]";
                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText))
                            {
                                paramsp.SearchText = paramsp.SearchText + " [AND] (";
                            }

                            paramsp.SearchText = paramsp.SearchText +
                                this.SearchBoxFilters.MultiDict.Text.Replace("OR", logicalTypeExpr).Replace("AND", logicalTypeExpr);

                            if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText))
                            {
                                paramsp.SearchText = paramsp.SearchText + ")";
                            }

                            searchParamsFilled = true;
                        }
                    }
                }

                //consider removing this duplicate
                if (!String.IsNullOrEmpty(this.SearchBoxFilters.SearchText) && this.SearchBoxFilters.SearchText.Contains("*"))
                {
                    this.SearchBoxFilters.ExactMatch = true;
                }

                if (searchParamsFilled)
                {
                    // populating system fictive classifiers
                    this.PopulateSystemClassifiers();

                    if (this.ProductId == 1 && (this.SearchBoxFilters.Law != null || this.SearchBoxFilters.Cases != null))
                    {
                        foreach (var systemClassifier in this.ClassifierFiltersSystemIds)
                        {
                            var curList = new List<string>();
                            curList.Add(systemClassifier.ToString());
                            paramsp.ClassificatorsGroups.Add(curList);
                        }
                    }

                    if (!this.SearchBoxFilters.ShowFreeDocuments.Value)
                    {
                        // paramsp.ClassificatorsNOTGroups.Add(new List<string> { "26080a3b-acba-4080-9364-69a6bef93d0b" });
                        paramsp.ClassificatorsNOTGroups.Add(new List<string> { "28374142-2597-4C8B-B4D3-38D7F729A7D9" });
                    }


                    // ensuring loaded fictive datetime values from a previously done search are not passed
                    if (paramsp.PeriodFrom == default(DateTime))
                    {
                        paramsp.PeriodFrom = null;
                        paramsp.PeriodTo = null;
                    }

                    if (financeParams.PeriodFrom == default(DateTime))
                    {
                        financeParams.PeriodFrom = null;
                        financeParams.PeriodTo = null;
                    }

                    try
                    {
                        if (this.ProductId == 2) // FinanceSearch
                        {
                            swRes = this.SearchWrapper.SearchDictQueryList(financeParams.GetQueries(), !this.SearchBoxFilters.ExactMatch);
                        }
                        else
                        { // EuroCasesSearch
                            //   var dic = paramsp.GetParams(); // no reflection anymore ;[ :D Koce is gone ;(((
                            var queries = paramsp.GetQueries();
                            swRes = this.SearchWrapper.SearchDictQueryList(paramsp.GetQueries(), !this.SearchBoxFilters.ExactMatch);
                        }
                    }
                    catch (Exception)
                    {
                        swRes = new int[0];
                    }
                }
                else
                {
                    swRes = new int[0];
                }
            }

            // filter document result by prefered languages
            if (swRes != null && swRes.Length > 0)
            {
                FilterDocsStructObj.FilterArray(ref swRes, langPref);
            }

            // group result with related documents
            var groupedResult = this.ResultsGroupperObj.FilterResults(swRes);
            groupedRes = groupedResult.ToDictionary(k => k.DocId, v => v.ConsolidatedActIds);
            swRes = groupedRes.Select(r => r.Key).ToArray();

            return searchParamsFilled;
        }

        public void ChangeLanguage(int langId, UserData ud)
        {
            Search.DelSearch(this.DbSearchId);
            this.dbSearchId = -1;
            this.LanguageId = langId;

            this.CreateSearch(ud.UserId, ud.SessionId);
        }

        public void CreateSearch(int userId, int sessionId)
        {
            // Performance log
            Stopwatch stopwatch = null;
            string perf = DateTime.Now.ToString() + ", SearchText='" + this.SearchBoxFilters.SearchText + "':" + Environment.NewLine;

            // Search is already created
            if (this.dbSearchId > 0)
                return;

            int totalCount = 0;

            bool searchPerformed = false;

            int[] swRes = null;

            Dictionary<int, List<int>> groupedRes = null;

            int[] langPref = DB.GetUserLangPrefForSearch(userId, this.LanguageId);

            if (this.SearchSource == SearchSources.InLinks)
            {
                if (this.SearchBoxFilters.DocInLinks.IsOriginApi == true)
                {
                    swRes = Doc.GetDocInLinks(this.SearchBoxFilters.DocInLinks.ToDocNumber, this.SearchBoxFilters.DocInLinks.ToParOriginal, this.SearchBoxFilters.DocInLinks.Domain, this.SearchBoxFilters.DocInLinks.UserRequestingId, this.SearchBoxFilters.DocInLinks.SiteLangRequestingId, int.MaxValue);
                }
                else
                {
                    string subCharSimple = null;
                    if (!String.IsNullOrEmpty(this.SearchBoxFilters.DocInLinks.SubTitle))
                    {
                        subCharSimple = this.SearchBoxFilters.DocInLinks.SubTitle.Split(' ')[1];
                    }

                    if (!String.IsNullOrEmpty(this.SearchBoxFilters.DocInLinks.LinkIdsString))
                    {
                        swRes = Doc.GetDocInLinks(this.SearchBoxFilters.DocInLinks.ToDocLangId, this.SearchBoxFilters.DocInLinks.ToParId, this.ProductId, this.SearchBoxFilters.DocInLinks.LinkIdsString.Split('-').Select(e => int.Parse(e)).ToArray(), subCharSimple, this.SearchBoxFilters.ShowFreeDocuments.Value);
                    }
                    else
                    {
                        swRes = Doc.GetDocInLinks(this.SearchBoxFilters.DocInLinks.ToDocLangId, this.SearchBoxFilters.DocInLinks.ToParId, this.ProductId, null, subCharSimple, this.SearchBoxFilters.ShowFreeDocuments.Value); //consider empty collection
                    }
                }

                FilterDocsStructObj.FilterArray(ref swRes, langPref);
                groupedRes = swRes.ToDictionary(x => x, x => new List<int>());
            }
            else if (this.SearchSource == SearchSources.HomePage && this.SearchBoxFilters.HomeSearch.QueryType == 2) // sql query
            {
                // TODO
                // search by sql query in db
            }
            else
            {
                stopwatch = Stopwatch.StartNew();
                searchPerformed = SearchFT(ref swRes, langPref, ref groupedRes);
                stopwatch.Stop();

                perf += "\tFT: " + stopwatch.ElapsedMilliseconds.ToString() + "ms" + Environment.NewLine;
            }

            string searchText = (String.IsNullOrWhiteSpace(this.SearchBoxFilters.SearchText)) ? "" : this.SearchBoxFilters.SearchText.Trim();
            // save to stat_searches
            if (!String.IsNullOrEmpty(searchText))
            {
                this._statSearchid = Stat.SetStatSearch(searchText, this.ProductId);
            }

            byte[] searchIds = new byte[swRes.Length * sizeof(int)];
            Buffer.BlockCopy(swRes, 0, searchIds, 0, searchIds.Length);

            int? docLangId = null;
            int? toDocParId = null;
            int[] linkId = new int[0];
            if (this.SearchSource == SearchSources.Search && this.SearchBoxFilters.Cases != null)
            {
                if (this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.ECHR && this.SearchBoxFilters.Cases.ReferedActECHRDocLangId.HasValue)
                {
                    docLangId = this.SearchBoxFilters.Cases.ReferedActECHRDocLangId.Value;
                }
                else if (this.SearchBoxFilters.Cases.EnactmentDocLangId.HasValue)
                {
                    docLangId = this.SearchBoxFilters.Cases.EnactmentDocLangId.Value;
                    toDocParId = null;
                    //linkId = null;
                    if (!String.IsNullOrEmpty(this.SearchBoxFilters.Cases.ProvisionId))
                    {
                        string[] arr = this.SearchBoxFilters.Cases.ProvisionId.Split(':'); // format <doc_par_id>:<link_id?>
                        toDocParId = Convert.ToInt32(arr[0]);
                        if (arr.Length == 2)
                        {
                            linkId = arr[1].Split('|').Select(x => Int32.Parse(x.Trim())).ToArray();
                            //linkId = Convert.ToInt32(arr[1]);
                        }
                    }
                }
            }
            else if (this.SearchSource == SearchSources.Search && this.SearchBoxFilters.Finances != null) // Finances search
            {
                if (this.SearchBoxFilters.Finances.EnactmentDocLangId.HasValue)
                {
                    docLangId = this.SearchBoxFilters.Finances.EnactmentDocLangId.Value;
                    toDocParId = null;
                    //linkId = null;
                    if (!String.IsNullOrEmpty(this.SearchBoxFilters.Finances.ProvisionId))
                    {
                        string[] arr = this.SearchBoxFilters.Finances.ProvisionId.Split(':'); // format <doc_par_id>:<link_id?>
                        toDocParId = Convert.ToInt32(arr[0]);
                        if (arr.Length == 2)
                        {
                            linkId = arr[1].Split('|').Select(x => Int32.Parse(x.Trim())).ToArray();
                            //linkId = Convert.ToInt32(arr[1]);
                        }
                    }
                }
            }

            SearchBox sb = this.SearchBoxFilters;

            string searchBoxFiltersJSON = JsonConvert.SerializeObject(Common.FixSearchBoxFiltersDatesForJSON(sb), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });

            this.dbSearchId = DB.AddSearch(userId, sessionId, this._siteSearchId, searchBoxFiltersJSON, ref groupedRes, this.LanguageId, searchIds, searchPerformed, docLangId, toDocParId, linkId, out totalCount);

            this.ResultCount = totalCount;

            return;
        }

        private string RecognizeTextLanguage(string text, string path)
        {
            var languageRecognitionProvider = (new RankedLanguageIdentifierFactory()).Load(path);
            var identifyResult = languageRecognitionProvider.Identify(text);

            var mostCertainLanguage = identifyResult.FirstOrDefault();

            if (mostCertainLanguage != null)
            {
                var language_3_letter = mostCertainLanguage.Item1;

                switch (language_3_letter.Iso639_2T.ToLower())
                {
                    case "bul":
                    case "rus": return "bg";
                    case "eng": return "en";
                    case "fra": return "fr";
                    case "deu": return "de";
                    case "ita": return "it";
                }
            }
            return "";
        }

        public List<SearchListItem> GetList(UserData ud, int langId)
        {
            if (dbSearchId == -1)
                throw new Exception("GetList(): Не е създадено търсене. dbSearchId = -1");

            if (this.LanguageId != langId)
            {
                this.ChangeLanguage(langId, ud);
            }

            List<SearchListItem> SearchList = new List<SearchListItem>();

            foreach (var r in DB.GetSearchList(this.dbSearchId, (int)this.SearchSource, ClassifierFilterIdsAll.ToArray(), this.SortBy, this.SortDir, this.CurrentPage, this.PageSize, ud.UserId, this.LanguageId))
            {
                SearchListItem item = new SearchListItem();

                Document docInfo = new Document();
                docInfo.DocLangId = int.Parse(r["doc_lang_id"].ToString());
                docInfo.Country = r["country"].ToString();
                docInfo.DocType = Convert.ToInt32(r["doc_type_id"]);
                docInfo.DocDate = Convert.ToDateTime(r["doc_date"]);
                if (r["date_of_effect"] != DBNull.Value)
                    docInfo.DateOfEffect = Convert.ToDateTime(r["date_of_effect"]);
                if (r["end_date"] != DBNull.Value)
                    docInfo.EndDate = Convert.ToDateTime(r["end_date"]);
                docInfo.DocNumber = r["doc_number"].ToString();
                docInfo.LastConsDocNumber = r["last_cons_doc_number"].ToString();
                docInfo.Title = r["title"].ToString();
                docInfo.LangId = Convert.ToInt32(r["lang_id"]);
                docInfo.Publisher = r["publisher"].ToString();
                docInfo.AdditionalInfo = (AdditionalInfoEnum)r["additional_info"];

                // Keywords
                Type keywordsT = r["keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    docInfo.SetKeywords((string[,])r["keywords"], langId);

                // Summaries
                Type summariesT = r["summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    docInfo.SetSummaries((string[,])r["summaries"], langId);

                docInfo.UserDocId = r["user_doc_id"].ToString();

                Type relatedDocsT = r["related_docs"].GetType();
                if (relatedDocsT.IsArray && relatedDocsT.IsAssignableFrom(typeof(string[,])))
                {
                    docInfo.SetRelatedDocNumbers((String[,])r["related_docs"]);
                }

                item.DocumentInfo = docInfo;
                item.LastOpenDate = DateTime.Now;
                item.Date = DateTime.Now;

                SearchList.Add(item);
            }

            return SearchList;
        }

        #region Filters

        public void AddFilterClassifier(Guid id, int classifierTypeId, string name, int langId)
        {
            var classifiersToBeAdded = new List<ClassifierItem>();

            //if (classifierTypeId == 7)
            //{
            //    var originalClassifiers = ClassificationService.ClassificatorService.Current.MappingService.TryGetOriginalsFromMapping(id.ToString());
            //    if (originalClassifiers != null && originalClassifiers.Count > 0)
            //    {
            //        foreach (var originalClassifier in originalClassifiers)
            //        {
            //            var curModelItem = new ClassifierItem();
            //            curModelItem.Id = new Guid(originalClassifier.Guid);
            //            curModelItem.Type = ClassifierTypes.Courts;
            //            curModelItem.Name = originalClassifier.LanguageVariantsWithHints[langId.ToString()].Title; //testing

            //            classifiersToBeAdded.Add(curModelItem);
            //        }
            //    }
            //}
            //else
            {
                ClassifierItem item = new ClassifierItem();
                item.Id = id;
                item.Type = (ClassifierTypes)classifierTypeId;
                item.Name = name;

                classifiersToBeAdded.Add(item);
            }

            foreach (var classifier in classifiersToBeAdded)
            {
                if (classifier.Id.ToString() == "e729ee04-2fed-48bc-a6c9-10ebaa85d14b")
                {
                    this.SortBy = "rel";
                    this.SortDir = "asc";
                }
                this.AddClassifierItem(classifier, (int)classifier.Type);
            }

            this.CurrentPage = 1; // Goes to first page when filter
            this.ResultCount = DB.GetSearchListCount(this.dbSearchId, ClassifierFilterIdsAll.ToArray());
        }

        private void AddClassifierItem(ClassifierItem item, int classifierTypeId)
        {
            if (this.ClassifierFilters.ContainsKey(classifierTypeId))
            {
                if (!this.ClassifierFilters[classifierTypeId].Contains(item))
                {
                    this.ClassifierFilters[classifierTypeId].Add(item);
                }
            }
            else
            {
                this.ClassifierFilters.Add(classifierTypeId, new List<ClassifierItem>() { item });
            }
        }

        public void RemoveFilterClassifier(Guid id, int classifierTypeId)
        {
            if (this.ClassifierFilters.ContainsKey(classifierTypeId))
            {
                if (this.ClassifierFilters[classifierTypeId].Contains(new ClassifierItem() { Id = id }))
                {
                    int itemIndex = -1;
                    for (int i = 0; i < this.ClassifierFilters[classifierTypeId].Count; i++)
                    {
                        ClassifierItem item = this.ClassifierFilters[classifierTypeId][i];
                        if (item.Id == id)
                        {
                            itemIndex = i;
                            break;
                        }
                    }

                    this.ClassifierFilters[classifierTypeId].RemoveRange(itemIndex, this.ClassifierFilters[classifierTypeId].Count - itemIndex);
                    // check and remove key if empty
                    if (this.ClassifierFilters[classifierTypeId].Count == 0)
                        this.ClassifierFilters.Remove(classifierTypeId);

                    this.ResultCount = DB.GetSearchListCount(this.dbSearchId, ClassifierFilterIdsAll.ToArray());
                }
            }
        }

        public void RemoveAllFilterClassifiers()
        {
            this.ClassifierFilters.Clear();
            this.ResultCount = DB.GetSearchListCount(this.dbSearchId, ClassifierFilterIdsAll.ToArray());
        }

        public static Dictionary<int, string> FilterTitleResKeys = new Dictionary<int, string>() {
            {8, "UI_DocumentType" },
            /*  {6, "UI_Jurisdiction" }, */
            {7, "UI_Jurisdiction" },
            {1021, "UI_Cases_States" },
            {1020, "UI_Cases_HudocImportance" },
            {1012, "UI_Cases_Hudoc" },
            {1024, "UI_Filter_Summaries" },
            {3, "UI_DirectoryLegislation" },
            {2, "UI_DirectoryCaseLaw" },
            {5, "UI_Subjectmatter" },
            {1, "UI_Eurovoc" },
            {4, "UI_EuroCases" },
            {1022, "UI_Countries" },
            {3034, "UI_ExpertMaterials" },
            { 3031, "UI_Test" }
        };

        public List<int> GetAvailableFilterTypes()
        {
            List<int> filters = new List<int>();

            if (this.SearchSource == SearchSources.Search)
            {
                switch (this.SearchType)
                {
                    case SearchTypes.Simple:
                        filters.AddRange(new int[] { 3031, 8, 7, 1024, 3, 2, 4, 5, 1, 1022, 3034 });
                        break;
                    case SearchTypes.Legislation:
                        filters.AddRange(new int[] { 3031, 8, 3, 5, 1, 1022, 3034 });
                        break;
                    case SearchTypes.CaseLaw:
                        switch (this.SearchBoxFilters.Cases.CaseLawType)
                        {
                            case Enums.CaseLawType.All:
                                filters.AddRange(new int[] { 3031, 8, 7, 1024, 2, 4, 5, 1 });
                                break;
                            case Enums.CaseLawType.EU:
                                filters.AddRange(new int[] { 3031, 8, 7, 1024, 2, 4, 5, 1 });
                                break;
                            case Enums.CaseLawType.ECHR:
                                filters.AddRange(new int[] { 3031, 8, 7, 1024, 2, 1021, 1020, 1012, 4 });
                                break;
                            case Enums.CaseLawType.National:
                                filters.AddRange(new int[] { 3031, 7, 1024, 2, 4, 5, 1 });
                                break;
                        }
                        break;
                }
            }
            else
            {
                if (this.SearchSource == SearchSources.HomePage)
                {
                    filters.AddRange(this.SearchBoxFilters.HomeSearch.Filters.Select(f => f.Key).ToList());
                }

                if (filters.Count == 0)
                {
                    filters.AddRange(new int[] { 3031, 8, 7, 1024, 3, 2, 4, 5, 1, 1021, 1020, 1012, 1022, 3034 });
                }
            }

            return filters;
        }

        //public List<ClassifierItem> GetFilterClassifierTypes()
        //{
        //    if (dbSearchId == -1)
        //        throw new Exception("GetFilterClassifierTypes(): Не е създадено търсене. dbSearchId = -1");

        //    // pass only last level parents to db
        //    List<Tuple<int, string>> parents = new List<Tuple<int, string>>();

        //    foreach (int key in ClassifierFilters.Keys)
        //    {
        //        parents.Add(new Tuple<int, string>(key, ClassifierFilters[key].Last().Id.ToString()));
        //    }
        //    return Search.GetFilterClassifierTypes(dbSearchId, parents);
        //}

        //private List<string> GetSelectedClassifiers()
        //{
        //    List<string> l = new List<string>();

        //    foreach (int key in ClassifierFilters.Keys)
        //    {
        //        l.Add(ClassifierFilters[key].Select(c => c.Id.ToString()).Last());
        //        //l.AddRange(ClassifierFilters[key].Select(c => c.Id.ToString()).ToList());
        //        //parents.Add(new Tuple<int, string>(key, ClassifierFilters[key].Last().Id.ToString()));
        //    }

        //    return l;
        //}

        private List<string> GetAllClassifiers()
        {
            List<string> l = new List<string>();

            var allDic = new Dictionary<int, List<Guid>>();

            // Home classifiers for home folders search
            if (this.SearchBoxFilters.HomeSearch != null && this.SearchBoxFilters.HomeSearch.QueryType == 1)
            {
                foreach (var item in this.SearchBoxFilters.HomeSearch.Filters)
                {
                    if (String.IsNullOrEmpty(item.Value))
                    {
                        continue;
                    }

                    if (item.Key == 7 && this.ClassifierFilters.ContainsKey(6))
                    {
                        continue;
                    }

                    var curList = new List<Guid>();
                    if (item.Value == "-1") // we do not want filters with -1 to appear so adding a fictive guide will do the trick,
                                            // not a random one though cause nothing is really random except woman logic :)
                    {
                        curList.Add(default(Guid));
                    }
                    else
                    {
                        curList.Add(new Guid(item.Value));
                    }

                    allDic.Add(item.Key, curList);
                }
            }
            else // System classifiers for normal search
            {
                foreach (var item in this.ClassifierFiltersSystem)
                {
                    var curList = new List<Guid>();
                    curList.Add(item.Value);

                    allDic.Add(item.Key, curList);
                }
            }

            foreach (var item in this.ClassifierFilters)
            {
                if (allDic.ContainsKey(item.Key))
                {
                    allDic[item.Key] = item.Value.Select(i => i.Id).ToList();
                }
                else
                {
                    allDic.Add(item.Key, item.Value.Select(i => i.Id).ToList());
                }
            }

            foreach (int key in allDic.Keys)
            {
                l.Add(allDic[key].Select(c => c.ToString()).Last());
            }

            return l;
        }

        public void PopulateSystemClassifiers()
        {
            // Custom filter levels
            if (this.SearchType == SearchTypes.Simple)
            {

                //   this.ClassifierFiltersSystem.Add(2, new Guid("32016c73-e519-447a-a222-a7ccfe2ae675"));
                // this.ClassifierFiltersSystemIds.Add(new Guid("32016c73-e519-447a-a222-a7ccfe2ae675")); // AFTER LISBON
            }
            else if (this.SearchType == SearchTypes.Legislation)
            {
                if (this.SearchBoxFilters.Law.LegislationType == LegislationType.EU)
                {
                    this.ClassifierFiltersSystem.Add(8, new Guid("af88ca51-7522-455a-aefe-ec0d3c2d6a37"));
                }
                else // national legislation
                {
                    this.ClassifierFiltersSystem.Add(8, new Guid("987e4eef-3e55-43be-9052-bb98ef1dfd83"));
                }
            }
            else if (this.SearchType == SearchTypes.CaseLaw)
            {
                // this.ClassifierFiltersSystem.Add(2, new Guid("32016c73-e519-447a-a222-a7ccfe2ae675"));
                // this.ClassifierFiltersSystemIds.Add(new Guid("32016c73-e519-447a-a222-a7ccfe2ae675")); // AFTER LISBON

                switch (this.SearchBoxFilters.Cases.CaseLawType)
                {
                    case Enums.CaseLawType.All:
                        if (!this.ClassifierFiltersSystem.ContainsKey(8))
                        {
                            this.ClassifierFiltersSystem.Add(8, new Guid("1a5f1eae-99a5-452e-a919-f2f3d11d3168"));
                        }
                        // this.ClassifierFiltersSystemIds.Add(new Guid("1a5f1eae-99a5-452e-a919-f2f3d11d3168"));
                        // Jurisdictions - already gotten
                        break;
                    case Enums.CaseLawType.EU:
                        // Document type - level 3:
                        if (!this.ClassifierFiltersSystem.ContainsKey(8))
                        {
                            this.ClassifierFiltersSystem.Add(8, new Guid("030bc6ca-9696-4efe-ba4b-27d54a335726"));
                        }

                        if (!this.ClassifierFiltersSystem.ContainsKey(6))
                        {
                            this.ClassifierFiltersSystem.Add(6, new Guid("ff977726-a75e-4f89-bf8c-cc8f451287a3"));
                        }

                        /*  if (!this.ClassifierFiltersSystem.ContainsKey(7))
                          {
                              // add equivalent
                          }*/

                        break;
                    case Enums.CaseLawType.National:
                        if (!this.ClassifierFiltersSystem.ContainsKey(8))
                        {
                            this.ClassifierFiltersSystem.Add(8, new Guid("fe8a24db-1e8a-49fd-9d80-bce857c6c944"));
                        }
                        // Jurisdiction - without European UNION - removed after result is returned
                        break;
                    case Enums.CaseLawType.ECHR:
                        // Courts
                        if (!this.ClassifierFiltersSystem.ContainsKey(6))
                        {
                            this.ClassifierFiltersSystem.Add(6, new Guid("6672ab3c-3386-4204-a019-493cdd62cd31"));
                        }

                        /*  if (!this.ClassifierFiltersSystem.ContainsKey(7))
                          {
                              // add equivalent
                          }*/

                        if (!this.ClassifierFiltersSystem.ContainsKey(8))
                        {
                            this.ClassifierFiltersSystem.Add(8, new Guid("6d981719-977c-4a7f-ade1-9b6e0e8852d6")); // document type
                        }

                        //  ClassifierFiltersSystemIds.Add(new Guid("6672ab3c-3386-4204-a019-493cdd62cd31")); // European court human rights
                        break;
                    default:
                        break;
                }
            }
        }

        public FilterClassifierItemsResponse GetFilterClassifiers(int siteLangId)
        {
            FilterClassifierItemsResponse response = new FilterClassifierItemsResponse
            {
                SearchSource = this.SearchSource
            };

            if (this.SearchSource == SearchSources.HomePage)
            {
                var query = this.SearchBoxFilters.HomeSearch.Query;

                if (query == "props:pr1 (classificators:(030bc6ca96964efeba4b27d54a335726 || fe8a24db1e8a49fd9d80bce857c6c944))"
                    || query == "props:pr1 (classificators:(030bc6ca96964efeba4b27d54a335726))"
                    || query == "props:pr1 (classificators:(fe8a24db1e8a49fd9d80bce857c6c944))")
                {
                    response.IsCaseLawFolder = true;
                }
            }

            if (dbSearchId == -1)
                throw new Exception("GetFilterClassifiers(): Не е създадено търсене. dbSearchId = -1");

            var allClassifiers = this.GetAllClassifiers();

            int[] searchIds = Search.GetSearchDocLangIdsBytes(this.dbSearchId);

            var classifiersToStruct = new List<string>();

            foreach (var classifier in allClassifiers)
            {
                var originalClassifiers = ClassificationService.ClassificatorService.Current.MappingService.TryGetOriginalsFromMapping(classifier);
                //   var originalClassifier = ClassificationService.ClassificatorService.Current.MappingService.TryGetOriginalFromMapping(classifier);
                if (originalClassifiers != null && originalClassifiers.Count > 0)
                {
                    foreach (var originalClassifier in originalClassifiers)
                    {
                        classifiersToStruct.Add(originalClassifier.Guid);
                    }
                }
                else
                {
                    classifiersToStruct.Add(classifier);
                }
            }

            Dictionary<string, int> d = this.FilterDocsClassifiers.GetClassifiers(searchIds, classifiersToStruct);

            string docClassifiersJSON = d.ToJson();

            var currentClassifiersToDB = allClassifiers.Select(i => new Guid(i)).ToList();
            if (this.ClassifierFilters.ContainsKey(2) == false && this.SearchBoxFilters.Law == null)
            {
                currentClassifiersToDB.Add(new Guid("32016c73-e519-447a-a222-a7ccfe2ae675")); // Adding after lisbon for simple and caselaw searches
            }

            if (this.ClassifierFilters.ContainsKey(1024) == false && ClassifiersProvider.Service.TryGetTreeByGuid("c7b20717-7571-4c96-a102-40acbe3b1159") != null)
            {
                currentClassifiersToDB.Add(new Guid("c7b20717-7571-4c96-a102-40acbe3b1159")); // Summary starting from down level temporarily if "bad" classifier exists;
            }

            //TODO: getting original guids
            for (int i = 0; i < currentClassifiersToDB.Count; i++)
            {
                var mappedClassifier = ClassificationService.ClassificatorService.Current.MappingService.TryGetMappingFromOriginal(currentClassifiersToDB[i].ToString());
                if (mappedClassifier != null)
                {
                    currentClassifiersToDB[i] = new Guid(mappedClassifier.Guid);
                }
            }

            List<ClassifierItem> l = GetFilterClassifiersFromCache(currentClassifiersToDB.Select(c => c.ToString()).ToList(), siteLangId, d);

            // European Union jurisdiction remove in National CaseLaw
            if (this.SearchType == SearchTypes.CaseLaw && this.SearchBoxFilters.Cases.CaseLawType == Enums.CaseLawType.National)
            {
                if (l.Any(i => i.Id == new Guid("10e88d3e-7589-46b3-bd75-35c08ab46a27"))) // European Union jurisdiction
                {
                    var europeanUnionClassifier = l.Where(i => i.Id == new Guid("10e88d3e-7589-46b3-bd75-35c08ab46a27")).FirstOrDefault();
                    l.Remove(europeanUnionClassifier);
                }
            }

            // Папка "Законодателство"
            if (this.SearchSource == SearchSources.HomePage && this.SearchBoxFilters.HomeSearch.Query == "props:pr1 (classificators:(af88ca517522455aaefeec0d3c2d6a37 || 987e4eef3e5543be9052bb98ef1dfd83 -f83a4979034843e0a7b6d50c07d4eee9))")
            {
                var europeanUnionClassifier = l.Where(i => i.Id == new Guid("be076ed2-9f60-4b24-9560-19b5e672d947")).FirstOrDefault();
                if (europeanUnionClassifier != null)
                {
                    l.Remove(europeanUnionClassifier);
                }
            }

            response.Data = l;
            return response;
        }

        #endregion

        #region Static methods ...

        public static SearchResult FindSearchResult(int searchId, object searchResults)
        {
            SearchResult sr = null;

            Dictionary<int, SearchResult> s = (Dictionary<int, SearchResult>)searchResults;

            if (s != null)
            {
                s.TryGetValue(searchId, out sr);
            }

            return sr;
        }

        public static int? AddSearchResult(SearchResult sr, object searchResults)
        {
            Dictionary<int, SearchResult> s = null;
            if (searchResults == null)
                return null;
            try
            {
                s = (Dictionary<int, SearchResult>)searchResults;
            }
            catch
            {
                return null;
            }

            int searchId = 10;
            int minSearchId = 10;
            int searchResultCount = 0;
            foreach (int key in s.Keys.ToArray<int>())
            {
                // reserved searchid range 1-10
                if (key < 10)
                {
                    continue;
                }

                searchResultCount++;
                if (key > searchId)
                {
                    searchId = key;
                }

                if (minSearchId == 10 || minSearchId > key)
                {
                    minSearchId = key;
                }
            }

            if (searchResultCount >= Convert.ToInt32(ConfigurationManager.AppSettings["Session_SearchResults_Count"]))
                s.Remove(minSearchId);

            searchId++;
            sr._siteSearchId = searchId;
            s.Add(searchId, sr);

            //s.Add("-global-"+searchId, sr.SearchBoxFilters.GlobalId);

            return searchId;
        }

        public static object GetNewSearchWrapper(string searchWrapperBasePath)
        {
            return new EUCasesSearchWrapper(searchWrapperBasePath);
        }

        public static object GetNewFilterDocsStruct(string filterDocsStructBasePath)
        {
            FilterDocsStruct fds = new FilterDocsStruct();
            fds.LoadFromFile(filterDocsStructBasePath);
            return fds;
        }

        public static object GetNewFilterDocsClassifiers(string filterDocsStructBasePath)
        {
            DocToClassifierWrapper fds = new DocToClassifierWrapper(filterDocsStructBasePath, FileDBOpenMode.omRead);
            return fds;
        }

        public static object GetNewSearchGroupper(string searchGroupperBasePath)
        {
            var sgrp = new SearchResultsGrouper.ResultsGrouper(searchGroupperBasePath);
            return sgrp;
        }

        private static List<ClassifierItem> GetFilterClassifiersFromCache(List<string> selectedClassifiersIds, int langId, Dictionary<string, int> docClassifiers)
        {
            var classifierItems = new List<ClassifierItem>();

            var selectedClassifierTypes = selectedClassifiersIds.Select(sc =>
            ClassificationService.ClassificatorService.Current.TryGetTreeByGuid(sc).ClassifierTypeId).ToList();

            var baseLevelParents = ClassifiersProvider.Classifiers.Where(c => c.TreeLevel == 1 && !selectedClassifierTypes.Contains(c.ClassifierTypeId)).ToList();

            var classifierItemsUnnormalizedChildrenList = baseLevelParents.Select(bp => bp.Children)
                .Union(selectedClassifiersIds.Select(sci => ClassificationService.ClassificatorService.Current.TryGetTreeByGuid(sci).Children)).ToList();

            foreach (var childrenList in classifierItemsUnnormalizedChildrenList)
            {
                foreach (var item in childrenList)
                {
                    ReadOnlyCollection<ClassificatorTreeModel> originalClassifiers = null;

                    if (!docClassifiers.ContainsKey(item.Guid) && item.ClassifierTypeId != 7)
                    {
                        continue;
                    }
                    else if (item.ClassifierTypeId == 7)
                    {
                        originalClassifiers = ClassificationService.ClassificatorService.Current.MappingService.TryGetOriginalsFromMapping(item.Guid);
                        var allGuids = originalClassifiers.Select(oc => oc.Guid).ToList();
                        if (originalClassifiers != null && originalClassifiers.Count > 0 && !(allGuids.Any(e => docClassifiers.ContainsKey(e))))
                        {
                            continue;
                        }
                    }

                    var normalizedClassifierItem = new ClassifierItem();
                    normalizedClassifierItem.Id = new Guid(item.Guid);
                    normalizedClassifierItem.Name = item.LanguageVariantsWithHints[langId.ToString()].Title;

                    if (item.ClassifierTypeId == 7)
                    {
                        if (originalClassifiers != null && originalClassifiers.Count > 0)
                        {
                            var countSum = 0;
                            var originalClassifiersGuids = originalClassifiers.Select(oc => oc.Guid).ToList();

                            if (!docClassifiers.Keys.Any(s => originalClassifiersGuids.Contains(s)))
                            {
                                continue;
                            }

                            foreach (var guid in originalClassifiersGuids)
                            {
                                if (docClassifiers.ContainsKey(guid))
                                {
                                    countSum += docClassifiers[guid];
                                }
                            }

                            normalizedClassifierItem.DocsCount = countSum;
                        }
                    }
                    else
                    {
                        normalizedClassifierItem.DocsCount = docClassifiers[item.Guid];
                    }


                    normalizedClassifierItem.Type = (ClassifierTypes)item.ClassifierTypeId;
                    if (item.Order.HasValue)
                    {
                        normalizedClassifierItem.OrderValue = item.Order.Value;
                    }

                    classifierItems.Add(normalizedClassifierItem);
                }
            }

            return classifierItems.OrderBy(c => c.Type).ThenBy(c => c.OrderValue).ThenBy(c => c.Name).ToList();
        }

        #endregion
    }

    public class ClassifierType
    {
        public int ClassifierTypeId { get; set; }
        public string FilterTitle_ResKey { get; set; }
    }
}