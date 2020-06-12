using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Models
{
    public class SearchListItem: ListItem
    {
        public DateTime Date { get; set; }

        public DateTime LastOpenDate { get; set; }
    }
}