namespace Interlex.BusinessLayer.Models.EuFins
{
    using DataLayer;
    using System;

    public class FinancialCrysisGlossaryModel
    {
        public static string GetFinancialCrysisGlossary(Guid menuId, int langId)
        {
            return DB.GetFinsStaticHtml(menuId, langId);
        }
    }
}
