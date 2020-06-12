using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 1.2. РЕАЛЕН БВП НА ГЛАВА ОТ НАСЕЛЕНИЕТО И ТЕМП НА ПРИРАСТ
    /// Real GDP per capita, growth rate and totals
    /// </summary>
    public class RealGdp : EurostatDataRow
    {
        private static string statisticType = "tsdec100";

        public static IEnumerable<EurostatDataRow> GetClv10DataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Верижни обеми (2010 г.), евро на глава от населението
            string tableType = "CLV10_EUR_HAB";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }

        public static IEnumerable<EurostatDataRow> GetClvPchDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Верижни обеми, процентна промяна спрямо предходен период, на глава от населението
            string tableType = "CLV_PCH_PRE_HAB";
            return GetEurostatDataRows(statisticType, tableType, null, dateFrom, dateTo, langId);
        }
    }
}
