namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Interlex.DataLayer;

    public class Language
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ShortCode { get; set; }
        public string Text { get; set; }
        public bool IsInterfaceLang { get; set; }
    }

    public static class InterfaceLanguages
    {
        public static List<Language> Languages
        {
            get;
            set;
        }

     /*   public static List<Language> DocumentLanguages
        {
            get;
            set;
        }*/

        static InterfaceLanguages()
        {
            Languages = CacheProvider.Provider.GetOrSet("interface_langs", () => PopulateLangs(), 1440);
           // DocumentLanguages = CacheProvider.Provider.GetOrSet("document_langs", () => PopulateDocumentLangs(), 1440);
        }

        public static Language GetLanguageById(int languageId)
        {
            return Languages.FirstOrDefault(m => m.Id == languageId);
        }

        public static Language GetLanguageByCode(string languageCode)
        {
            return Languages.FirstOrDefault(m => m.Code == languageCode);
        }

        public static Language GetLanguageByShortCode(string shortCode)
        {
            return Languages.FirstOrDefault(m => m.ShortCode == shortCode);
        }

     /*   public static Language GetDocumentLanguageById(int languageId)
        {
            return DocumentLanguages.FirstOrDefault(m => m.Id == languageId);
        }

        public static Language GetDocumentLanguageByCode(string languageCode)
        {
            return DocumentLanguages.FirstOrDefault(m => m.Code == languageCode);
        }

        public static Language GetDocumentLanguageByShortCode(string shortCode)
        {
            return DocumentLanguages.FirstOrDefault(m => m.ShortCode == shortCode);
        }*/

        private static List<Language> PopulateLangs()
        {
            var langsFromDB = DB.GetLangs();

            var langs = new List<Language>();

            foreach (var lang in langsFromDB)
            {
                var curLang = new Language
                {
                    Id = int.Parse(lang["id"].ToString()),
                    Code = lang["code"].ToString(),
                    ShortCode = lang["short_lang"].ToString(),
                    Text = lang["name"].ToString(),
                    IsInterfaceLang = Convert.ToBoolean(lang["is_site_lang"])
                };

                langs.Add(curLang);
            }

            return langs;
        }

      /*  private static List<Language> PopulateDocumentLangs()
        {
            var langsFromDB = DB.GetLangs();

            var langs = new List<Language>();

            foreach (var lang in langsFromDB)
            {
                var curLang = new Language
                {
                    Id = int.Parse(lang["id"].ToString()),
                    Code = lang["code"].ToString(),
                    ShortCode = lang["short_lang"].ToString(),
                    Text = lang["name"].ToString()
                };

                langs.Add(curLang);
            }

            return langs;
        }*/
    }
}