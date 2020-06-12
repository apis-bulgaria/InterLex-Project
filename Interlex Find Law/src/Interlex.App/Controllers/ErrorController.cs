using System.Web.Mvc;

namespace Interlex.App.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        [ActionName("404")]
        public ActionResult NotFound(string aspxerrorpath)
        {
            ViewBag.path = aspxerrorpath;
            Response.StatusCode = 404;

            return View();
        }

        [ActionName("500")]
        public ActionResult ServerError()
        {
            return View("~/Views/Error/500.cshtml");
        }
    }
}
