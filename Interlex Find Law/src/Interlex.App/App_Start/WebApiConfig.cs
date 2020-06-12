using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;


namespace Interlex.App
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            #region Doc Controller

            config.Routes.MapHttpRoute(
                name: "ApiHint",
                routeTemplate: "api/Doc/ParHint/{langId}/{siteLangId}/{docNumber}/{toPar}",
                defaults: new { controller = "Doc", action = "ParHint", toPar = UrlParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ApiDocInLinks",
                routeTemplate: "api/Doc/DocInLinks/{domain}/{langId}/{siteLangId}/{limit}/{docNumber}/{toPar}",
                defaults: new { controller = "Doc", action = "DocInLinks", toPar = UrlParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ApiSearchByTerm",
                routeTemplate: "api/Doc/SearchByTerm/{domain}/{langId}/{siteLangId}",
                defaults: new { controller = "Doc", action = "SearchByTerm"}
            );

            config.Routes.MapHttpRoute(
                name: "ApiSearchByXmlId",
                routeTemplate: "api/Doc/SearchByXmlId/{xmlId}/{domain}/{langId}/{siteLangId}",
                defaults: new { controller = "Doc", action = "SearchByXmlId" }
            );

            config.Routes.MapHttpRoute(
                name: "Cite",
                routeTemplate: "api/Doc/Cite/{langId}/{docNumber}/{citeType}",
                defaults: new { controller = "Doc", action = "Cite" }
            );

            #endregion

            config.Routes.MapHttpRoute(
                name: "ContentsApi",
                routeTemplate: "api/Entity/DocContents/{docLangId}",
                defaults: new { controller = "Entity", action = "DocContents" }
            );

            config.Routes.MapHttpRoute(
                name: "EntityApi",
                routeTemplate: "api/{controller}/{action}/{classifierType}/{parentId}/{tab}/{searchId}",
                defaults: new { classifierType = "", parentId = UrlParameter.Optional, tab = UrlParameter.Optional, searchId = UrlParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ApiDefault",
                routeTemplate: "api/{controller}/{action}/{clientSelectedIds}",
                defaults: new { }
            );

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.EnableCors();
        }
    }
}
