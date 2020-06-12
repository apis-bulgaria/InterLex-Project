namespace Interlex.BusinessLayer.Models
{
    using System.Collections.Generic;
    using Interlex.DataLayer;
    using System.Linq;

    public class LanguagePreferences
    {
        public class LanguagePreferencesUpdateModel 
        {
            public int LangNumber { get; set; }

            public int Position { get; set; }
        }

        public IList<PrefStruct> Languages
        {
            get;
            set;
        }

        public struct PrefStruct
        {
            public int PrefPos { get; set; }

            public Language lang { get; set; }
        }

        public LanguagePreferences(int userId)
        {
            IEnumerable<System.Data.IDataRecord> preferences = DB.GetLanguagePreferences(userId);

            this.Languages = new List<PrefStruct>();

            foreach (var record in preferences)
            {
                var curLang = InterfaceLanguages.GetLanguageById(int.Parse(record["lang"].ToString()));

                if (curLang == null)
                {
                    continue;
                }
                else
                {
                    this.Languages.Add(new PrefStruct
                    {
                        PrefPos = int.Parse(record["ord"].ToString()),
                        lang = curLang
                    });
                }

            }

            this.Languages = this.Languages.OrderBy(p => p.PrefPos).ToList();
        }

        public void UpdateLanguagePreferences(int userId, List<LanguagePreferencesUpdateModel> preferences)
        {
            var preferencesAsIntegersArray = new int[50,2]; //if languages become far too many - increase first dimension's size

            for (int i = 0; i < preferences.Count; i++)
            {
                preferencesAsIntegersArray[i, 0] = preferences[i].LangNumber;
                preferencesAsIntegersArray[i, 1] = preferences[i].Position;
            }

            DB.UpdateLanguagePreferences(userId, preferencesAsIntegersArray);
        }
    }
}
