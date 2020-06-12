using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public enum RecentDocDatePeriod
    { 
        All = 0,
        Today = 1,
        Yesterday = 2,
        LastWeek = 3,
        LastMonth = 4

    }
    public class RecentDocFilters
    {
        public bool? Pinned { get; set; }
        public int? DocType { get; set; }
        public RecentDocDatePeriod Period { get; set; }

        public RecentDocFilters()
        {
            Pinned = null;
            DocType = null;
            Period = RecentDocDatePeriod.All;
        }
    }

    public class RecentDocuments
    {
        public List<RecentDoc> Items { get; set; }

        public RecentDocFilters Filters { get; set; }

        public int ResultCount { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }

        public RecentDocuments()
        {
            Items = new List<RecentDoc>();
            OrderBy = "open_date";
            OrderDir = "desc";

            Filters = new RecentDocFilters();
        }
    }
}
