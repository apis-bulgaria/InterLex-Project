using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class Pager
    {
        public static List<PagerPage> RefreshPager(int recordCount, int currentPage, int pageSize, int visiblePagesCount)
        {
            List<PagerPage> pager = new List<PagerPage>();
            if (recordCount > 0)
            {
                int totalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(recordCount) / Convert.ToDecimal(pageSize)));

                int pageFrom = (currentPage - (visiblePagesCount / 2) >= 0) ? currentPage - (visiblePagesCount / 2) : 1;
                int pageTo = (currentPage + (visiblePagesCount / 2) <= totalPages) ? currentPage + (visiblePagesCount / 2) : totalPages;

                int prevPageFreeLinks = visiblePagesCount / 2 - (currentPage - pageFrom);
                int nextPageFreeLinks = visiblePagesCount / 2 - (pageTo - currentPage);

                pageFrom -= ((pageFrom - nextPageFreeLinks) >= 0) ? nextPageFreeLinks : 0;
                pageTo += ((pageTo + prevPageFreeLinks) <= totalPages) ? prevPageFreeLinks : 0;

                if (pageFrom > 1)
                {
                    pager.Add(new PagerPage() { PageName = "<<", PageNo = "1", Selected = false });

                    int prevPage = (currentPage - visiblePagesCount > 0) ? currentPage - visiblePagesCount : 1;
                    pager.Add(new PagerPage() { PageName = "<", PageNo = prevPage.ToString(), Selected = false });
                }

                for (int i = pageFrom; i <= pageTo; i++)
                {
                    bool selected = (i == currentPage) ? true : false;
                    pager.Add(new PagerPage() { PageName = i.ToString(), PageNo = i.ToString(), Selected = selected });
                }

                if (pageTo < totalPages)
                {
                    int nextPage = (currentPage + visiblePagesCount < totalPages) ? currentPage + visiblePagesCount : totalPages;

                    pager.Add(new PagerPage() { PageName = ">", PageNo = nextPage.ToString(), Selected = false });
                    pager.Add(new PagerPage() { PageName = ">>", PageNo = totalPages.ToString(), Selected = false });
                }
            }
            return pager;
        }
    }
}
