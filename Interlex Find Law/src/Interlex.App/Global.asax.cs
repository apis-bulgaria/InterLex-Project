namespace Interlex.App
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Optimization;
    using System.Configuration;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Models;
    using Interlex.App.Helpers;
    using Interlex.App.CustomBinders;
    using System.Web.SessionState;
    using Apis.Common.Extensions;
    using System.IO;
    using AkomaNtosoXml.Xslt.Core.Classes.Resolver;
    using Apis.Mail.Notification;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // registering custom view engines
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new DocListViewEngine());

            // this.GenerateLessColors();
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application.Lock();

            string searchWrapperUrl = null;
            string filterDocsStructUrl = null;
            if (ConfigurationManager.AppSettings["SolutionVersion"] == "product")
            {
                searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath"];
                filterDocsStructUrl = ConfigurationManager.AppSettings["FilterDocsStruct_BasePath"];
            }
            else
            {
                searchWrapperUrl = ConfigurationManager.AppSettings["SearchWrapper_BasePath_cc"];
                filterDocsStructUrl = ConfigurationManager.AppSettings["FilterDocsStruct_BasePath_cc"];
            }

            Application["SearchWrapper"] = null;
            Application["FilterDocsStruct"] = null;
            Application["FilterDocsClassifiers"] = null;
            Application["ClassifiersMap"] = null;
            Application["ResultsGroupper"] = null;

            //Application["SearchWrapper"] = SearchResult.GetNewSearchWrapper(ConfigurationManager.AppSettings["SearchWrapper_BasePath"]);
            Application["SearchWrapper"] = SearchResult.GetNewSearchWrapper(Server.MapPath(searchWrapperUrl));
            Application["FilterDocsStruct"] = SearchResult.GetNewFilterDocsStruct(Server.MapPath(filterDocsStructUrl));
            Application["FilterDocsClassifiers"] = SearchResult.GetNewFilterDocsClassifiers(Server.MapPath(filterDocsStructUrl));
            Application["ClassifiersMap"] = Classifiers.GetClassifiersMap();
            Application["ResultsGroupper"] = SearchResult.GetNewSearchGroupper(HttpRuntime.AppDomainAppPath);
            Application.UnLock();

            //ObjectFactory.Initialize(x => { x.PullConfigurationFromAppConfig = true; });
            //StructureMapConfiguration();

            ModelBinders.Binders.Add(typeof(SearchBox), new SearchBoxBinder());
            ModelBinders.Binders.Add(typeof(SearchCases), new SearchCasesBinder());
            ModelBinders.Binders.Add(typeof(SearchLaw), new SearchLawBinder());
            ModelBinders.Binders.Add(typeof(SearchFinances), new SearchFinancesBinder());

            AkomaNtosoPreProcessor.IsInterlex = true;
            AkomaNtosoPreProcessor.EnableEurlexCasesHints = true;
            AkomaNtosoPreProcessor.DisableMetadataCitation = false;
            AkomaNtosoPreProcessor.FullFilePathProvider = this.Server.MapPath;
#if DEBUG
            AkomaNtosoPreProcessor.EnableEurlexTableAnnotations = true;
#endif
            var isTestEnviournment = WebConfigHelper.IsTestEnvironment();
            if (isTestEnviournment)
            {
                AkomaNtosoPreProcessor.EnableEurlexTableAnnotations = true;
            }

            // Popualting multilingual dictionary alphabets 
            Languages.RePopulateAlphabetsToCache();

            CacheProvider.Provider.DeleteCacheItem("multilingual_dictionary");
            CacheProvider.Provider.GetOrSetForever("multilingual_dictionary", () => MultiDictItem.GetAllMultiDictItems());
        }

        protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null && Session != null && Session.SessionID != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = Session.SessionID;
                    if (Request.Url.Host.ToLower() != "localhost")
                    {
                        Response.Cookies["ASP.NET_SessionId"].Domain = Request.Url.Host;//".know24.net"; // the full stop prefix denotes all sub domains
                        Response.Cookies["ASP.NET_SessionId"].Path = "/"; //default session cookie path root         
                    }
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            AkomaNtosoPreProcessor.ImagePath = $"{WebAppHelper.AppRootFolder}\\DocImages\\";
            AkomaNtosoPreProcessor.RootPath = WebAppHelper.AppRootFolder;
        }

        /// <summary>
        /// Handles the EndRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // THE HELL AM I DOING MICROSOFT BECAUSE OF YOUR IIS POOL?
            //  Application["SearchWrapper"] = null;
            // Application["FilterDocsStruct"] = null;
            //  Application["FilterDocsClassifiers"] = null;
            //  Application["ClassifiersMap"] = null;
            // Application["ResultsGroupper"] = null;

            //  GC.Collect();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
