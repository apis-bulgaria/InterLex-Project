namespace System.Web.Mvc
{
    using Microsoft.Ajax.Utilities;

    public static class MinificationExtensions
    {
        public static MvcHtmlString JsMinify(
            this HtmlHelper helper, Func<object, object> markup)
        {
            string notMinifiedJs =
             (markup.DynamicInvoke(helper.ViewContext) ?? "").ToString();

#if DEBUG
            return new MvcHtmlString(notMinifiedJs);
#endif


            var minifier = new Minifier();
            var minifiedJs = minifier.MinifyJavaScript(notMinifiedJs, new CodeSettings
            {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            });
            return new MvcHtmlString(minifiedJs);
        }
    }
}