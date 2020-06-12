namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interlex.DataLayer;

    public class MultiDictTranslation
    {
        private string langCode;
        private string langShortCode;
        private int correspondingId;
        private string correspondingItemId;

        public int Id { get; set; }

        public int CorrespondingId
        {
            get
            {
                return this.correspondingId;
            }
            private set
            {
                this.correspondingId = value;
            }
        }

        public string CorrespondingItemId
        {
            get
            {
                return this.correspondingItemId;
            }
            private set
            {
                this.correspondingItemId = value;
            }
        }

        public string Text { get; set; }
        public string SearchText { get; set; }

        public int LangId { get; set; }

        public bool Selected { get; set; }

        public string LangShortCode
        {
            get
            {
                return this.langShortCode;
            }
            private set
            {
                this.langShortCode = value;
            }
        }

        public string LangCode
        {
            get
            {
                return this.langCode;
            }
            private set
            {
                this.langCode = value;
            }
        }

        public MultiDictTranslation(int id, string text, int langId, int correspondingId, String searchText)
        {
            this.Id = id;
            this.Text = text;
            this.LangId = langId;
            this.CorrespondingId = correspondingId;

            var langInfo = InterfaceLanguages.GetLanguageById(this.LangId);
            this.LangCode = langInfo.Code;
            this.langShortCode = langInfo.ShortCode;
            this.SearchText = searchText;
        }

        public MultiDictTranslation(int id, string text, int langId, string correspondingItemId, String searchText)
        {
            this.Id = id;
            this.Text = text;
            this.LangId = langId;
            this.correspondingItemId = correspondingItemId;

            var langInfo = InterfaceLanguages.GetLanguageById(this.LangId);
            this.LangCode = langInfo.Code;
            this.langShortCode = langInfo.ShortCode;
            this.SearchText = searchText;

        }

        /*  public static IEnumerable<MultiDictTranslation> GetMultiDictItemTranslations(string itemId)
          {
              var items = new List<MultiDictTranslation>();

              foreach (var item in DB.GetMultiDictTranslations(itemId))
              {
                  var curItem = new MultiDictTranslation(int.Parse(item["id"].ToString()), item["translation_value"].ToString(), Convert.ToInt32(item["lang_id"]), itemId);
                  items.Add(curItem);
              }

              return items;
          }*/

        public static IEnumerable<MultiDictTranslation> GetMultiDictItemTranslations(string itemId)
        {
            var items = new List<MultiDictTranslation>();

            var allMultiDictItems = CacheProvider.Provider.GetOrSetForever("multilingual_dictionary", () => MultiDictItem.GetAllMultiDictItems());

            foreach (var item in allMultiDictItems)
            {
                var curDictEntry = item.Value.Where(i => i.ItemId == itemId).FirstOrDefault();

                if (curDictEntry != null)
                {
                    var curTranslationItem = new MultiDictTranslation(0, curDictEntry.Text, curDictEntry.LangId, itemId, curDictEntry.SearchText);
                    items.Add(curTranslationItem);
                }
            }

            var externalOrderingList = InterfaceLanguages.Languages.Where(l => l.IsInterfaceLang).Select(l => l.Id).Reverse().ToList(); // Interface languages

           // var externalOrderingList = new List<int> { 5, 10, 15, 25 };

            return items.OrderByDescending(i => externalOrderingList.IndexOf(i.LangId)).ThenBy(i => i.LangId);
        }
    }
}
