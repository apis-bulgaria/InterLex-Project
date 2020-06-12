using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Interlex.BusinessLayer.Models;
using System.Configuration;

namespace Interlex.App
{
    public abstract class BaseApiController : ApiController
    {
        public BaseApiController()
        {
            //if (HttpContext.Current.Session == null || HttpContext.Current.Session["UserId"] == null)
            //{
            //    string acceptTypeJSON = HttpContext.Current.Request.AcceptTypes.FirstOrDefault(s => s.Contains("application/json") || s.Contains("*/*"));
            //    if (!String.IsNullOrEmpty(acceptTypeJSON))
            //    {
            //        HttpContext.Current.Response.Write("{ \"status\": \"unauth\" }"); // json
            //        HttpContext.Current.Response.End();
            //    }
            //}
        }

        private Language _language;
        public Language Language
        {
            get
            {
                //return Session["LanguageId"] == null ? null : Session["LanguageId"].ToString();
                //return Convert.ToInt32(Session["LanguageId"]);
                if (_language == null)
                {
                    if (HttpContext.Current.Session["LanguageId"] != null)
                        this._language = InterfaceLanguages.GetLanguageById(Convert.ToInt32(HttpContext.Current.Session["LanguageId"]));
                    else
                    {
                        this._language = InterfaceLanguages.GetLanguageById(Convert.ToInt32(ConfigurationManager.AppSettings["DefaultLanguageId"]));
                    }
                }
                return _language;
            }
        }


        //public string LanguageId
        //{
        //    get
        //    {
        //        return HttpContext.Current.Session["LanguageId"] == null ? null : HttpContext.Current.Session["LanguageId"].ToString();
        //    }
        //    //set
        //    //{
        //    //    HttpContext.Current.Session["LanguageId"] = value;
        //    //    if (value != null)
        //    //    {
        //    //        CultureInfo ci = new CultureInfo(value);
        //    //        System.Threading.Thread.CurrentThread.CurrentCulture = ci;
        //    //        System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
        //    //    }
        //    //}
        //}
    }
}