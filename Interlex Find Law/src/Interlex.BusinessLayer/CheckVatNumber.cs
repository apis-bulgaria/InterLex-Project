using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckEUVatNumber;

namespace Interlex.BusinessLayer
{
    public class CheckVatNumber
    {
        public static VatCheckResult Check(string countryAbr, string vatNumber, int timeoutMiliseconds)
        {
            return Methods.CheckVat(countryAbr, vatNumber, timeoutMiliseconds);
        }
    }
}
