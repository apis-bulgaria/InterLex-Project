using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Enums;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Interlex.App.Filters;

namespace Interlex.App
{
    [AllowAnonymous]
    public abstract class BaseController : Controller
    {
        protected String NationalLegislationMapPath => this.Server.MapPath("~/Content/FileStructure/nationalLegislationMap.txt");

        private UserData _userData;
        public UserData UserData
        {
            get
            {
                if (this._userData == null)
                    this._userData = Session["UserData"] == null ? null : (UserData)Session["UserData"];
                return _userData;
            }
        }

        public bool CheckRights
        {
            get
            {
                if (this.UserData.UserType == UserTypes.User || this.UserData.UserType == UserTypes.LocalUser)
                    return true;
                else
                    return false;
            }
        }

        private int _productId;
        public int ProductId
        {
            get
            {
                if (this._productId == 0)
                    this._productId = Convert.ToInt32(Session["SelectedProductId"]);
                return this._productId;
            }
        }

        public ContentResult CamelCaseJson(Object obj, JsonRequestBehavior requestBehaviour = JsonRequestBehavior.AllowGet)
        {
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

            JsonResult result = new JsonResult();

            result.Data =
                result.JsonRequestBehavior = requestBehaviour;

            ContentResult contentResult = new ContentResult();
            contentResult.Content = JsonConvert.SerializeObject(obj, camelCaseFormatter);
            contentResult.ContentType = "application/json";

            return contentResult;
        }

        
        protected override void ExecuteCore()
        {
            Language currentLang = InterfaceLanguages.GetLanguageById(Convert.ToInt32(Session["LanguageId"]));
            base.ExecuteCore();
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //string controllerName = filterContext.Controller.GetType().Name;
            //string actionName = filterContext.ActionDescriptor.ActionName;

            //if (Session["UserData"] == null && !filterContext.IsChildAction)
            //{
            //    if (!controllerName.Equals("LoginController", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        HttpContextBase reqContext = filterContext.RequestContext.HttpContext;
            //        if (reqContext.Request.IsAjaxRequest())
            //        {
            //            reqContext.Response.ContentType = "application/json";
            //            reqContext.Response.Clear();
            //            string acceptTypeJSON = reqContext.Request.AcceptTypes.FirstOrDefault(s => s.Contains("application/json") || s.Contains("*/*"));
            //            if (!String.IsNullOrEmpty(acceptTypeJSON))
            //            {
            //                reqContext.Response.Write("{ \"status\": \"unauth\" }"); // json
            //                reqContext.Response.End();
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            if (!reqContext.Response.IsRequestBeingRedirected)
            //                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
            //        }
            //    }
            //}

            base.OnActionExecuting(filterContext);
        }

        private Language _language;
        public Language Language
        {
            get
            {
                if (_language == null)
                {
                    this._language = InterfaceLanguages.GetLanguageById(Convert.ToInt32(this.Session["LanguageId"]));
                }
                return _language;
            }
        }

        public string GetViewString(string viewName, object model, bool isForExport)
        {
            if (isForExport)
            {
                ViewBag.IsForExport = "Export";
            }
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public bool ViewExists(string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, name, null);
            return (result.View != null);
        }
        
      /*  protected bool CheckFreeDocsCookie()
        {
            /*if (Session != null && this.UserData != null)
            {
                if (this.UserData.Username.ToLower() == "sysdemo")
                {
                    return true;
                }
                else if (Request.Cookies["include_free_cases_docs"] != null &&
                    Request.Cookies["include_free_cases_docs"].Value != null &&
                    int.Parse(Request.Cookies["include_free_cases_docs"].Value) == 1) // free documents included
                {
                    return true;
                }

                return false;

            return this.UserData.ShowFreeDocuments;
        }*/
    }
}
