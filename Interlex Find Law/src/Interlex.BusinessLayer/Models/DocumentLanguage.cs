namespace EUCases.ConsumerCasesWebApp.BusinessLayer.Models
{
    using System.Collections.Generic;

    public static class DocumentLanguage
    {
        public static List<Language> Languages
        {
            get;
            set;
        }

        static DocumentLanguage()
        {
            Languages = CacheProvider.Provider.GetOrSet("document_langs", () => PopulateLangs(), 1440);
        }

        public static Language GetLanguageById(int languageId)
        {
            return Languages.FirstOrDefault(m => m.Id == languageId);
        }
    }
}
