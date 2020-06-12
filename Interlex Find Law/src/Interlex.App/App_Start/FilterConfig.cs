using System.Web;
using System.Web.Mvc;
using Interlex.App.Filters;

namespace Interlex.App
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UserAuthorize());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
