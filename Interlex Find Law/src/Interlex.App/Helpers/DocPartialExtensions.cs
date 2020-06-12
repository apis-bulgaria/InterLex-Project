using System.Web.Mvc;
using System.Web.Mvc.Html;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer;

namespace Interlex.App.Helpers
{
    public static class DocPartialExtensions
    {
        public static MvcHtmlString DocPartial(this HtmlHelper helper, Document model, int langId, string appRootFolder, bool linksInNewTab)
        {
            return helper.DocPartial(model, langId, appRootFolder, linksInNewTab, null /* View Data */);
        }

        public static MvcHtmlString DocPartial(this HtmlHelper helper, Document model, int langId, string appRootFolder, bool linksInNewTab, ViewDataDictionary viewData)
        {
            var rewriter = new DocLinksRewriter(langId, appRootFolder, linksInNewTab);
            model.RewriteLinksInDocList(rewriter);
            return helper.Partial("~/Views/Shared/_Doc.cshtml", model, viewData);
        }
    }
}