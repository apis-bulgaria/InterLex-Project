using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 1.3. БРУТЕН ВЪТРЕШЕН ПРОДУКТ ПО ПАЗАРНИ ЦЕНИ
    /// Gross domestic product at market prices
    /// </summary>
    public class GdpDataRow : EurostatDataRow
    {
        private static string statisticType = "tec00001";

        public static IEnumerable<EurostatDataRow> GetEurHabDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Текущи цени, евро на глава от населението
            string tableTypeAffix = "CP_EUR_HAB";
            return GetEurostatDataRows(statisticType, null, tableTypeAffix, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetMppsDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Текущи цени, милион стандарти на покупателна способност
            string tableTypeAffix = "CP_MPPS";
            return GetEurostatDataRows(statisticType, null, tableTypeAffix, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetMeurDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Текущи цени, милиона евро
            string tableTypeAffix = "CP_MEUR";
            return GetEurostatDataRows(statisticType, null, tableTypeAffix, dateFrom, dateTo, langId);
        }
    }
}
