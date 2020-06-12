namespace Interlex.App.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public static class EuroCasesClearExtensions
    {
        /// <summary>
        /// Searchbox clear helper
        /// </summary>
        /// <param name="clearType">Input clear type</param>
        /// <param name="clearableElementId">Id for the input element that is about to be cleared.</param>
        /// <returns></returns>
        public static EuroCasesClear EUCSClear(this HtmlHelper html, SearchBoxClearType clearType, string clearableElementId)
        {
            return new EuroCasesClear(clearType, clearableElementId);
        }
    }
}