using System.Collections.Generic;
namespace EUCases.ConsumerCasesWebApp.BusinessLayer.Models
{
    public static class Language
    {
        public static List<LanguageStruct> Languages
        {
            get;
            set;
        }

        public struct LanguageStruct
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Text { get; set; }
        }

        public static LanguageStruct GetLanguageById(int languageId)
        {
            return Languages.FirstOrDefault(m => m.Id == languageId);
        }

        public static LanguageStruct GetLanguageByCode(string languageCode)
        {
            return Languages.FirstOrDefault(m => m.Code == languageCode);
        }


        public Language()
        {
            Languages = new List<LanguageStruct>();
        }
    }
}