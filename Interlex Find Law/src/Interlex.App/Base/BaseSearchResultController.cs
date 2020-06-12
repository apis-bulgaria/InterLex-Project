using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Enums;

namespace Interlex.App
{
    public abstract class BaseSearchResultController : BaseController
    {
        protected readonly int pageSize;
        protected readonly int visiblePagesCount;

        public BaseSearchResultController()
            : base()
        {
            this.pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["ListPageCount"]);
            this.visiblePagesCount = Convert.ToInt32(ConfigurationManager.AppSettings["VisiblePagesCount"]);
        }

        protected override void ExecuteCore()
        {
            if (Session["SearchResults"] == null)
            {
                Dictionary<int, SearchResult> srDict = new Dictionary<int, SearchResult>();
                
                // Reserved searchId range 1-10
                // Home: searchId=1

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
                    this.Language.Id, pageSize, visiblePagesCount);
                srDict.Add(1, sr);

                Session["SearchResults"] = srDict;
                
            }
            base.ExecuteCore();
        }

    }
}
