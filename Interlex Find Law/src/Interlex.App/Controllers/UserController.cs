namespace Interlex.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Helpers;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Enums;
    using Interlex.BusinessLayer.Models;
    using Interlex.App.Filters;
    using Interlex.App.Helpers;
    using System.Text;
    using ViewModels;
    [UserAuthorize]
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult CommonSettings()
        {
            return PartialView("~/Views/User/_CommonSettings.cshtml");
        }

        [HttpPost]
        public ActionResult UpdateCommonSettings(string linksInNewTab, string showFreeDocuments)
        {
            if (linksInNewTab != null && showFreeDocuments != null)
            {
                try
                {
                    bool linksInNewTabAsBoolean = linksInNewTab == "true" ? true : false;
                    bool showFreeDocumentsAsBoolean = showFreeDocuments == "true" ? false : true; // it's actually a "show only linked documents" so we are switching the var

                    UserMng.UpdateUserCommonSettings(this.UserData.UserId, linksInNewTabAsBoolean, showFreeDocumentsAsBoolean); // updating DB
                    // updating session values
                    this.UserData.OpenDocumentsInNewTab = linksInNewTabAsBoolean;
                    this.UserData.ShowFreeDocuments = showFreeDocumentsAsBoolean;

                    return Json(Resources.Resources.UI_SettingsUpdated);
                }
                catch (Exception)
                {
                    return Json(Resources.Resources.UI_SettingsUpdatedError);
                }
            }

            return Json(Resources.Resources.UI_SettingsUpdatedError);

            /* if (linksInNewTab != null)
             {
                 try
                 {
                     bool linksInNewTabAsBoolean = false;
                     if (linksInNewTab == "true")
                     {
                         linksInNewTabAsBoolean = true;
                     }

                     UserMng.SetUserLinksInNewTab(this.UserData.UserId, linksInNewTabAsBoolean);
                     this.UserData.OpenDocumentsInNewTab = linksInNewTabAsBoolean;

                     // (Session["UserData"] as UserData).OpenDocumentsInNewTab = linksInNewTabAsBoolean;
                 }
                 catch (Exception)
                 {
                     return Json(Resources.Resources.UI_SettingsUpdatedError);
                 }
             }*/
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return PartialView("~/Views/User/_ChangePassword.cshtml");
        }

        [HttpPost]
        public ActionResult ChangePassword(EditPassword model)
        {
            var newPassword = model.Password;
            var userId = UserData.UserId;
            var result = EditPassword.ChangePassword(userId, newPassword);
            if (result)
            {
                ViewBag.Msg = Resources.Resources.Notify_PasswordChangeSuccess;
            }
            else
            {
                ViewBag.Msg = Resources.Resources.Notify_PasswordChangeError;
            }

            return PartialView("~/Views/User/_SettingsChangeResult.cshtml");
        }

        [HttpGet]
        public ActionResult ChangeEmail()
        {
            return PartialView("~/Views/User/_ChangeEmail.cshtml");
        }

        [HttpPost]
        public ActionResult ChangeEmail(EditEmail model)
        {
            var userId = UserData.UserId;
            try
            {
                var result = EditEmail.ChangeEmail(userId, model.Email, model.Password);

                if (result)
                {
                    ViewBag.Msg = Resources.Resources.Notify_EmailChangeSuccess;
                    ViewBag.Status = "Success";
                }
                else
                {
                    ViewBag.Msg = Resources.Resources.Notify_EmailChangeWrongPassword;
                    ViewBag.Status = "Error";
                }
            }
            catch (Exception)
            {
                ViewBag.Msg = Resources.Resources.Notify_EmailChangeError;
                ViewBag.Status = "Error";
            }

            return PartialView("~/Views/User/_SettingsChangeResult.cshtml");
        }

        [HttpGet]
        public ActionResult AccountDeletion()
        {
            return PartialView("~/Views/User/_AccountDeletion.cshtml");
        }

        [HttpGet]
        public ActionResult PreferedLanguages()
        {
            var lp = new LanguagePreferences(UserData.UserId);

            var languages = lp.Languages;

            ViewBag.Languages = languages;

            return PartialView("~/Views/User/_PreferedLanguages.cshtml", languages);
        }


        [HttpPost]
        public ActionResult PreferedLanguages(int[] model)
        {
            var userId = UserData.UserId;
            var lp = new LanguagePreferences(userId);

            var updateModel = new List<Interlex.BusinessLayer.Models.LanguagePreferences.LanguagePreferencesUpdateModel>();

            for (int i = 0; i < model.Length; i++)
            {
                updateModel.Add(
                new LanguagePreferences.LanguagePreferencesUpdateModel
                {
                    LangNumber = model[i],
                    Position = i + 1
                });
            }

            try
            {
                lp.UpdateLanguagePreferences(userId, updateModel);
                ViewBag.Msg = @Resources.Resources.Notify_LangPreferencesSuccess;
            }
            catch (Exception)
            {
                ViewBag.Msg = @Resources.Resources.Notify_LangPreferencesError;
            }

            return PartialView("~/Views/User/_SettingsChangeResult.cshtml");
        }

        [HttpGet]
        public ActionResult MySearches()
        {
            if (UserData.Username.ToLower() == "sysdemo")
            {
                if (ProductId == 1) // EuroCases
                {
                    return RedirectToAction("ProductFeaturesInfo", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
                }
                else // Tax & Financial Standarts
                {
                    return RedirectToAction("ProductFeaturesInfoFinances", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
                }
            }

            var userId = UserData.UserId;

            int curProductId = 1;
            if (Session["SelectedProductId"] != null)
            {
                curProductId = int.Parse(Session["SelectedProductId"].ToString());
            }

            var model = UserSearches.GetSearchesFromDB(userId, UserSearchDatePeriod.All, UserSearchType.All, curProductId);

            ViewData["UserSearchType"] = model.Filters.Type.ToSelectList();

            return View(model);
        }

        [HttpPost]
        public ActionResult MySearches(string periodId, string typeName)
        {
            if (periodId == null || periodId == "" || string.IsNullOrEmpty(periodId))
            {
                periodId = "0";
            }

            var periodIdAsInt = int.Parse(periodId);
            var period = (UserSearchDatePeriod)periodIdAsInt;

            if (typeName == null || typeName == "" || string.IsNullOrEmpty(typeName))
            {
                typeName = "All";
            }

            var type = (UserSearchType)Enum.Parse(typeof(UserSearchType), typeName);
            int curProductId = 1;
            if (Session["SelectedProductId"] != null)
            {
                curProductId = int.Parse(Session["SelectedProductId"].ToString());
            }

            var model = UserSearches.GetSearchesFromDB(UserData.UserId, period, type, curProductId);

            return Json(model);
        }

        [HttpPost]
        public ActionResult MySearchesDelete(int delid)
        {
            UserSearches.DelUserSearch(delid);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult MySearchesDeleteAll()
        {
            int userid = int.Parse(UserData.UserId.ToString());
            int curProductId = 1;
            if (Session["SelectedProductId"] != null)
            {
                curProductId = int.Parse(Session["SelectedProductId"].ToString());
            }

            try
            {
                UserSearches.DelAllUserSearches(userid, curProductId);
                return Json("Ok");
            }
            catch
            {
                return Json("Failed");
            }
        }

        [HttpPost]
        public ActionResult MySearchesView(int viewId, string globalId, int siteSearchId)
        {
            var searchResults = Session["SearchResults"] as Dictionary<int, SearchResult>;
            SearchResult desiredSearchResultFromSession = null;
            if (searchResults != null && searchResults.ContainsKey(siteSearchId))
            {
                desiredSearchResultFromSession = searchResults[siteSearchId];
            }
            if (desiredSearchResultFromSession != null)
            {
                return Json(desiredSearchResultFromSession.SearchBoxFilters);
            }

            var searchSiteId = Session["search-global-" + globalId];

            var searchDetails = new object();

            if (searchSiteId != null)
            {
                var allSearchResults = Session["SearchResults"];
                var searchResult = SearchResult.FindSearchResult(int.Parse(searchSiteId.ToString()), allSearchResults);
                searchDetails = searchResult.SearchBoxFilters;
            }
            else
            {
                searchDetails = UserSearches.GetUserSearchDetails(viewId);
            }

            return Json(searchDetails);
        }

        [HttpGet]
        public ActionResult MyDocuments()
        {
            if (UserData.Username.ToLower() == "sysdemo")
            {
                if (ProductId == 1) // EuroCases
                {
                    return RedirectToAction("ProductFeaturesInfo", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
                }
                else // Tax & Financial Standarts
                {
                    return RedirectToAction("ProductFeaturesInfoFinances", "User", new { funcTypeId = (int)FunctionalityTypes.MySearches });
                }
            }

            return View("MyDocuments");
        }

        [HttpPost]
        public JsonResult GetFolderData(int? parentId)
        {
            var folderData = UserMng.GetUserFoldersParent(UserData.UserId, ProductId, parentId);
            var jsonData = UserFolderDataJson.FromData(folderData);

            return Json(jsonData);
        }

        public JsonResult GetDocsCountFolder(int? folderId)
        {
            int currentFolderDocsCount = UserMng.GetUserDocsCountFolder(UserData.UserId, ProductId, folderId);

            return Json(new
            {
                documentsCount = currentFolderDocsCount
            });
        }

        [HttpPost]
        public JsonResult CreateFolder(string folderName, int? parentId)
        {
            if (UserData.Username.ToLower() == "sysdemo")
            {
                var result = new { status = 401, title = "Access denied", message = "my documents feature" };
                return Json(result);
            }

            if (String.IsNullOrEmpty(folderName))
            {
                var res = new { status = 400, errorText = Resources.Resources.Notify_FolderNameEmpty };
                return Json(res);
            }

            folderName = folderName.Trim();
            try
            {
                var folderData = UserMng.AddUserFolder(UserData.UserId, ProductId, folderName, parentId);
                var jsonData = new UserFolderDataJson(folderData);
                var result = new { status = 200, folderData = jsonData };

                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                var res = new { status = 500, errorText = Resources.Resources.Notify_FolderCreationError };
                return Json(res);
            }

        }

        [HttpPost]
        public JsonResult RenameFolder(int folderId, string folderName)
        {
            if (String.IsNullOrEmpty(folderName))
            {
                var res = new { status = 400, errorText = Resources.Resources.Notify_FolderNameEmpty };
                return Json(res);
            }

            folderName = folderName.Trim();
            try
            {
                bool isRenamed = UserMng.RenameUserFolder(UserData.UserId, folderId, folderName);
                if (isRenamed)
                {
                    var res = new { status = 200, folderName = folderName };
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                var res = new { status = 500, errorText = Resources.Resources.Notify_FolderRenameError };
                return Json(res);
            }

            var result = new { status = 400, errorText = Resources.Resources.Notify_FolderNotFoundError };
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteFolder(int folderId)
        {
            try
            {
                UserMng.DeleteUserFolder(UserData.UserId, folderId);
                var result = new { status = 200, folderId = folderId };
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                var result = new { status = 500, errorText = Resources.Resources.Notify_FolderDeleteError };
                return Json(result);
            }
        }

        public ActionResult MyDocumentsList(string orderBy, string orderDir, int? folderId)
        {
            if (orderBy == null)
            {
                orderBy = "add_date";
            }

            if (orderDir == null)
            {
                if (orderBy != "add_date")
                {
                    orderDir = "asc";
                }
                else
                {
                    orderDir = "desc";
                }
            }

            var model = UserMng.GetUserDocsFolder(UserData.UserId, this.Language.Id, orderBy,
                                                  orderDir, ProductId, folderId);

            /*  if (orderBy == "add_date")
              {
                  if (orderDir == "desc")
                  {
                      model = model.OrderByDescending(m => m.AddedDate);
                  }
                  else
                  {
                      model = model.OrderBy(m => m.AddedDate);
                  }
              }
              else if (orderBy == "title")
              {
                  if (orderDir == "desc")
                  {
                      model = model.OrderByDescending(m => m.Title);
                  }
                  else
                  {
                      model = model.OrderBy(m => m.Title);
                  }
              }*/

            ViewBag.OrderBy = orderBy;
            ViewBag.OrderDir = orderDir;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult MyDocumentsDeleteAll(int? folderId)
        {
            int userid = int.Parse(UserData.UserId.ToString());

            try
            {
                UserMng.DelAllUserDocs(userid, folderId);
                return Json("Ok");
            }
            catch (Exception ex)
            {
                Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                return Json("Failed");
            }
        }

        [HttpPost]
        public ActionResult AddUserDoc(string docLangId, int? folderId)
        {
            if (UserData.Username.ToLower() == "sysdemo")
            {
                var result = new { status = 401, title = "Access denied", message = "my documents feature" };
                return Json(result);
            }

            int langId = int.Parse(docLangId);

            if (UserMng.GetUserDocsCount(UserData.UserId, ProductId) >= int.Parse(System.Configuration.ConfigurationManager.AppSettings["FavouriteDocumentsCount"]))
            {
                var statusTitle = Resources.Resources.Notify_DocumentAddMaxCountTitle;
                var statusMessage = Resources.Resources.Notify_DocumentAddMaxCountMessage;

                var result = new { status = 202, title = statusTitle, message = statusMessage };
                Response.StatusCode = 202;

                return Json(result);
            }
            else if (UserMng.GetUserHasDocument(UserData.UserId, langId, ProductId))
            {
                var statusTitle = Resources.Resources.Notify_DocumentAddExistingTitle;
                var statusMessage = Resources.Resources.Notify_DocumentAddExistingMessage;

                var result = new { status = 202, title = statusTitle, message = statusMessage };
                Response.StatusCode = 202;

                return Json(result);
            }
            else
            {
                try
                {
                    UserMng.AddUserDoc(UserData.UserId, langId, ProductId, folderId);

                    var result = new { status = 200, title = "Success", message = "Document added successfuly" };
                    Response.StatusCode = 200;

                    return Json(result);
                }
                catch (Exception ex)
                {
                    Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                    var statusTitle = Resources.Resources.Notify_DocumentAddFailTitle;
                    var statusMessage = Resources.Resources.Notify_DocumentAddFailMessage;

                    var result = new { status = 500, title = statusTitle, message = statusMessage };
                    Response.StatusCode = 202;

                    return Json(result);
                }
            }
        }

        [HttpPost]
        public ActionResult DelAcc(string password)
        {
            var userid = base.UserData.UserId;
            var username = base.UserData.Username;
            var testGet = UserMng.GetUser(username, password, false);
            if (testGet == null)
            {
                return Content("FAIL");
            }
            else
            {
                UserMng.DelClient(testGet.ClientId);
                Session.Abandon();
                return Content("OK");
            }
        }

        [HttpPost]
        public ActionResult DelUserDoc(string docLangId)
        {
            int langId = int.Parse(docLangId);

            try
            {
                UserMng.DelUserDoc(UserData.UserId, langId, ProductId);

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                var statusTitle = Resources.Resources.Notify_DocumentDeletionFailTitle;
                var statusMessage = Resources.Resources.Notify_DocumentDeletionFailMessage;

                var result = new { status = 500, title = statusTitle, message = statusMessage };
                Response.StatusCode = 202;

                return Json(result);
            }
        }

        public ActionResult MoveDocs(int? fromId, int? toId)
        {
            try
            {
                UserMng.MoveDocsFolderToFolder(UserData.UserId, ProductId, fromId, toId);
                var result = new { status = 200 };
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionToFolder(HttpRuntime.AppDomainAppPath, UserData, ex);
                var result = new { status = 500, errorText = Resources.Resources.UI_MoveDocsError };
                return Json(result);
            }
        }

        public ActionResult ProductFeaturesInfoFinances(int? funcTypeId)
        {
            ViewBag.UseLayout = true;
            var functionalityType = GetFunctionalityType(funcTypeId);
            var productFeaturesInfo = new ProductFeaturesInfo(functionalityType);
            return View("~/Views/Shared/_ProductFeaturesInfo_Finances.cshtml", productFeaturesInfo);
        }

        public ActionResult ProductFeaturesInfo(int? funcTypeId)
        {
            var functionalityType = GetFunctionalityType(funcTypeId);
            var productFeaturesInfo = new ProductFeaturesInfo(functionalityType);
            return View("~/Views/Shared/_ProductFeaturesInfo_Wrapper.cshtml", productFeaturesInfo);
        }

        public ActionResult ProductFeaturesInfoPartial(int? funcTypeId)
        {
            var functionalityType = GetFunctionalityType(funcTypeId);
            var productFeaturesInfo = new ProductFeaturesInfo(functionalityType);
            return PartialView("~/Views/Shared/_ProductFeaturesInfo.cshtml", productFeaturesInfo);
        }

        public ActionResult GetSelectedLangId()
        {
            return Content(Language.Id.ToString());
        }

        // temp
        public ActionResult TransformUserSearchesMultiDictObjects()
        {
            var updateModel = UserMng.TransformUserSearchesMultiDictObjects();

            if (updateModel != null)
            {
                UserMng.UpdateUserSearchesMultiDictObjects(updateModel);
            }

            return new EmptyResult();
        }

        public FunctionalityTypes GetFunctionalityType(int? funcTypeId)
        {
            if (funcTypeId.HasValue)
            {
                return (FunctionalityTypes)funcTypeId.Value;
            }
            else
            {
                return FunctionalityTypes.Default;
            }
        }
    }
}
