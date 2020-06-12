using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Models
{
    public class PagerPage
    {
        public string PageName
        {
            get;
            set;
        }

        public string PageNo
        {
            get;
            set;
        }

        public bool Selected
        {
            get;
            set;
        }
    }
}