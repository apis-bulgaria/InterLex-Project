using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.App.Helpers
{
    public class WebAppHelper
    {
        public static string AppRootFolder
        {
            get
            {
                try
                {
                    string appRoot = HttpContext.Current.Request.ApplicationPath;
                    if (appRoot == "/")
                        return "";
                    return appRoot;
                }
                catch
                { }
                return "";
            }
        }

    }
}