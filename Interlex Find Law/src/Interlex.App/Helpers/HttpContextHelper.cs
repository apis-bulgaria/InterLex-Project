using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Interlex.App.Helpers
{
    public class HttpContextHelper
    {
        public static string GetClientIPAddress()
        {
            string clientIP = String.Empty;
            var request = HttpContext.Current.Request;
            string headerName = ConfigurationManager.AppSettings["RequestIPHeaderName"];

            var reqHeaderName = request.Headers.AllKeys.Where(x => x.ToLower() == headerName.ToLower()).FirstOrDefault();
            if (reqHeaderName != null)
            {
                HttpContext.Current.Response.Write($"header {headerName} found");
                clientIP = request.Headers[reqHeaderName];
            }
            else
            {
                HttpContext.Current.Response.Write($"header {headerName} not found");
                clientIP = request.UserHostAddress;
            }

            return clientIP;
        }
    }
}