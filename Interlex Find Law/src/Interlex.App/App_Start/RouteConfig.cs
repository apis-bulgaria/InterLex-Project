using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Configuration;
using System.Configuration;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Models;

namespace Interlex.App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            string siteLangs = "(" + String.Join("|", InterfaceLanguages.Languages.Where(l => l.IsInterfaceLang == true).Select(l => l.ShortCode).ToList()) + ")";

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("browserconfig.xml");
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("DocImages/{*pathInfo}");
            routes.IgnoreRoute("Icons/{*pathInfo}");
            routes.IgnoreRoute("font-awesome/{*pathInfo}");

            //routes.MapRoute(
            // name: "ApisLink",
            // url: "Doc/CourtAct/CELEX={celex}{ToPar}",
            // defaults: new { controller = "Doc", action = "ApisLink", ToPar = UrlParameter.Optional },
            // constraints: new { shortCode = siteLangs }
            // );

            routes.MapRoute(
             name: "ChangeLanguage",
             url: "Lang-{shortCode}",
             defaults: new { controller = "Home", action = "ChangeLanguage" },
             constraints: new { shortCode = siteLangs }
             );

            routes.MapRoute(
                name: "Tour",
                url: "Tour/{productName}",
                defaults: new { controller = "Home", action = "ChangeProduct" },
                constraints: new { productName = "(EuroCases|TaxAndFinancialStandards)" }
                );

            // Document by docLangId
            routes.MapRoute(
              name: "ActById",
              url: "Doc/Act/Id/{id}/{siteSearchId}",
              defaults: new { controller = "Doc", action = "CourtAct", siteSearchId = UrlParameter.Optional }
              );

            routes.MapRoute(
              name: "ProductFeaturesInfo",
              url: "User/ProductFeaturesInfo/{funcTypeId}",
              defaults: new { controller = "User", action = "ProductFeaturesInfo", funcTypeId = UrlParameter.Optional }
              );

            routes.MapRoute(
             name: "ProductFeaturesInfoPartial",
             url: "User/ProductFeaturesInfoPartial/{funcTypeId}",
             defaults: new { controller = "User", action = "ProductFeaturesInfoPartial", funcTypeId = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "MachineTranslation",
                url: "MachineTranslation",
                defaults: new { controller = "Home", action = "MachineTranslation", langId = UrlParameter.Optional }
            );

            routes.MapRoute(
          name: "FinancesTaxInformation",
          url: "Finances/TaxInformation/{type}",
          defaults: new { controller = "Finances", action = "Index", page = "TaxInformation" }
          );

            routes.MapRoute(
      name: "FinancialCrysisGlossary",
      url: "Finances/FinancialCrysisGlossary/{type}",
      defaults: new { controller = "Finances", action = "Index", page = "FinancialCrysisGlossary" }
      );

            routes.MapRoute(
               name: "FinancesParametrified",
               url: "Finances/{page}/{from}-{to}/{type}",
               defaults: new { controller = "Finances", action = "Index", from = UrlParameter.Optional, to = UrlParameter.Optional, type = UrlParameter.Optional }
               );



            routes.MapRoute(
              name: "FinancesData",
              url: "Finances/Data/{action}",
              defaults: new { controller = "Finances", action = "Index" }
              );

            routes.MapRoute(
              name: "Finances",
              url: "Finances/{page}",
              defaults: new { controller = "Finances", action = "Index" }
              );

            routes.MapRoute(
                name: "Compare",
                url: "Compare/{firstDocument}/{secondDocument}",
                defaults: new { controller = "Compare", action = "Index" }
                );

            routes.MapRoute(
                name: "CompareByIdentifier",
                url: "Compare/ByIdentifier/{firstIdentifier}/{secondIdentifier}",
                defaults: new { controller = "Compare", action = "ByIdentifier" }
                );

            #region Search routes...

            routes.MapRoute(
                name: "SearchBox",
                url: "SearchBox/Show/{searchId}",
                defaults: new { controller = "SearchBox", action = "Show", searchId = UrlParameter.Optional },
                constraints: new { searchId = @"\d+" }
            );

            routes.MapRoute(
               name: "CommonSearches",
               url: "SearchBox/CommonSearches/",
               defaults: new { controller = "SearchBox", action = "CommonSearches" }
           );

            routes.MapRoute(
                name: "SearchList_ChangePage",
                url: "Search/DocList/Search-{searchId}/Page{page}/{sort}/{sortDir}",
                defaults: new { controller = "Search", action = "DocList", page = UrlParameter.Optional, sort = "rel", sortDir = "" },
                constraints: new { searchId = @"\d+" }
            );

            //routes.MapRoute(
            //    name: "SearchList_ChangeFolders",
            //    url: "Search/SearchList/Search-{searchId}/Folders-{folderIds}",
            //    defaults: new { controller = "Search", action = "SearchList", folderIds = UrlParameter.Optional },
            //    constraints: new { searchId = @"\d+" }
            //);

            routes.MapRoute(
                name: "List",
                url: "Search/List/Search-{searchId}",
                defaults: new { controller = "Search", action = "List" },
                constraints: new { searchId = @"\d+" }
            );

            routes.MapRoute(
                name: "SetFilterClassifier",
                url: "Search/SetFilterClassifier/Search-{searchId}/{classifierTypeId}/{id}",
                defaults: new { controller = "Search", action = "SetFilterClassifier" },
                constraints: new { searchId = @"\d+" }
            );

            routes.MapRoute(
                name: "RemoveFilterClassifier",
                url: "Search/RemoveFilterClassifier/Search-{searchId}/{classifierTypeId}/{id}",
                defaults: new { controller = "Search", action = "RemoveFilterClassifier" },
                constraints: new { searchId = @"\d+" }
            );

            //routes.MapRoute(
            //    name: "GetFilterClassifier",
            //    url: "Search/GetFilterClassifier/Search-{searchId}/{classifierTypeId}",
            //    defaults: new { controller = "Search", action = "GetFilterClassifier" },
            //    constraints: new { searchId = @"\d+" }
            //);

            routes.MapRoute(
                name: "GetFilterClassifiers",
                url: "Search/GetFilterClassifiers/Search-{searchId}",
                defaults: new { controller = "Search", action = "GetFilterClassifiers" },
                constraints: new { searchId = @"\d+" }
            );

            routes.MapRoute(
                name: "SearchByFilter",
                url: "Search/ByFilter/{classifierId}",
                defaults: new { controller = "Search", action = "ByFilter" }
            );

            routes.MapRoute(
                name: "HomeSearch",
                url: "Search/HomeSearch/{folderId}",
                defaults: new { controller = "Search", action = "HomeSearch" }
            );

            routes.MapRoute(
                name: "SearchDocInLinks",
                url: "Search/DocInLinks/{langId}/{toDocLangId}/{toParId}/{linkIdsString}",
                defaults: new { controller = "Search", action = "DocInLinks", toParId = UrlParameter.Optional, linkIdsString = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SearchDocInLinksFromApi",
                url: "DocInLinks/{docPrefLangId}/{sitelangId}/{domain}/{docCelex}/{toPar}",
                defaults: new { controller = "Search", action = "DocInLinksFromApi", toPar = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "SearchByXmlId",
              url: "SearchByXmlId/{xmlId}/{domain}/{langId}/{siteLangId}",
              defaults: new { controller = "Search", action = "SearchByXmlId" }
          );

            #endregion

            routes.MapRoute(
                name: "RecentDocs",
                url: "RecentDocuments/{action}/{pinned}/{period}/{docType}/{orderBy}/{orderDir}",
                defaults: new { controller = "RecentDocuments", action = "Index", pinned = "", docType = "", period = 0, orderBy = "open_date", orderDir = "desc" }
            );

            routes.MapRoute(
                name: "ParHint",
                url: "Doc/ParHint/{linkType}/Lang{LangIdFromDoc}/{toDocNumber}/{toPar}",
                defaults: new { controller = "Doc", action = "ParHint", toDocNumber = "", toPar = UrlParameter.Optional, lastConsWithText = false },
                constraints: new { LangIdFromDoc = @"\d+" }
            );

            routes.MapRoute(
                name: "ParHintLastCons",
                url: "Doc/ParHint/{linkType}/LastConsTxt/Lang{LangIdFromDoc}/{toDocNumber}/{toPar}",
                defaults: new { controller = "Doc", action = "ParHint", toDocNumber = "", toPar = UrlParameter.Optional, lastConsWithText = true },
                constraints: new { LangIdFromDoc = @"\d+" }
            );

            routes.MapRoute(
                name: "ParHintById",
                url: "Doc/ParHintById/{id}/{articleEid}",
                defaults: new { controller = "Doc", action = "ParHintById", articleEid = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ModHint",
                url: "Doc/ModHint/{fromDocNumber}/{LangIdFromDoc}/{toDocNumber}/{modType}",
                defaults: new { controller = "Doc", action = "ModHint" },
                constraints: new { LangIdFromDoc = @"\d+" }
            );

            routes.MapRoute(
                name: "DocHintByIdentifier",
                url: "Doc/DocHintByIdentifier/{identifier}/{toPar}",
                defaults: new { controller = "Doc", action = "DocHintByIdentifier", toPar = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ActBase",
                url: "Doc/Act/{docPrefLangId}/{DocNumber}/{toPar}",
                defaults: new { controller = "Doc", action = "Act", toPar = UrlParameter.Optional, lastCons = false },
                constraints: new { docPrefLangId = @"\d+" }
            );

            routes.MapRoute(
                name: "ActBaseLastCons",
                url: "Doc/Act/LastCons/{docPrefLangId}/{DocNumber}/{toPar}",
                defaults: new { controller = "Doc", action = "Act", toPar = UrlParameter.Optional, lastCons = true },
                constraints: new { docPrefLangId = @"\d+" }
            );

            routes.MapRoute(
                name: "ActByIdentifier",
                url: "Doc/ActByIdentifier/{guid}/{toPar}",
                defaults: new { controller = "Doc", action = "ActByIdentifier", toPar = UrlParameter.Optional }
            );

            /*   routes.MapRoute(
                   name: "Doc",
                   url: "{controller}/{action}/{id}/{siteSearchId}",
                   defaults: new { controller = "Doc", action = "CourtAct", siteSearchId = UrlParameter.Optional }
               );*/

            routes.MapRoute(
                name: "DocText",
                url: "Doc/{action}/{id}/{siteSearchId}",
                defaults: new { controller = "Doc", action = "CourtAct", siteSearchId = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "LoginIndex",
               url: "Login/Index/{forceLogin}",
               defaults: new { controller = "Login", action = "Index", forceLogin = UrlParameter.Optional }
               );

            routes.MapRoute(
             name: "Logout",
             url: "Login/Logout/{forceLogin}",
             defaults: new { controller = "Login", action = "Logout", forceLogin = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "ResetPassword",
                url: "Login/ResetPassword/{code}",
                defaults: new { controller = "Login", action = "ResetPassword", code = UrlParameter.Optional }
                );


            routes.MapRoute(
                name: "ActivateUser",
                url: "Login/Activate/{token}",
                defaults: new { controller = "Login", action = "Activate"}
                );

            routes.MapRoute(
                name: "LoginUsername",
                url: "Login/LoginUsername/{username}",
                defaults: new { controller = "Login", action = "LoginUsername" }
            );

            routes.MapRoute(
             name: "LoginUsernamePasswordHash",
             url: "Login/LoginUsernamePasswordHash/{username}/{passwordHash}",
             defaults: new { controller = "Login", action = "LoginUsernamePasswordHash" }
         );

            /* Used by web.apis.bg to get user products and hashed password information */
            routes.MapRoute(
                name: "GetUserInformationRemote",
                url: "GetUserInformation/{username}",
                defaults: new { controller = "Login", action = "GetUserInformation" }
                );

            //routes.MapRoute(
            //    name: "ReturnUrl",
            //    url: "{controller}/{action}/?returnUrl={returnUrl}",
            //    defaults: new { controller = "Login", action = "Index"}
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
