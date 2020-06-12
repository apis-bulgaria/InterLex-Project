using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Interlex.BusinessLayer.Models;
using Interlex.App.Filters;

namespace Interlex.App.Controllers
{
    [UserAuthorize]
    public class ContextMenusController : BaseController
    {
        [ChildActionOnly]
        public ActionResult LanguageMenu()
        {
            //InterfaceLanguages il = new InterfaceLanguages(this.Language.Id);
            //Session["LanguageCode"] = "en-GB";
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult UserMenu()
        {            
            return PartialView();
        }

        public ActionResult MainMenu()
        {
            return View();
        }

    }
}
