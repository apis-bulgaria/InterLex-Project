namespace Interlex.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Models;
    using Interlex.App.Filters;
    using System.Configuration;

    [UserAuthorize]
    public class SearchBoxController : BaseController
    {
        //[HttpGet] Reason: Hope to help Marek work on XP+Firefox
        public PartialViewResult Show(int? searchId)
        {
            SearchBox sb = null;

            // Load SearchBox criteria for searchId
            if (searchId.HasValue && Session["SearchResults"] != null)
            {
                SearchResult sr = SearchResult.FindSearchResult(searchId.Value, Session["SearchResults"]);
                if (sr != null)
                    sb = sr.SearchBoxFilters;
            }
            else
            {
                // empty SearchBox
                sb = new SearchBox(this.Language.Id)
                {
                    SearchText = ""
                };
            }
            return PartialView("_SearchBox", sb);
        }

        [HttpPost]
        public JsonResult CommonSearches(string like) 
        {
            int curProductId = 1;
            if (Session["SelectedProductId"] != null)
            {
                curProductId = int.Parse(Session["SelectedProductId"].ToString());
            }

            var commonSearches = Stat.GetStatSearches(like, curProductId);

            commonSearches.OrderBy(e => e.Value);

            var commonSearchesList = new List<string>();

            foreach (var item in commonSearches)
            {
                commonSearchesList.Add(item.Key);
            }

            return Json(commonSearchesList);
        }

        [HttpPost]
        public JsonResult UserSearches(string like) 
        {
            var userId = UserData.UserId;
            int curProductId = 1;
            if (Session["SelectedProductId"] != null)
            {
                curProductId = int.Parse(Session["SelectedProductId"].ToString());
            }

            var searchesFromDB = Interlex.BusinessLayer.Models.UserSearches.GetTopSearches(userId, like, curProductId);
            
            var userSearches = new List<string>();

            foreach (var search in searchesFromDB)
            {
                userSearches.Add(search["txt"].ToString());
            }

            return Json(userSearches);
        }

    }
}
