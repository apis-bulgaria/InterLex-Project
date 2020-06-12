using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Interlex.App.Filters;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Entities;
using Interlex.BusinessLayer.Enums;
using Interlex.BusinessLayer.Models;
using System.Web.Routing;

namespace Interlex.App.Controllers
{
    [UserAuthorize]
    public class RecentDocumentsController : BaseController
    {
        public ActionResult Index()
        {
            if (UserData.Username.ToLower() == "sysdemo")
            {
                if (ProductId == 1) // EuroCases
                {
                    return RedirectToAction("ProductFeaturesInfo", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
                }
                else // Tax & Financial Standarts
                {
                    return RedirectToAction("ProductFeaturesInfoFinances", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
                }
            }

            return View();
        }

        public ActionResult List(bool? pinned, int? docType, RecentDocDatePeriod period, string orderBy, string orderDir)
        {
            int curProductId = 1;
            if (Session["SelectedProductId"] != null)
            {
                curProductId = int.Parse(Session["SelectedProductId"].ToString());
            }

            RecentDocuments rd = Doc.GetRecentDocs(this.UserData.UserId, Language.Id, pinned, docType, period, orderBy, orderDir, curProductId);

            return PartialView(rd);
        }

        public void Unpin(int id)
        {
            Doc.SetRecentDocPin(id, false);
        }

        public void Pin(int id)
        {
            Doc.SetRecentDocPin(id, true);
        }


        [HttpPost]
        public ActionResult DeleteAll()
        {
            int userid = int.Parse(UserData.UserId.ToString());
        
            try
            {
                UserSearches.DelAllUserRecentDocuments(userid);
                return Json("Ok");
            }
            catch
            {
                return Json("Failed");
            }
        }
    }
}
