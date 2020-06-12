namespace Interlex.BusinessLayer
{
    using ClassificationService;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interlex.DataLayer;

    public static class ClassifiersProvider
    {
        public static List<ClassificatorTreeModel> Classifiers;
        public static List<ClassificatorMapModel> Mappings;

        public static ClassificationService.IClassificatorService Service { get { return ClassificationService.ClassificatorService.Current; } }

        static ClassifiersProvider()
        {
            PopulateClassifiersFromCache();
            PopulateMappingsFromCache();
            Fill();
        }

        public static void Fill()
        {
            ClassificationService.ClassificatorService.ExternalFill(Classifiers, Mappings);
        }

        public static void PopulateClassifiersFromCache()
        {
            Classifiers = CacheProvider.Provider.GetOrSetForever("classifiers", () => GetClassifiersFromDatabase());
        }

        public static void PopulateMappingsFromCache()
        {
            Mappings = CacheProvider.Provider.GetOrSetForever("classifiers_mappings", () => GetMappingsFromDatabase());
        }

        private static List<ClassificatorTreeModel> GetClassifiersFromDatabase()
        {
            int deepestLevel = DB.GetClassifiersDeepestLevel();
            var dic = new Dictionary<string, ClassificatorTreeModel>();

            for (int i = 1; i <= deepestLevel; i++)
            {
                var classifiersFromDB = DB.GetClassifiersByTreeLevel(i);

                foreach (var classifier in classifiersFromDB)
                {
                    var curModel = new ClassificatorTreeModel();
                    curModel.Guid = classifier["_id"].ToString();
                    curModel.Order = (classifier["ord"] != DBNull.Value) ? Convert.ToInt32(classifier["ord"]) : (int?)null;
                    curModel.XmlId = classifier["_xml_id"].ToString();
                    curModel.ClassifierTypeId = int.Parse(classifier["_classifier_type_id"].ToString());
                    curModel.TreeLevel = i;
                 /*   if (Convert.ToBoolean(classifier["_has_assigned_caselaw"])) // change this later to real count for caselaw is needed
                    {
                        curModel.DocumentCounts.Add("caselaw", new DocumentCountModel { Count = 1 });
                    }
                    else
                    {
                        curModel.DocumentCounts.Add("caselaw", new DocumentCountModel { Count = 0 });
                    }

                       if (Convert.ToBoolean(classifier["_has_assigned_legislation"])) // change this later to real count for legislation is needed
                       {
                           curModel.DocumentCounts.Add("legislation", new DocumentCountModel { Count = 1 });
                       }
                       else
                       {
                           curModel.DocumentCounts.Add("legislation", new DocumentCountModel { Count = 0 });
                       }

                       if (Convert.ToBoolean(classifier["_has_assigned_any_doc"]))
                       {
                           curModel.DocumentCounts.Add("any", new DocumentCountModel { Count = 1 });
                       }
                       else
                       {
                           curModel.DocumentCounts.Add("any", new DocumentCountModel { Count = 0 });
                       }*/

                    if (Convert.ToBoolean(classifier["_has_asigned_eurocases"]))
                    {
                        curModel.DocumentCounts.Add("eurocases", new DocumentCountModel { Count = 1 });
                    }
                    else
                    {
                        curModel.DocumentCounts.Add("eurocases", new DocumentCountModel { Count = 0 });
                    }

                    if (Convert.ToBoolean(classifier["_has_asigned_tfs"]))
                    {
                        curModel.DocumentCounts.Add("tfs", new DocumentCountModel { Count = 1 });
                    }
                    else
                    {
                        curModel.DocumentCounts.Add("tfs", new DocumentCountModel { Count = 0 });
                    }
                    
                    var curLangIds = (int[])classifier["_langIds"];
                    var curTitles = (string[])classifier["_langTitles"];
                    var curHints = (string[])classifier["_langTooltips"];

                    if (curLangIds.Length == curTitles.Length)
                    {
                        for (int k = 0; k < curLangIds.Length; k++)
                        {
                            curModel.LanguageVariantsWithHints.Add(curLangIds[k].ToString(), new Pair() { Title = curTitles[k], Hint = curHints[k] });
                        }
                    }

                    //  var curLangs = DB.GetClassifierLangs(classifier["_id"].ToString());
                    /* foreach (var lang in curLangs)
                     {
                         curModel.LanguageVariants.Add(lang["lang_id"].ToString(), lang["title"].ToString());
                     }*/

                    if (dic.ContainsKey(classifier["_parent_id"].ToString()))
                    {
                        curModel.Parent = dic[classifier["_parent_id"].ToString()];
                        dic[classifier["_parent_id"].ToString()].Children.Add(curModel);
                    }

                    dic.Add(curModel.Guid, curModel);
                }
            }

            var classifiers = new List<ClassificatorTreeModel>();

            var topLevelCount = DB.GetClassifiersTopLevelCount();
            for (int i = 0; i < topLevelCount; i++)
            {
                classifiers.Add(dic.Values.ElementAt(i));
            }

            var fictiveClassifier = new ClassificatorTreeModel();
            fictiveClassifier.Guid = default(Guid).ToString();
            fictiveClassifier.TreeLevel = 0;
            fictiveClassifier.ClassifierTypeId = 0;

            classifiers.Add(fictiveClassifier);

            return classifiers;
        }

        private static List<ClassificatorMapModel> GetMappingsFromDatabase()
        {
            var mappingsList = new List<ClassificatorMapModel>();

            var mappingsFromDb = DB.GetClassifiersMappings();

            foreach (var mapping in mappingsFromDb)
            {
                var mappingItemModel = new ClassificatorMapModel();
                mappingItemModel.ClassifierId = mapping["classifier_id"].ToString();
                mappingItemModel.MappingClassifierId = mapping["mapping_classifier_id"].ToString();
                mappingsList.Add(mappingItemModel);
            }

            return mappingsList;
        }


        public static void ClearClassifiers()
        {
            CacheProvider.Provider.DeleteCacheItem("classifiers");
        }

        public static void ClearMappings()
        {
            CacheProvider.Provider.DeleteCacheItem("classifiers_mappings");
        }
    }
}
