namespace Interlex.BusinessLayer.Models.EuFins
{
    using System;
    using Interlex.DataLayer;
    
    public class TaxInformationModel
    {
        public static string GetTaxInformation(Guid menuId, int langId)
        {
            return DB.GetFinsStaticHtml(menuId, langId);
        }
    }
}