#if DEBUG
            return;
#endif

            if (!System.Diagnostics.EventLog.SourceExists
                    ("Interlex"))
            {
                System.Diagnostics.EventLog.CreateEventSource
                   ("Interlex", "Application");
            }

            var ex = Server.GetLastError();

            Server.ClearError();

            string userInfo = "";
            if (Session["UserData"] != null)
            {
                UserData userData = Session["UserData"] as UserData;
                if (userData != null) { userInfo = "-- " + userData.Username + " (" + userData.UserId + ")"; }
            }
            string urlPath = Request.Url.PathAndQuery;

            string errorMsg = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "--" + urlPath + "-- " + userInfo + Environment.NewLine + "Message: " + ex.Message + "|| Stack Trace: " + ex.StackTrace + "|| Target: " + ex.TargetSite
                + Environment.NewLine + Environment.NewLine;

            System.Diagnostics.EventLog.WriteEntry("Interlex", errorMsg, System.Diagnostics.EventLogEntryType.Error);

            /*exception logging in logs folder*/
            var logsDirectory = HttpRuntime.AppDomainAppPath;
            logsDirectory = logsDirectory + "Logs\\";

            var timeOfLoging = DateTime.Now;
            var timePathParsed = "";

            if (timeOfLoging.Day.ToString().Length == 1)
            {
                if (timeOfLoging.Month.ToString().Length == 1)
                {
                    timePathParsed = timePathParsed + timeOfLoging.Year + "_0" + timeOfLoging.Month + "_0" + timeOfLoging.Day;
                }
                else
                {
                    timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_0" + timeOfLoging.Day;
                }
            }
            else
            {
                if (timeOfLoging.Month.ToString().Length == 1)
                {
                    timePathParsed = timePathParsed + timeOfLoging.Year + "_0" + timeOfLoging.Month + "_" + timeOfLoging.Day;
                }
                else
                {
                    timePathParsed = timePathParsed + timeOfLoging.Year + "_" + timeOfLoging.Month + "_" + timeOfLoging.Day;
                }
            }

            var fullPath = logsDirectory + timePathParsed + ".txt";

            //File.Create(fullPath);
            System.IO.File.AppendAllText(fullPath, errorMsg + Environment.NewLine);

            // xslt error
            if (ex.StackTrace.Contains("AkomaNtosoXml.Xslt.Core.Classes.Resolver"))
            {
                var dbName = ConfigurationManager.ConnectionStrings["ConnPG"].DatabaseName();

                var title = $"[{dbName}] Xslt error";
                var body = $"<div>{ex}</div>";

                ApisEmail.SendFromSupport(to: "oreshenski@apis.bg", title: title, body: body);
            }

            Server.TransferRequest("~/Error/500", true);
            // Response.Redirect("~/Error/500", false);
            //  Response.End();
            //  Response.End();
            //  HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            System.Web.SessionState.HttpSessionState Session = System.Web.HttpContext.Current.Session;
            HttpRequest Request = System.Web.HttpContext.Current.Request;

            string[] host = Request.Url.Host.Split('.');
            if (Session != null && Session.IsNewSession && (host[0].ToLower() == "demo" || host[0].ToLower() == "freecases"))
            {
                // temporarily saving user agent strings, gotta catch that google bot.
                var userAgentString = Request.UserAgent;
                /*  using (StreamWriter sw = new StreamWriter(Server.MapPath("~/Logs/ua.txt"), true))
                  {
                      sw.WriteLine(userAgentString);
                  }*/
                // end of temp saving

                UserData ud = UserMng.Login("sysdemo", ConfigurationManager.AppSettings["DEMO_USER_PASS"], false, Request.UserHostAddress);
                if (ud != null)
                {
                    if (ud.SessionId > 0)
                    {
                        Session["UserData"] = ud;
                        Session.Timeout = ud.SessionTimeout; // minutes

                        var productsList = UserMng.GetProductsList(ud.UserId);
                        Session["ProductsList"] = productsList;

                        var selectedProductCookie = Request.Cookies["SelectedProductId"];
                        int selectedProductId = 1;

                        if (ud.Products.Where(p => p.IsActive.HasValue && p.IsActive == true).ToList().Count == 1)
                        {
                            selectedProductId = ud.Products.FirstOrDefault().ProductId;
                            var newCookie = new HttpCookie("SelectedProductId");
                            newCookie.Value = selectedProductId.ToString();

                            Response.SetCookie(newCookie);
                        }
                        else
                        {
                            if (selectedProductCookie != null && selectedProductCookie.Value != null && selectedProductCookie.Value.ToString() != "")
                            {
                                selectedProductId = int.Parse(selectedProductCookie.Value.ToString());
                            }
                        }
                        Session["SelectedProductId"] = selectedProductId;
                    }
                }
            }


            if (Session != null)
            {
                if (Request.Cookies["sitelang"] != null)
                {
                    Language currentLang = InterfaceLanguages.GetLanguageByCode(Request.Cookies["sitelang"].Value);
                    if (!currentLang.IsInterfaceLang) // some bad cookie
                    {
                        currentLang = null;
                    }
                    if (currentLang != null) // check if cookie contains valid language code, mmmmmm cookieeee (Cookie monster was here)
                        Session["LanguageId"] = currentLang.Id;
                }

                if (Session["LanguageId"] == null)
                {
                    Language preferedLang = null;
                    string languageCode;

                    if (Request.UserLanguages != null)
                    {
                        foreach (var headerLang in Request.UserLanguages)
                        {
                            languageCode = headerLang.Split(';')[0];

                            if (languageCode.Length == 2)
                            {
                                preferedLang = InterfaceLanguages.GetLanguageByShortCode(languageCode);
                            }
                            else
                            {
                                preferedLang = InterfaceLanguages.GetLanguageByCode(languageCode);
                            }

                            if (preferedLang != null && !preferedLang.IsInterfaceLang)
                            {
                                preferedLang = null;
                            }

                            if (preferedLang != null)
                            {
                                break;
                            }
                        }
                    }

                    if (preferedLang == null)
                    {
                        preferedLang = InterfaceLanguages.GetLanguageById(Convert.ToInt32(ConfigurationManager.AppSettings["DefaultLanguageId"]));
                    }

                    Session["LanguageId"] = preferedLang.Id;

                    //HttpCookie cultureCookie = new HttpCookie("sitelang", defaultLang.Code);
                    //cultureCookie.Expires = DateTime.Now.AddYears(1);
                    //Response.Cookies.Add(cultureCookie);
                }

                string code = InterfaceLanguages.GetLanguageById(Convert.ToInt32(Session["LanguageId"])).Code;
                CultureInfo ci = new CultureInfo(code);
                System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

                if (Request.Cookies["SelectedProductId"] != null)
                {
                    Session["SelectedProductId"] = Request.Cookies["SelectedProductId"].Value.ToString();
                }

                if (Session["SelectedProductId"] == null)
                {
                    Session["SelectedProductId"] = 1;
                }

                // Refreshing language to english in T&FSt if previously another not primary language has been selected. Sry for this. Blame it on the financists.
                if (Convert.ToInt32(Session["SelectedProductId"]) == 2
                    && Convert.ToInt32(Session["LanguageId"]) > 4
                    && Session["UserData"] != null
                    && ((Session["UserData"] as UserData).ClientId != 1 || (Session["UserData"] as UserData).Username.ToUpper() == "SYSDEMO")) // Apis Europe client ignore
                {
                    Session["LanguageId"] = 4; // english
                    var newLangCookie = new HttpCookie("sitelang");
                    newLangCookie.Value = "en-GB";
                    newLangCookie.Expires.AddYears(365);
                    Response.SetCookie(newLangCookie);
                }
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(
                  System.Web.SessionState.SessionStateBehavior.Required);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            for (int i = 1; i <= 5; i++) // Increase counter if products become more than 5
            {
                Session.Add("opened_docs-" + i, new HashSet<int>());
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            if (Session["UserData"] != null)
            {
                System.Diagnostics.Debug.WriteLine("Logged Session end (sessionId=" + ((UserData)Session["UserData"]).SessionId.ToString() + ")");

                string sessionFolder = HttpRuntime.AppDomainAppPath;
                sessionFolder = sessionFolder + "Session_Files\\" + ((UserData)Session["UserData"]).SessionId.ToString();

                UserMng.EndUserSession(((UserData)Session["UserData"]).SessionId, sessionFolder);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Session_End");
            }

        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(@"~/api");
        }

        //private void StructureMapConfiguration()
        //{
        //    ObjectFactory.Initialize(x =>
        //    {
        //        x.For<IClassifier>().HybridHttpOrThreadLocalScoped().Use<Classifier>();
        //        //x.For<IEurovoc>().HybridHttpOrThreadLocalScoped().Use<Eurovoc>();

        //        //x.For<ISubjectMatter>().HybridHttpOrThreadLocalScoped().Use<SubjectMatterMock>();
        //        //x.For<ICourts>().HybridHttpOrThreadLocalScoped().Use<CourtsMock>();
        //        //x.For<ISyllabus>().HybridHttpOrThreadLocalScoped().Use<SyllabusMock>();
        //        //x.For<IEULegislation>().HybridHttpOrThreadLocalScoped().Use<EULegislationMock>();
        //        x.For<IJurisdiction>().HybridHttpOrThreadLocalScoped().Use<JurisdictionMock>();
        //    }
        //        );
        //}
    }
}
