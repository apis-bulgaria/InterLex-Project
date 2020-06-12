using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.DataLayer;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Enums;

namespace Interlex.BusinessLayer
{
    public class Search
    {
        public static List<SearchListItem> SearchCourtActs()
        {
            List<SearchListItem> result = new List<SearchListItem>();

            return result;
        }

        //public static List<ClassifierItem> GetFilterClassifierTypes(int searchId, List<Tuple<int, string>> classifierFilters)
        //{
        //    List<ClassifierItem> l = new List<ClassifierItem>();
        //    foreach (var r in DB.GetFilterClassifierTypes(searchId, classifierFilters))
        //    {
        //        l.Add(new ClassifierItem()
        //        {
        //            Id = Guid.Parse(r["classifier_id"].ToString()),
        //            Type = (ClassifierTypes)r["classifier_type_id"]
        //        });
        //    }
        //    return l;
        //}

        public static List<ClassifierItem> GetFilterClassifier(int searchId, Guid? parent, ClassifierTypes classifierType, int langId, Guid[] classifierFiltersIds)
        {
            List<ClassifierItem> l = new List<ClassifierItem>();
            foreach (var r in DB.GetFilterClassifier(searchId, parent, (int)classifierType, langId, classifierFiltersIds))
            {
                l.Add(new ClassifierItem()
                {
                    Id = Guid.Parse(r["classifier_id"].ToString()),
                    Type = classifierType,
                    Name = r["title"].ToString(),
                    DocsCount = Convert.ToInt32(r["cnt"])
                });
            }
            return l;
        }

        public static List<ClassifierItem> GetFilterClassifiers(Guid[] selectedClassifiers, int langId, string docClassifiersJSON)
        {
            List<ClassifierItem> l = new List<ClassifierItem>();
            foreach (var r in DB.GetFilterClassifiers(selectedClassifiers, langId, docClassifiersJSON))
            {
                l.Add(new ClassifierItem()
                {
                    Id = Guid.Parse(r["classifier_id"].ToString()),
                    Type = (ClassifierTypes)r["classifier_type_id"],
                    Name = r["title"].ToString(),
                    DocsCount = Convert.ToInt32(r["cnt"])
                });
            }
            return l;
        }

        public static int[] GetSearchDocLangIdsBytes(int searchId)
        { 
            byte[] b = DB.GetSearchDocLangIdsBytes(searchId);
            //int[] searchIds = Array.ConvertAll(b, c => (int)c);
            //int[] searchIds = b.Select(x => (int)x).ToArray();

            int arrIndex = 0;
            int[] searchIds = new int[b.Length/4];
            for (int i = 0; i < b.Length; i = i + 4)
            {
                searchIds[arrIndex] = BitConverter.ToInt32(b, i);
                arrIndex++;
            }

            return searchIds;
        }

        public static void DelSearch(int searchId)
        {
            DB.DelSearch(searchId);
        }

        public static int[] GetPALSearchResult(int[] docLangIds, int productId)
        {
            return DB.GetPALSearchResult(docLangIds, productId);
        }

        public static int[] GetReferedActECHRSearchResult(int[] docLangIds)
        {
            return DB.GetReferedActECHRSearchResult(docLangIds);
        }
    }
}
