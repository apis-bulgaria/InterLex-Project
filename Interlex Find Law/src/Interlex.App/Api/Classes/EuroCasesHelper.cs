using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apis.Common.Classes.Common;

namespace Interlex.App.Api.Classes
{
    internal static class EuroCasesHelper
    {
        internal static String BuildAllUrlForSearchByXmlId(String baseUrl, String xmlId, String domain, int langId, int uiLangId)
        {
            var url = ApisPathHelper.Combine(baseUrl, "SearchByXmlId", xmlId, domain, langId.ToString(), uiLangId.ToString());

            return url;
        }

        internal static String BuildAllUrlForDocInLinks(String baseUrl, int langId, int uiLangId, String domain, String docNumber, String toPar)
        {
            var url = ApisPathHelper.Combine(baseUrl, "DocInLinks", langId.ToString(), uiLangId.ToString(), domain, docNumber);

            if (!String.IsNullOrEmpty(toPar))
            {
                url = ApisPathHelper.Combine(url, toPar);
            }

            return url;
        }
    }
}