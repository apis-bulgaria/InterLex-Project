using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Interlex.App.ApiFilters;
using Interlex.BusinessLayer.Entities;
using Interlex.BusinessLayer;
using Interlex.BusinessLayer.Models;
using ClassificationService;

namespace Interlex.App.Api
{
    [UserAuthorize]
    public class EntityController : BaseApiController
    {
        #region Folders

        [HttpPost]
        [HttpGet]
        public IReadOnlyCollection<FolderData> GetClassifier(string classifierType, string parentId, string tab, int? searchId)
        {
            //List<FolderData> folderData = ObjectFactory.GetInstance<IClassifier>().Get();
            //SetSelectedFolders<IClassifier>(folderData, searchId, tab);

            string clientSelectedIds = HttpContext.Current.Request.Form["selectedIds"];

            int searchCaseLawArea = 0;
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["caseLawArea"]))
                searchCaseLawArea = Convert.ToInt32(HttpContext.Current.Request.Form["caseLawArea"]);

            int searchLegislationArea = 0;
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["legislationArea"]))
            {
                searchLegislationArea = Convert.ToInt32(HttpContext.Current.Request.Form["legislationArea"]);
            }

            int productId = 1;
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["productId"]))
            {
                productId = Convert.ToInt32(HttpContext.Current.Request.Form["productId"]);
            }

            List<FolderData> folderData = null;
            if (tab == "PAL" && classifierType == "DocumentTypes" && parentId == "null")
            {
                //Законодателство на ЕС
                folderData = Classifiers.GetClassifier(tab, classifierType, "af88ca51-7522-455a-aefe-ec0d3c2d6a37", Language.Id, productId)
                    .Where(c => c.key == "cc256228-38a8-4cc0-a83b-4418373a757e" || c.key == "78234434-a07a-48ed-bd2c-f2f7f4615326"
                    || c.key == "9ae781f4-290e-4344-b76b-f9f98b2dd309" || c.key == "c2f33f1a-1f6e-4619-8c7f-386be4636c1a"
                    || c.key == "4fb08dfb-2f8b-4892-8268-4d9d8d59416d").ToList();

                folderData.FirstOrDefault(c => c.key == "9ae781f4-290e-4344-b76b-f9f98b2dd309").selected = true; // Регламенти chosen by default

                // 	Международни споразумения
                // 	Споразумения между държавите - членки
                // 	Регламенти
                // 	Директиви
                // 	Решения
            }
            if (tab == "Cases")
            {
                if (classifierType == "CourtsFolders" && searchCaseLawArea != 0)
                {
                    if (searchCaseLawArea == 2)
                    {
                        folderData = Classifiers.GetClassifier(tab, classifierType, parentId, Language.Id, productId)
                            .Where(c => c.tree_level != 2 || c.key != "7eff6a8f-2a80-4128-a3d6-16687a5a54ae")
                            .Where(c => c.key != "5bcbe8a9-f7ac-4b9a-ae16-c664e12768e3")
                            .ToList();
                    }
                    else if (searchCaseLawArea == 1 && parentId == "null")
                    {
                        folderData = Classifiers.GetClassifier(tab, classifierType, "7eff6a8f-2a80-4128-a3d6-16687a5a54ae", Language.Id, productId);
                    }
                }
                else if (classifierType == "ProcedureType" && parentId == "null")
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, parentId, Language.Id, productId);
                    folderData.ForEach(f => f.lazy = false);
                }
                else if (classifierType == "DocumentTypes" && searchCaseLawArea == 1 && parentId == "null")
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, "030bc6ca-9696-4efe-ba4b-27d54a335726", Language.Id, productId);
                }
                else if (classifierType == "DocumentTypes" && searchCaseLawArea == 3 && parentId == "null")
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, "6d981719-977c-4a7f-ade1-9b6e0e8852d6", Language.Id, productId);
                }
                else if (classifierType == "DirectoryCaseLaw")
                {
                    int caseLawDirCodesFull = 0;
                    if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["caseLawDirCodesFull"]))
                        caseLawDirCodesFull = Convert.ToInt32(HttpContext.Current.Request.Form["caseLawDirCodesFull"]);

                    if (caseLawDirCodesFull == 0 && parentId == "null") // показваме децата на папка '...след Лисабон...'
                    {
                        folderData = Classifiers.GetClassifier(tab, classifierType, "32016c73-e519-447a-a222-a7ccfe2ae675", Language.Id, productId);
                    }
                }
                else if (classifierType == "Courts" && searchCaseLawArea == 3) // Showing HUDOC Courts
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, "6672ab3c-3386-4204-a019-493cdd62cd31", Language.Id, productId);
                }
                else if (classifierType == "HudocArticles" && parentId == "null")
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, "8da600c2-52f2-48aa-b9a4-57d6936f4e1f", Language.Id, productId);
                }
                else if (classifierType == "RulesOfTheCourt" && parentId == "null")
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, "1abda806-23b9-41e7-b64a-793b6a402d4d", Language.Id, productId);
                }
                else if (classifierType == "EuroCases" && searchCaseLawArea == 3 && parentId == "null")
                {
                    // showing only political rights and civil rights for ECHR
                    folderData = Classifiers.GetClassifier(tab, classifierType, "c9fe93b7-7858-1645-9c4d-ddf66565c0af", Language.Id, productId)
                        .Where(fd => fd.id == new Guid("0489508c-7d34-f64d-940c-a08d9801a1d4") || fd.id == new Guid("9289e459-2cf5-1540-81ef-0d1dc7164d9b")).ToList();
                }
            }

            if (tab == "Law")
            {
                if (classifierType == "DocumentTypes" && parentId == "null")
                {
                    if (searchLegislationArea == 2) // National legislation
                    {
                        folderData = Classifiers.GetClassifier(tab, classifierType, "987e4eef-3e55-43be-9052-bb98ef1dfd83", Language.Id, productId);
                    }
                    else // EU Legislation
                    {
                        folderData = Classifiers.GetClassifier(tab, classifierType, "af88ca51-7522-455a-aefe-ec0d3c2d6a37", Language.Id, productId);
                    }
                }

                if (classifierType == "ActJurisdictions" && parentId == "null")
                {
                    folderData = Classifiers.GetClassifier(tab, classifierType, "6cd20df9-8110-480e-807d-d840fce88b9a", Language.Id, productId);
                    folderData = folderData.Where(d => d.id.ToString().ToUpper() != "BE076ED2-9F60-4B24-9560-19B5E672D947").ToList();
                }
            }

            /*  if (classifierType == "SummarySources")
              {
                  folderData = Classifiers.GetClassifier(tab, classifierType, "8bb4698f-ea1e-408a-9614-46f2feea8199", Language.Id);
                  if (folderData != null && folderData.Count == 0)
                  {
                      folderData = null;
                  }
              }*/

            if (folderData == null) // Default
            {
                folderData = Classifiers.GetClassifier(tab, classifierType, parentId, Language.Id, productId);
            }

            SetSelectedFolders(classifierType, folderData, searchId, clientSelectedIds, tab);

            foreach (var folder in folderData)
            {
                if (folder.docs_count == 0)
                {
                    folder.extraClasses = "classifier-no-docs";
                }
            }

            return folderData.OrderBy(f => f.ord).ThenBy(f => f.title).ToList();
        }

        private void CheckFolderDataChildren(FolderData fd, Guid[] selectedIds, bool parentSelected)
        {
            if (parentSelected == true || selectedIds.Contains<Guid>(fd.id))
                fd.selected = true;
            //for (int i = 0; i < fd.children.Count; i++)
            //    CheckFolderDataChildren(fd.children[i], selectedIds, fd.selected);
        }

        private void SetSelectedFolders(string classifierType, List<FolderData> folderData, int? searchId, string clientSelectedIds, string tab)
        {
            Guid[] selectedIds = null;
            if (!String.IsNullOrWhiteSpace(clientSelectedIds))
            {
                selectedIds = Array.ConvertAll<string, Guid>(clientSelectedIds.Split(','), Guid.Parse);
            }
            else if (searchId.HasValue && HttpContext.Current.Session["SearchResults"] != null)
            {
                SearchResult sr = SearchResult.FindSearchResult(searchId.Value, HttpContext.Current.Session["SearchResults"]);
                if (sr != null)
                {
                    string SelectedIdsStr = "";

                    //if (tab == "Cases" && sr.SearchBoxFilters.Cases != null)
                    //{
                    //    if (classifierType == "Eurovoc")
                    //        SelectedIdsStr = sr.SearchBoxFilters.Cases.Eurovoc.SelectedIds;
                    //    if (classifierType == "SubjectMatter")
                    //        SelectedIdsStr = sr.SearchBoxFilters.Cases.SubjectMatter.SelectedIds;
                    //}
                    //else if (tab == "Law" && sr.SearchBoxFilters.Law != null)
                    //{
                    //    if (classifierType == "Eurovoc")
                    //        SelectedIdsStr = sr.SearchBoxFilters.Law.Eurovoc.SelectedIds;
                    //}

                    if (!String.IsNullOrEmpty(SelectedIdsStr))
                    {
                        selectedIds = Array.ConvertAll<string, Guid>(SelectedIdsStr.Split(','), Guid.Parse);
                    }
                }
            }

            if (selectedIds != null)
            {
                foreach (FolderData fd in folderData)
                    CheckFolderDataChildren(fd, selectedIds, false);
            }
        }

        /// <summary>
        /// Fill in classifier titles in treeview textbox when search is loaded from 'My Searches'
        /// </summary>
        /// <returns>Classifier titles delimited by | .</returns>
        [HttpPost]
        public string GetSelectedTitlePaths()
        {
            string clientSelectedIds = HttpContext.Current.Request.Form["clientSelectedIds"];
            string titles = "";
            if (!String.IsNullOrWhiteSpace(clientSelectedIds))
            {
                Guid[] selIds = Array.ConvertAll<string, Guid>(clientSelectedIds.Split(','), Guid.Parse);
                if (selIds != null)
                {
                    List<string> titlePaths = new List<string>();
                    foreach (Guid id in selIds)
                    {
                        titlePaths.Add(Classifiers.GetClassifierId_TitlePath(id, this.Language.Id, true));
                    }
                    titles = String.Join("|", titlePaths);
                }
            }

            return titles;
        }

        #endregion


        #region Doc Contents

        [HttpPost]
        [HttpGet]
        public IReadOnlyCollection<DocContentItem> DocContents(int docLangId)
        {
            var content = Doc.GetDocContents(docLangId);

            return content;
        }


        #endregion
    }

}
