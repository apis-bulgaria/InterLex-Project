using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class ConsVersion
    {
        public int? DocLangId { get; set; }
        public int? LeadDocLangId { get; set; }
        public int? Type { get; set; }
        public DateTime? Date { get; set; }
        private string _celex;
        public string Celex
        {
            get
            {
                return this._celex;
            }
            set
            {
                this._celex = value.ToUpper();
            }
        }

        public int? LangId { get; set; }
        public bool IsLast { get; set; }

    }
}
