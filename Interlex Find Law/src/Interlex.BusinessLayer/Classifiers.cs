namespace Interlex.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.DataLayer;
    using Interlex.BusinessLayer.Models;
    using Interlex.BusinessLayer.Entities;
    using ClassificationService;

    public class Classifiers
    {
        public static List<FolderData> GetClassifier(string tab, string classifierType, string parentId, int langId, int productId, bool skipFirstLevel = true)
        {
            var csc = ClassificatorService.Current;

            List<FolderData> fd = new List<FolderData>();
            if (classifierType == "EULegislation")
            {
                classifierType = "DirectoryLegislatioN";
            }
            int classifierTypeId = DB.GetClassifierTypeIdByName(classifierType);
            /*
            if (classifierTypeId.HasValue)
            {
                foreach (var r in DB.GetClassifier(classifierTypeId.Value, parentId, langId))e
                {
                    Guid id = Guid.Parse(r["id"].ToString());
                    
                    if (skipFirstLevel && !parentId.HasValue)
                        return GetClassifier(classifierType, id, langId, skipFirstLevel);

                    fd.Add(new FolderData()
                    {
                        id = id,
                        key = r["id"].ToString(),
                        title = r["name"].ToString(),
                        lazy = true,
                        folder = false
                    });
                }
            }*/

            if (String.IsNullOrEmpty(parentId) || parentId == null || parentId == "null")
            {
                parentId = DB.GetClassifierBaseByTypeId(classifierTypeId);
            }

            var treeByParent = ClassifiersProvider.Service.GetTreeByGuid(parentId);

            foreach (var child in treeByParent.Children)
            {
                var curFolderData = new FolderData()
                {
                    id = new Guid(child.Guid),
                    key = child.Guid,
                    ord = (child.Order.HasValue) ? child.Order : -1,
                    title = child.LanguageVariantsWithHints.ContainsKey(langId.ToString()) ? child.LanguageVariantsWithHints[langId.ToString()].Title : child.LanguageVariantsWithHints.Values.FirstOrDefault().Title.ToString(),
                    //tooltip = (child.LanguageVariantsWithHints.ContainsKey(langId.ToString()) && child.LanguageVariantsWithHints[langId.ToString()].Hint != "") ? child.LanguageVariantsWithHints[langId.ToString()].Hint : child.LanguageVariantsWithHints.Values.FirstOrDefault().Hint.ToString(),
                    tooltip = (child.LanguageVariantsWithHints.ContainsKey(langId.ToString())) ? child.LanguageVariantsWithHints[langId.ToString()].Hint : "",
                    lazy = (child.Children.Count > 0), // Show/Hide lazy load icon (rectangle) in tree view
                    folder = false,
                    tree_level = child.TreeLevel
                    //  docs_count = child.DocumentCount,
                    //  unselectable = child.DocumentCount == 0 ? true : false,
                    //  hideCheckbox = child.DocumentCount == 0 ? true : false
                };

                /* if (tab.ToUpper() == "CASES") // ToUpper is faster than ToLower lol..
                 {
                     curFolderData.docs_count = child.DocumentCounts["caselaw"].Count;
                 }
                 else if (tab.ToUpper() == "LAW")
                 {
                     curFolderData.docs_count = child.DocumentCounts["legislation"].Count;
                 }
                 else if (tab.ToUpper() == "PAL")
                 {
                     curFolderData.docs_count = child.DocumentCounts["legislation"].Count;
                 }
                 else // finances and other
                 {
                     curFolderData.docs_count = child.DocumentCounts["any"].Count;
                 }*/

                if (productId == 1)
                {
                    // Juridiction context
                    if (child.ClassifierTypeId == 7)
                    {
                        // Direct jurisdiction mappings
                        ClassificatorTreeModel mappedClassifier = null;
                        mappedClassifier = csc.MappingService.GenericTryGet(child.Guid.ToString());
                        if (mappedClassifier != null)
                        {
                            // mapping is found
                            curFolderData.docs_count = mappedClassifier.DocumentCounts["eurocases"].Count;
                        }
                        else
                        {
                            // checking childs - no deep-level recursive check for performance, edit this if classifier struct changes
                            var currentClassifierChilds = csc.TryGetTreeByGuid(child.Guid.ToString()).Children;
                            var mappedChilds = currentClassifierChilds.Select(c => csc.MappingService.GenericTryGet(c.Guid.ToString()));
                            var mappedChildsCount = 0;
                            if (mappedChilds.Any(c => c != null))
                            {
                                mappedChildsCount = mappedChilds.Sum(c => c.DocumentCounts["eurocases"].Count);
                            }

                            curFolderData.docs_count = mappedChildsCount;
                        }
                    }
                    else
                    {
                        curFolderData.docs_count = child.DocumentCounts["eurocases"].Count;
                    }
                }
                else if (productId == 2)
                {
                    curFolderData.docs_count = child.DocumentCounts["tfs"].Count;
                }

                var isDocsCountZero = curFolderData.docs_count == 0;

                curFolderData.unselectable = isDocsCountZero;
                curFolderData.hideCheckbox = isDocsCountZero;

                if (productId == 1 || (productId == 2 && isDocsCountZero == false))
                {
                    fd.Add(curFolderData);
                }
            }

            return fd;
        }

        public static string GetClassifierId_TitlePath(Guid id, int langId, bool skipRoot)
        {
            return DB.GetClassifierId_TitlePath(id, langId, skipRoot);
        }

        public static string[] GetClassifierChildrenIds(Guid id)
        {
            return DB.GetClassifierChildrenIds(id);
        }

        public static Dictionary<string, string[]> GetClassifiersMap()
        {
            Dictionary<string, string[]> map = new Dictionary<string, string[]>();
            foreach (var row in DB.GetClassifiersMap())
            {
                map.Add(row["classifier_id"].ToString(), (string[])row["mapping_classifier_ids"]);
            }
            return map;
        }

        /// <summary>
        /// // Map classifier id to meaningful classifier id/ids (used in Courts classifiers tree)
        /// </summary>
        /// <param name="ids">classifier ids to map</param>
        /// <param name="map">Full classifiers map stored in Application object</param>
        /// <returns></returns>
        public static List<string> MapClassifiers(List<string> ids, Dictionary<string, string[]> map)
        {
            List<string> result = new List<string>();
            foreach (string id in ids)
            {
                if (map.ContainsKey(id))
                    result.AddRange(map[id]); // add mapped classifier id list
                else
                    result.Add(id); // add id
            }
            return result.Distinct().ToList();
        }

        public static string GetClassifierIdByTitle(string title)
        {
            return DB.GetClassifierIdByTitle(title);
        }

        public static string GetClassifierIdByXmlId(string xmlId)
        {
            return DB.GetClassifierIdByXmlId(xmlId);
        }
    }
}
