using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Models.EuFins
{
    public class FinsModel
    {
        public DateTime MaxDate { get; set; }

        public List<string> Options { get; set; }
    }
}