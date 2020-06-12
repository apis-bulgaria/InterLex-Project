namespace Interlex.App.Api
{
    using Interlex.App.Resources;
    using System.Collections.Generic;
    using System.Web.Http;

    public class ResourcesController : BaseApiController
    {
        [HttpGet]
        public IDictionary<string, string> ParTitles()
        {
            return Res.GetParTitles(typeof(Resources));
        } 
    }
}