using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 2.1. ХАРМОНИЗИРАН КОЕФИЦИЕНТ НА БЕЗРАБОТИЦА ПО ПОЛ
    /// Harmonised unemployment rate by sex
    /// </summary>
    public class HursDataRow : EurostatDataRow
    {
        private static string statisticType = "teilm020";

        public static IEnumerable<EurostatDataRow> GetTDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Общо
            string tableType = "T";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetFDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Мъже
            string tableType = "F";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetMDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Жени
            string tableType = "M";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }
    }
}
