using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Models;
using System.Globalization;
using System.Configuration;

namespace Interlex.App.Filters
{
    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            base.AuthorizeCore(httpContext);

            if (httpContext.Session["UserData"] == null)
            {
                string controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
                string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
                if (controller != "Login" && controller != "ContextMenus" && controller != "ApplicationUpdate" && controller != "Error" && controller != "Blog" && controller != "Sitemap")
                {
                    if (httpContext.Request.IsAjaxRequest())
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.Clear();
                        string acceptTypeJSON = httpContext.Request.AcceptTypes.FirstOrDefault(s => s.Contains("application/json") || s.Contains("*/*"));
                    }
                    else
                    {
                        if (!httpContext.Response.IsRequestBeingRedirected)
                        {
                            string returnUrl = httpContext.Request.Url.PathAndQuery;
                            httpContext.Response.Redirect("~/Login/Index?returnUrl=" + HttpUtility.UrlEncode(returnUrl), true);
                            httpContext.Response.End();
                        }
                    }
                    return false;
                }
                else
                    return true;
            }
            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var controller = filterContext.RequestContext.RouteData.GetRequiredString("controller");
            var action = filterContext.ActionDescriptor.ActionName;

            bool sessionExists = false;
            if (filterContext.HttpContext.Session["UserData"] != null)
            {
                sessionExists = UserMng.UpdateUserSession(
                            ((UserData)filterContext.HttpContext.Session["UserData"]).SessionId,
                            filterContext.HttpContext.Session.Timeout * 60);
            }

            if (!sessionExists)
            {
                // Anonymous controllers list
                List<string> anonymousControllers = new List<string>();
                anonymousControllers.Add("Login");
                anonymousControllers.Add("ContextMenus");
                anonymousControllers.Add("ApplicationUpdate");
                anonymousControllers.Add("Blog");
                anonymousControllers.Add("Presentation");

                if (!anonymousControllers.Contains(controller))
                {
                    filterContext.HttpContext.Session.Abandon();
                    HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.ContentType = "application/json";
                    filterContext.HttpContext.Response.Clear();
                    string acceptTypeJSON = filterContext.HttpContext.Request.AcceptTypes.FirstOrDefault(s => s.Contains("application/json") || s.Contains("*/*"));
                    if (!String.IsNullOrEmpty(acceptTypeJSON))
                    {
                        JsonResult UnauthorizedResult = new JsonResult();
                        UnauthorizedResult.Data = "{ \"status\" : \"unauth\" }";
                        UnauthorizedResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = UnauthorizedResult;
                    }
                }
                else
                {
                    if (!filterContext.HttpContext.Response.IsRequestBeingRedirected)
                    {
                        string returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery;
                        filterContext.Result = new RedirectResult("~/Login/Index?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
                    }
                }
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}