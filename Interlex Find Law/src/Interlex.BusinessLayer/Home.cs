using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.DataLayer;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Entities;
using Newtonsoft.Json;

namespace Interlex.BusinessLayer
{
    public class Home
    {
        public static HomeData GetHomeData(int userId, int siteLangId, int productId)
        {
            HomeData h = new HomeData();
            h.NewDocs = Doc.GetNewDocs(userId, siteLangId, productId);

            // folders
            List<HomeFolderData> l = GetFoldersFlat(productId, siteLangId);

            foreach (HomeFolderData item in l.Where(i => i.parent_id == null))
            {
                //HomeFolder fRoot = new HomeFolder();
                //fRoot.Id = item.id;
                //fRoot.Name = item.title;
                //fRoot.Folders = GetHomeFolderChildren(item.id, l, true).OrderBy(f => f.ord).ThenBy(f => f.title).ToList();
                //h.Folders.Add(fRoot);

                item.children = GetHomeFolderChildren(item.id, l, true).OrderBy(f => f.ord).ThenBy(f => f.title).ToList();
                h.Folders.Add(item);
            }

            return h;
        }

        public static List<HomeFolderData> GetFoldersFlat(int productId, int siteLangId)
        {
            List<HomeFolderData> l = new List<HomeFolderData>();
            foreach (var r in DB.GetFolders(productId, siteLangId))
            {
                HomeFolderData f = new HomeFolderData();
                f.id = Convert.ToInt32(r["folder_id"]);
                f.key = r["folder_id"].ToString();
                f.parent_id = r["parent_folder_id"] as int?;
                f.title = r["name"].ToString();
                f.tooltip = r["hint"].ToString();
                f.ord = (r["ord"] == DBNull.Value) ? -1 : Convert.ToInt32(r["ord"]);
                f.queryType = r["query_type"] as int?;
                f.query = r["query"].ToString();
                f.DocsCount = (r["docs_count"].ToString() != "") ? Convert.ToInt32(r["docs_count"]) : 0;

                l.Add(f);
            }
            return l;
        }

        public static void SetFolderDocsCount(int folderId, int docsCount)
        {
            DB.SetFolderDocsCount(folderId, docsCount);
        }

        private static IEnumerable<HomeFolderData> GetHomeFolderChildren(int parentDocParId, List<HomeFolderData> l, bool root)
        {
            List<HomeFolderData> children = new List<HomeFolderData>();

            foreach (HomeFolderData item in l.Where(i => i.parent_id == parentDocParId))
            {
                item.children = GetHomeFolderChildren(item.id, l, false);
                if (item.children.Count() == 0 && !root)
                    item.extraClasses = "tree-node-last";
                children.Add(item);

                // ensuring no null reference when appending extra classes if more than one
                if (item.extraClasses == null)
                {
                    item.extraClasses = String.Empty;
                }

                // National case law
                if (item.parent_id == 923) // change ID if it changes in DB
                {
                    item.extraClasses = item.extraClasses + " " + "node-national-caselaw-first-level";
                    item.expanded = true;
                }
            }

            return children.OrderBy(f => f.ord).ThenBy(f => f.title);
        }

        public static HomeSearchData GetSearchFolder(int folderId)
        {
            HomeSearchData s = null;
            var row = DB.GetFolder(folderId);
            if (row != null)
            {
                s = new HomeSearchData();
                s.QueryType = Convert.ToInt32(row["query_type"]);
                s.Query = row["query"].ToString();
                if (!(String.IsNullOrEmpty(row["filters_json"].ToString())))
                {
                    s.Filters = JsonConvert.DeserializeObject<Dictionary<int, string>>(row["filters_json"].ToString());
                }

                if (s.Filters.ContainsKey(6))
                {
                    var jurisdictionFilterOriginal = s.Filters[6];
                    if (!String.IsNullOrEmpty(s.Filters[6]))
                    {
                        var mappedClassifier = ClassificationService.ClassificatorService.Current.MappingService.GenericTryGet(jurisdictionFilterOriginal);
                        if (mappedClassifier != null)
                        {
                            s.Filters.Remove(6);
                            s.Filters.Add(7, mappedClassifier.Guid);
                        }
                    }
                    else
                    {
                        s.Filters.Remove(6);
                        s.Filters.Add(7, String.Empty);
                    }
                }

                foreach (var lang in DB.GetFolderLangs(folderId))
                {
                    s.Titles.Add(Convert.ToInt32(lang["lang_id"]), lang["title_path"].ToString());
                }
            }

            return s;
        }
    }
}
