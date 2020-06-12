using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.DataLayer;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class Prag
    {
        public static string GetPrag(Guid menuId, int langId)
        {
            return DB.GetFinsStaticHtml(menuId, langId);
        }
    }
}
