using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Constant
{
    public static class Constants
    {
        #region PROPERTY

        public static IEnumerable<String> TreatiesConsolidatedVersionTextTranslation { get; private set; }

        public static String TreatiesConsolidatedTextRegexPattern { get; private set; }

        #endregion PROPERTY


        static Constants()
        {
            SetTreatiesConsolidatedVersionTextTranslation();
            SetTreatiesConsolidatedTextRegexPattern();
        }

        private static void SetTreatiesConsolidatedVersionTextTranslation()
        {
            TreatiesConsolidatedVersionTextTranslation = new HashSet<String>()
            {
                 "Консолидирана версия",
                 "Consolidated version",
                 "Konsolidierte Fassung",
                 "Version consolidée",
                 "Versione consolidata",
            };
        }

        private static void SetTreatiesConsolidatedTextRegexPattern()
        {
            // (Консолидирана версия|Consolidated version|Konsolidierte Fassung|Version consolidée|Versione consolidata)(\s)(\d+)

            TreatiesConsolidatedTextRegexPattern = $@"({String.Join("|", TreatiesConsolidatedVersionTextTranslation)})(\s)(\d+)";
        }
    }
}
