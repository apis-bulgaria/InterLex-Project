using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 1.6. ОБЩ ДЪРЖАВЕН ДЕФИЦИТ / ИЗЛИШЪК
    /// General government deficit/surplus
    /// </summary>
    public class GgdsDataRow : EurostatDataRow
    {
        private static string statisticType = "tec00127";

        public static IEnumerable<EurostatDataRow> GetPcgdpDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Процент от брутния вътрешен продукт (БВП)
            string tableType = "PC_GDP";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetMioeurDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Милиони евро
            string tableType = "MIO_EUR";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }
    }
}
