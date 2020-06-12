using Interlex.App.Helpers;
using System.Web;
using System.Web.Optimization;

namespace Interlex.App
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
#if !DEBUG
    BundleTable.EnableOptimizations = true;
#endif
            //BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/Scripts/lib/jquery-{version}.js",
                        "~/Content/Scripts/lib/jquery.cookie.js",
                        "~/Content/Scripts/lib/jquery.cookiebar.js",
                        "~/Content/Scripts/lib/jquery.countdownTimer.js",
                        "~/Content/Scripts/lib/jquery-scrollto.js",
                        "~/Content/Scripts/lib/jquery.flot.js",
                        "~/Content/Scripts/lib/jquery.flot.time.js",
                        "~/Content/Scripts/lib/jquery.flot.tooltip.min.js",
                        "~/Content/Scripts/lib/jquery.dataTables.min.js",
                        "~/Content/Scripts/lib/jquery.contextMenu.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        //"~/Content/Scripts/lib/jquery-ui-{version}.js",
                        "~/Content/Scripts/lib/jquery-ui.js",
                        "~/Content/Scripts/lib/jquery.fancytree.js",
                        "~/Content/Scripts/lib/jquery.fancytree.persist.js",
                        "~/Content/Scripts/lib/jquery.splitter-0.14.0.js",
                        "~/Content/Scripts/lib/jquery.scrollTo.js",
                        "~/Content/Scripts/lib/jquery.qtip.js",
                        "~/Content/Scripts/lib/jquery.paginate.js",
                        "~/Content/Scripts/lib/jquery.paging.js", //used in PAL list
                        "~/Content/Scripts/lib/jquery.notify.js",
                        "~/Content/Scripts/lib/jquery.clearsearch.js",
                        "~/Content/Scripts/lib/jquery.highlight.js",
                        "~/Content/Scripts/lib/addclear.js",
                        "~/Content/Scripts/lib/jquery.textfill.js",
                        "~/Content/Scripts/lib/imagesloaded.pkg.min.js",
                        "~/Content/Scripts/lib/jquery.fancytree.dnd.js",
                        "~/Content/Scripts/lib/jquery.easing.1.3.js",
                        "~/Content/Scripts/lib/jquery.jBreadCrumb.1.1.js",
                        "~/Content/Scripts/lib/jquery.dd.js"
                        )); //"~/Content/Scripts/lib/fotorama.js"

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Content/Scripts/SearchBox.js",
                "~/Content/Scripts/SearchBox_clear.js",
                "~/Content/Scripts/app.js",
                "~/Content/Scripts/utility.js",
                "~/Content/Scripts/export.js",
                "~/Content/Scripts/favourites.js",
                "~/Content/Scripts/filters.js",
                "~/Content/Scripts/keywords-summaries.js",
                "~/Content/Scripts/search.js",
                "~/Content/Scripts/lib/underscore.js",
                "~/Content/Scripts/eurocases-clear.js",
                "~/Content/Scripts/eurocases-back-to-top.js",
                "~/Content/Scripts/lib/jquery.tooltipster.min.js",
                "~/Content/Scripts/hint-examples.js",
                "~/Content/Scripts/finances.js",
                "~/Content/Scripts/lib/gridstack.js",
                "~/Content/Scripts/finances.js",
                "~/Content/Scripts/multidict.js",
                "~/Content/Scripts/lib/hammer.min.js",
                "~/Content/Scripts/lib/hammer-time.min.js",
                "~/Content/Scripts/jquery.simplePagination.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/MtJS").Include(
                "~/Content/Scripts/machineTranslation.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Scripts/lib/jquery.validate.min.js",
                        "~/Content/Scripts/lib/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Content/Scripts/lib/bootstrap.min.js",
                "~/Content/Scripts/lib/bootstrap-notify.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/doc").Include(
                "~/Content/Scripts/doc.js",
                "~/Scripts/pdf.js/build/pdf.js",
                "~/Content/Scripts/jquery.fitvids.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/lib/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/LoginCSS").Include(
               //"~/Content/Styles/bootstrap.css",
               // "~/Content/Styles/app.css"));
               ));

            bundles.Add(new StyleBundle("~/Content/MtCSS").Include(
                "~/Content/Styles/machine-translation.css"));

            
            //if(WebAppHelper.AppRootFolder == "")
            //    cssBundle.Include("~/Content/Styles/lib/ui.fancytree.css");
            //else
            //    cssBundle.Include("~/Content/Styles/lib/ui.fancytree_FindLaw.css");
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/Styles/lib/jquery-ui.css",
                "~/Content/Styles/lib/jquery.qtip.css",
                "~/Content/Styles/lib/jquery.splitter.css",
                "~/Content/Styles/lib/jquery.cookiebar.css",
                // "~/Content/Styles/bootstrap.min.css",
                // "~/Content/Styles/bootstrap-theme.min.css",
                //"~/Content/Styles/app.css",
                "~/Content/Styles/paging.css",
                "~/Content/Styles/lib/ui.notify.css",
                "~/Content/Styles/lib/tooltipster.css",
                "~/Content/Styles/lib/themes/tooltipster-light.css",
                "~/Content/Styles/lib/jquery.contextMenu.css",
                "~/Content/Styles/lib/gridstack.css",
                "~/Content/Styles/lib/gridstack-extra.css",
                "~/Content/Styles/lib/dd.css",
                "~/Content/Styles/simplePagination.css",
                "~/Content/Styles/lib/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/fancytree").Include("~/Content/Styles/lib/ui.fancytree.css"));
            bundles.Add(new StyleBundle("~/Content/fancytree_findlaw").Include("~/Content/Styles/lib/ui.fancytree_FindLaw.css"));

            bundles.Add(new StyleBundle("~/Content/doc").Include(
                "~/Content/Styles/doc.css"));

            bundles.Add(new StyleBundle("~/Content/Styles/lib/jqueryui").Include(
                    "~/Content/Styles/lib/jquery-ui.css",
                    "~/Content/Styles/lib/jquery-ui-structure.css",
                    "~/Content/Styles/lib/jquery-ui.theme.css",
                    "~/Content/Styles/lib/breadcrumbs/BreadCrumb.css"
                ));


            bundles.Add(new StyleBundle("~/Content/Styles/eurocases").Include(
                        "~/Content/Styles/eurocases.css",
                        "~/Content/Styles/utility.css",
                        "~/Content/Styles/layout.css",
                        "~/Content/Styles/login.css",
                        "~/Content/Styles/notifications.css",
                        "~/Content/Styles/spritecrops.css",
                        "~/Content/Styles/doc-list.css",
                        "~/Content/Styles/filters.css",
                        "~/Content/Styles/search.css",
                        "~/Content/Styles/settings.css",
                        "~/Content/Styles/my-searches.css",
                        "~/Content/Styles/nav.css",
                        "~/Content/Styles/doc-top-bar.css",
                        "~/Content/Styles/finances.css",
                         "~/Content/Styles/media.css",
                        "~/Content/Styles/my-documents.css",
                        "~/Content/Styles/login-username-failure.css"));
            //"~/Content/Styles/home.css"

            /*   bundles.Add(new StyleBundle("~/Content/Styles/xslt").Include(
                           "~/Content/Styles/xslt/styleActGeneral.css",
                           "~/Content/Styles/xslt/styleEurolexJudgment.css",
                           "~/Content/Styles/xslt/styleGeneral.css",
                           "~/Content/Styles/xslt/styleJudgmentGeneral.css"));*/
        }
    }
}
