namespace Interlex.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Enums;
    using Interlex.BusinessLayer.Models;
    using System.Text.RegularExpressions;
    using System.Resources;
    public class CompareController : BaseController
    {
        public ActionResult Index(string firstDocument, string secondDocument)
        {
            int firstDocLangId = int.Parse(firstDocument);
            int secondDocLangId = int.Parse(secondDocument);

            var highlightParams = new DocHighlightSearchParams(null, null, true, null, null);

            var firstContentArray =
                CourtActBL.GetCourtAct(firstDocLangId,
                this.Language.Id,
                UserData.UserId,
                highlightParams,
                ProductId,
                this.UserData.ShowFreeDocuments)
                    .HtmlModel
                    .Body
                    .Content;

            var secondContentArray =
                CourtActBL.GetCourtAct(
                    secondDocLangId,
                    this.Language.Id,
                    UserData.UserId,
                    highlightParams,
                    ProductId,
                    this.UserData.ShowFreeDocuments)
                        .HtmlModel
                        .Body
                        .Content;

            var firstContent = String.Join(String.Empty, firstContentArray);
            var secondContent = String.Join(String.Empty, secondContentArray);

            Merger merger = new Merger(firstContent, secondContent);
            string result = merger.Merge();

            ViewBag.Html = result;

            return View();
        }

        public ActionResult ByIdentifier(string firstIdentifier, string secondIdentifier)
        {
            int? firstDocLangId = Doc.GetDocLangIdByIdentifier(firstIdentifier);
            int? secondDocLangId = Doc.GetDocLangIdByIdentifier(secondIdentifier);

            if (firstDocLangId.HasValue && secondDocLangId.HasValue)
            {
                return RedirectToAction("Index", new { firstDocument = firstDocLangId.Value.ToString(), secondDocument = secondDocLangId.Value.ToString() });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
