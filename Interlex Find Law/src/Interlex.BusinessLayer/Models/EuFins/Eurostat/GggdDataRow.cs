using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 1.4. БРУТЕН ДЪРЖАВЕН ДЪЛГ
    /// General government gross debt
    /// </summary>
    public class GggdDataRow :EurostatDataRow
    {
        private static string statisticType = "tsdde410";

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
