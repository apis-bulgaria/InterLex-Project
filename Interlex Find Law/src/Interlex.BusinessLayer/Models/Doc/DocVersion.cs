using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class DocVersion
    {
        public string DocNumber { get; set; }
        public int DocLangId { get; set; }
        public int LangId { get; set; }
        public DateTime? PublDate { get; set; }
    }
}
