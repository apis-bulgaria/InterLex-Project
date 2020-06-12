using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models.EuFins.Eurostat
{
    /// <summary>
    /// 1.1. БВП НА ЧОВЕК ОТ НАСЕЛЕНИЕТО В СТАНДАРТИ НА ПОКУПАТЕЛНАТА СПОСОБНОСТ
    /// GDP per capita in PPS
    /// </summary>
    public class GdpPerCapitaDataRow : EurostatDataRow
    {

        private static string statisticType = "tec00114";

        public static IEnumerable<EurostatDataRow> GetGdpPerCapitaDataRows(DateTime dateFrom, DateTime dateTo, int langId)
        {
            // Верижни обеми (2010 г.), евро на глава от населението
            return GetEurostatDataRows(statisticType, null, null, dateFrom, dateTo, langId);
        }


    }
}
