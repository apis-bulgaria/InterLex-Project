namespace Interlex.App.Helpers
{
    using System;
    using System.Web;
    using Interlex.BusinessLayer.Models;

    public static class SessionExtensions
    {
        /// <summary>
        /// Checks if the 'SelectedProductId' key in the session is equal to 2
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsCurrentProductEuFins(this HttpSessionStateBase state)
        {
            int parsed;
            var isParsed = int.TryParse(state["SelectedProductId"].ToString(), out parsed);
            return parsed == 2;
        }

        public static int LanguageId(this HttpSessionStateBase state)
        {
            return Convert.ToInt32(state["LanguageId"]);
        }

        public static UserData UserData(this HttpSessionStateBase state)
        {
            return state["UserData"] as UserData;
        }
    }
}