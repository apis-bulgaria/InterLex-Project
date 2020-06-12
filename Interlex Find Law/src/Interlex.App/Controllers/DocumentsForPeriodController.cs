namespace Interlex.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using ApiFilters;
    using Interlex.BusinessLayer;
    using Interlex.BusinessLayer.Models;

    [UserAuthorize]
    public class DocumentsForPeriodController : BaseController
    {
        public ActionResult Index()
        {
            // at the moment this is valid only for users which has bought product 2 
            if (this.UserData.Products.Any(x => x.ProductId == 2))
            {
                var allPeriods = Doc.GetEuFinsDocumentsPeriods(siteLangId: this.Language.Id).ToList();

                var period = allPeriods.FirstOrDefault()?.Period ?? YearMonth.Now();

                return this.PeriodInternal(period, period, allPeriods);
            }
            else
            {
                return RedirectToAction(actionName: nameof(HomeController.Index), controllerName: nameof(HomeController));
            }
        }

        public ActionResult PeriodForYear(int year)
        {
            var allPeriods = Doc.GetEuFinsDocumentsPeriods(siteLangId: this.Language.Id).ToList();
            var startPeriod = YearMonth.Create(year, 1);
            var endPeriod = YearMonth.Create(year, 12);

            return this.PeriodInternal(startPeriod, endPeriod, allPeriods);
        }

        public ActionResult PeriodForYearAndMonth(int year, int month)
        {
            var allPeriods = Doc.GetEuFinsDocumentsPeriods(siteLangId: this.Language.Id).ToList();

            var period = YearMonth.Create(year, month);

            return this.PeriodInternal(period, period, allPeriods);
        }

        private ActionResult PeriodInternal(YearMonth startPeriod, YearMonth endPeriond, IReadOnlyCollection<DocumentInfoForPeriod> allPeriods)
        {
            var newDocuments = Doc.GetEuFinsNewDocumentsForPeriod(
                siteLangId: this.Language.Id,
                userId: this.UserData.UserId,
                startPeriod: startPeriod,
                endPeriod: endPeriond,
                periodsInfo: allPeriods);

            return this.View(viewName: nameof(DocumentsForPeriodController.Index), model: newDocuments);
        }
    }
}