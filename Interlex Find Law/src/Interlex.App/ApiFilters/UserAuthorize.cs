namespace Interlex.App.ApiFilters
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Models;

    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext context)
        {
            bool sessionExists = false;
            if (HttpContext.Current.Session["UserData"] != null)
            {
                sessionExists = UserMng.UpdateUserSession(
                            ((UserData)HttpContext.Current.Session["UserData"]).SessionId,
                            HttpContext.Current.Session.Timeout * 60);
            }

            //if (HttpContext.Current.Session["UserData"] == null)
            if (!sessionExists)
            {
                string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                string action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
                if (controller != "Login")
                {
                    HttpContext.Current.Session.Abandon();

                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Clear();
                    string acceptTypeJSON = HttpContext.Current.Request.AcceptTypes.FirstOrDefault(s => s.Contains("application/json") || s.Contains("*/*"));
                    if (!String.IsNullOrEmpty(acceptTypeJSON))
                    {
                        HttpContext.Current.Response.Write("{ \"status\": \"unauth\" }");
                        HttpContext.Current.Response.End();
                        return false;
                    }
                }
                return false;
            }
            return true;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

    }
}