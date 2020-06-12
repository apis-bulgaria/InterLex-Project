namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SearchDetails
    {
        public string GlobalId { get; set; }

        public SearchDetailsLaw Law { get; set; }

        public SearchDetailsCases Cases { get; set; }

        public SearchDetailsFinances Finances { get; set; }

        public SearchDetailsMultiDict MultiDict { get; set; }
    }
}
