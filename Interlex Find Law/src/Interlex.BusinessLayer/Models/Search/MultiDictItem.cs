namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interlex.DataLayer;

    public class MultiDictItem : IComparable
    {
        //  public int Id { get; set; }

        public string ItemId { get; set; }

        public string Text { get; set; }

        public string SearchText { get; set; }

        public bool Selected { get; set; }

        public int LangId { get; set; }

        public MultiDictItem(string itemId, string text, bool selected = false)
        {
            this.ItemId = itemId;
            this.Text = text;
            this.Selected = selected;
        }

        public MultiDictItem(string itemId, string text, bool selected, int langId, String searchText) : this(itemId, text, selected)
        {
            this.LangId = langId;
            this.SearchText = searchText;
        }

        /*   public static List<MultiDictItem> GetMultiDictSearchItems(int langId, string searchText, string leadingCharacter)
           {
               var items = new List<MultiDictItem>();

               foreach (var item in DB.GetMultiDictSearchItems(langId, searchText, leadingCharacter))
               {
                   var curItem = new MultiDictItem(item["item_id"].ToString(), item["item_value"].ToString(), false);
                   items.Add(curItem);
               }

               return items;
           }*/

        public static List<MultiDictItem> GetMultiDictSearchItems(int langId, string searchText, string leadingCharacter)
        {
            var items = CacheProvider.Provider.GetOrSetForever("multilingual_dictionary", () => MultiDictItem.GetAllMultiDictItems())[langId]
                .Where(i => i.Text.ToUpper().Contains(searchText.ToUpper())).ToList();

            if (!String.IsNullOrEmpty(leadingCharacter))
            {
                items = items.Where(i => i.Text.ToUpper().StartsWith(leadingCharacter)).ToList(); // no need of to uppering of leading character
            }

            return items;
        }

        public static Dictionary<int, SortedSet<MultiDictItem>> GetAllMultiDictItems()
        {
            var items = new Dictionary<int, SortedSet<MultiDictItem>>();

            foreach (var item in DB.GetAllMultiDictItems())
            {
                var curLangId = Convert.ToInt32(item["lang_id"]);

                if (!items.ContainsKey(curLangId))
                {
                    items[curLangId] = new SortedSet<MultiDictItem>();
                }

                var searchText = item["value"].ToString();
                var text = searchText?.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries)
                    .OrderBy(x => x.Length)
                    .First();


                var curItem = new MultiDictItem(
                    item["item_id"].ToString(),
                    text,
                    false,
                    Convert.ToInt32(item["lang_id"]),
                    searchText
                );

                items[curLangId].Add(curItem);
            }

            return items;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            MultiDictItem comparingItem = obj as MultiDictItem;

            return this.Text.CompareTo(comparingItem.Text);
        }
    }
}
