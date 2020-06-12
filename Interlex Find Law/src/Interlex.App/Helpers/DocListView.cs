using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Interlex.BusinessLayer;
using System.Collections.Generic;

namespace Interlex.App.Helpers
{
    public class DocListView : RazorView
    {
        public DocListView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator)
        : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, viewPageActivator)
        {

        }

        protected override void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {
            var writerToString = writer.ToString();

            base.RenderView(viewContext, writer, instance);

            writerToString = writer.ToString(); ;

            var view = (BuildManagerCompiledView)viewContext.View;
            var context = viewContext.HttpContext;
            var path = context.Server.MapPath(view.ViewPath);
            var viewName = Path.GetFileNameWithoutExtension(path);
            
            if (viewName != "Index" && viewName != "_SearchBox" && viewName != "UserMenu")
            {
                var c = 5;
            }

          //  var controller = viewContext.RouteData.GetRequiredString("controller");
        }
    }
}