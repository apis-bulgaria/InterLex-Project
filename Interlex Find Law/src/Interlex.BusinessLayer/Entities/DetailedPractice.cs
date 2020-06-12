using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interlex.BusinessLayer.Models;

namespace Interlex.BusinessLayer.Entities
{
    public class DetailedPractice
    {
        public string DetailName
        {
            get;
            set;
        }
        public string DetailId
        {
            get;
            set;
        }
        public List<SearchListItem> SearchItems
        {
            get;
            set;
        }

        public bool HasMore
        {
            get;
            set;
        }
    }
}