namespace Interlex.App.Controllers
{
    using System;
    using System.Configuration;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Enums;
    using Interlex.BusinessLayer.Models;
    using System.Web;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Interlex.App.Resources;
    using Interlex.App.Helpers;
    using System.Linq;

    [AllowAnonymous]
    public class ApplicationUpdateController : Controller
    {
        private string FTI()
        {
            string ip = HttpContextHelper.GetClientIPAddress(); //Request.UserHostAddress;
            try
            {
                string searchWrapperUrl = null;
                if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
                {
                    searchWrapperUrl = "~/" + ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
                }
                else
                {
                    searchWrapperUrl = "~/" + ConfigurationManager.AppSettings["SearchWrapper_BasePath_cc"];
                }

                HttpContext.Application["SearchWrapper"] = SearchResult.GetNewSearchWrapper(Server.MapPath(searchWrapperUrl));

                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.FTI, true, ip);

                return "Successfuly updated FTI";
            }
            catch (Exception ex)
            {
                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.FTI, false, ip);

                return ex.Message;
            }
        }

        private string DocsStruct()
        {
            string ip = HttpContextHelper.GetClientIPAddress(); //Request.UserHostAddress;
            try
            {
                string filterDocsStructUrl = null;
                if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
                {
                    filterDocsStructUrl = "~/" + ConfigurationManager.AppSettings["FilterDocsStruct_BasePath"];
                }
                else
                {
                    filterDocsStructUrl = "~/" + ConfigurationManager.AppSettings["FilterDocsStruct_BasePath_cc"];
                }

                HttpContext.Application["FilterDocsStruct"] = SearchResult.GetNewFilterDocsStruct(Server.MapPath(filterDocsStructUrl)); // For mother RUSSIA!!!
                HttpContext.Application["FilterDocsClassifiers"] = SearchResult.GetNewFilterDocsClassifiers(Server.MapPath(filterDocsStructUrl));
                HttpContext.Application["ClassifiersMap"] = Interlex.BusinessLayer.Classifiers.GetClassifiersMap();

                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.DocStruct, true, ip);
                return "Successfuly updated DocsStruct";
            }
            catch (Exception ex)
            {
                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.DocStruct, false, ip);
                return ex.Message;
            }
        }

        private string Classifiers()
        {
            string ip = HttpContextHelper.GetClientIPAddress();  //Request.UserHostAddress;

            try
            {
                ClassifiersProvider.ClearClassifiers();
                ClassifiersProvider.PopulateClassifiersFromCache(); //Will always get them from DB here because they are previously cleared

                ClassifiersProvider.ClearMappings();
                ClassifiersProvider.PopulateMappingsFromCache(); //Will always get them from DB here because they are previously cleared

                ClassifiersProvider.Fill();

                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.Classifiers, true, ip);
                return "Successfuly updated classifiers and classifier mappings";
            }
            catch (Exception ex)
            {
                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.Classifiers, false, ip);
                return ex.Message;
            }
        }

        public string FoldersDocsCount()
        {
            string ip = HttpContextHelper.GetClientIPAddress();  //Request.UserHostAddress;

            try
            {
                int[] wsRes = null;
                //SearchBox sb = new SearchBox(langId);
                //sb.SearchText = term;
                int[] langPref = new int[] { 4, 1, 5 };
                string searchWrapperUrl = null;

                if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
                }
                else
                {
                    searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath_cc"];
                }

                SearchResult sr = new SearchResult(SearchSources.Search, HttpContext.Application["SearchWrapper"],
                        searchWrapperUrl,
                        HttpContext.Application["FilterDocsStruct"],
                        HttpContext.Application["FilterDocsClassifiers"],
                        HttpContext.Application["ClassifiersMap"],
                        HttpContext.Application["ResultsGroupper"],
                        HttpRuntime.AppDomainAppPath, 1, 0, 0
                      );

                foreach (var folder in Home.GetFoldersFlat(1, 1))
                {
                    sr.SearchFTQuery(folder.query, ref wsRes, langPref);

                    Home.SetFolderDocsCount(folder.id, wsRes.Length);
                }
                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.FoldersDocsCount, true, ip);
                return "Successfuly updated home folders docs counts";
            }
            catch (Exception ex)
            {
                Logger.LogApplicationUpdate(HttpRuntime.AppDomainAppPath, ApplicationUpdateType.Classifiers, false, ip);
                return ex.Message;
            }
        }

        private string ResultsGroupper()
        {
            string ip = HttpContextHelper.GetClientIPAddress(); //Request.UserHostAddress;

            try
            {
                HttpContext.Application["ResultsGroupper"] = SearchResult.GetNewSearchGroupper(HttpRuntime.AppDomainAppPath);
                //TODO: write to logger
                return "Successfuly updated results groupper";
            }
            catch (Exception ex)
            {
                //TODO: write to logger
                return ex.Message;
            }
        }

        private string MultilingualDictionary()
        {
            try
            {
                CacheProvider.Provider.DeleteCacheItem("multilingual_dictionary");
                CacheProvider.Provider.GetOrSetForever("multilingual_dictionary", () => MultiDictItem.GetAllMultiDictItems());

                return "Successfuly updated multilingual dictionary entries";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string MultilingualDictionaryAlphabets()
        {
            string ip = HttpContextHelper.GetClientIPAddress();  //Request.UserHostAddress;

            try
            {
                Languages.RePopulateAlphabetsToCache();
                return "Successfuly updated alphabets";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult AfterUpdate()
        {
            string ip = HttpContextHelper.GetClientIPAddress();// Request.UserHostAddress;

            Logger.LogApplicationUpdateMessage(HttpRuntime.AppDomainAppPath, $"Request from {ip}");

            var results = new List<string>();

            if (ip == "::1" || ip == "127.0.0.1" || ip == "192.168.7.212" || ip == "87.121.111.212" || ip == "87.121.111.210")
            {
                results.Add(this.FTI()); // Updating FTI
                results.Add(this.ResultsGroupper()); // Updating search results groupper
                results.Add(this.DocsStruct()); // Updating languages structures
                results.Add(this.Classifiers()); // Updating classifiers
                results.Add(this.MultilingualDictionaryAlphabets()); // Updating multilingual dictionary alphabet letters
                results.Add(this.MultilingualDictionary()); // Updating multilingual dictionary entries
                results.Add(this.FoldersDocsCount()); // Updating home folders docs count in database

                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json($"Ip address {ip} not authorized to perform an update", JsonRequestBehavior.AllowGet);
            }
        }

        public void Test()
        {

            string ip = HttpContextHelper.GetClientIPAddress();
            Response.Write("ApplicationPath=" + HttpContext.Request.ApplicationPath + ";");
            Response.Write("MyIP=" + ip + ";");
            HttpContext.Request.Headers.AllKeys.ToList().ForEach(x => { Response.Write(x + "=" + HttpContext.Request.Headers[x] + ";"); });
            //Response.Write(ip);
        }
    }
}
