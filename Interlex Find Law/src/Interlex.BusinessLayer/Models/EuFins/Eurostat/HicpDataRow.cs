using Interlex.DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 1.7. ОБЩ ХИПЦ
    /// Harmonized Indices of Consumer Prices
    /// </summary>
    public class HicpDataRow : EurostatDataRow
    {
        private static string statisticType = "teicp000";

        public static IEnumerable<EurostatDataRow> GetI2005DataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Индекс 2005=100
            string tableType = "I15";//"I2015";
            return GetEurostatDataRows(statisticType, null, tableType, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetPchM12DataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Годишна инфлация: съответният месец от предходната година=100, проценти
            string tableType = "PCH_M12";
            return GetEurostatDataRows(statisticType, null, tableType, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetPchM1DataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Месечна инфлация: предходният месец=100, проценти
            string tableType = "PCH_M1";
            return GetEurostatDataRows(statisticType, null, tableType, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetPchMV12DataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Средногодишна инфлация: предходните 12 месеца=100, проценти
            string tableType = "PCH_MV12";

            return GetEurostatDataRows(statisticType, null, tableType, dateFrom, dateTo, langId);
        }
    }
}
