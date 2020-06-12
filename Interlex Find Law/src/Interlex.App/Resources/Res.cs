using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace Interlex.App.Resources
{
    public class Res
    {
        public static string GetCountryNameByCode(string code)
        {
            string name = "";
            switch (code.ToUpper())
            {
                case "EU":
                    name = "European union"; //todo
                    break;
                case "UK":
                case "GB":
                    name = Resources.UI_UnitedKingdom;
                    break;
                case "BG":
                    name = Resources.UI_Bulgaria;
                    break;
                case "DE":
                    name = Resources.UI_Germany;
                    break;
                case "FR":
                    name = Resources.UI_France;
                    break;
                case "AT":
                    name = Resources.UI_Austria;
                    break;
            }
            return name;
        }

        public static string GetResource(string key)
        {
            string result = "";
            try
            {
                result = Resources.ResourceManager.GetString(key);
                if (result == null)
                    result = key;
            }
            catch
            {
                result = key;
            }
            return result;
        }

        public static string GetResource(string key, string code)
        {
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;

            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}", code));
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(string.Format("{0}", code));

            //string result = Res.GetResource(key);

            //Thread.CurrentThread.CurrentCulture = ci;
            //Thread.CurrentThread.CurrentUICulture = ci;

            string result = "";
            try
            {
                result = Resources.ResourceManager.GetString(key, CultureInfo.GetCultureInfo(string.Format("{0}", code)));
                if (result == null)
                    result = key;
            }
            catch
            {
                result = key;
            }
            return result;
        }

        public static IDictionary<String, String> GetParTitles(Type resources)
        {
            IDictionary<String, String> rubrics = resources.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Select(p => new { Key = p.Name, Value = p.GetValue(null) })
                                    .Where(a => a.Key.StartsWith("UI_ParTitle_", StringComparison.OrdinalIgnoreCase))
                                    .Where(a => a.Value is String)
                                    .Select(a => new { Key = a.Key, Value = a.Value as String })
                                    .ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);
            return rubrics;
        }
    }
}