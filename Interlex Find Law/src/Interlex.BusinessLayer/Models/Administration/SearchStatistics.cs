namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Data;

    public struct SearchStatistics
    {
        public long LawSearchesCount { get; set; }
        public long CasesSearchesCount { get; set; }
        public long SimpleSearchesCount { get; set; }
        public long FinancesSearchesCount { get; set; }

    /*    public SearchStatistics(long lawSearchesCount, long casesSearchesCount, long simpleSearchesCount, long financesSearchesCount)
        {
            this.LawSearchesCount = lawSearchesCount;
            this.CasesSearchesCount = casesSearchesCount;
            this.SimpleSearchesCount = simpleSearchesCount;
            this.FinancesSearchesCount = financesSearchesCount;
        }*/

        public SearchStatistics(DataRow r)
        {
            this.SimpleSearchesCount = Convert.ToInt64(r["simple_count"]);
            this.CasesSearchesCount = Convert.ToInt64(r["cases_count"]);
            this.LawSearchesCount = Convert.ToInt64(r["law_count"]);
            this.FinancesSearchesCount = Convert.ToInt64(r["finances_count"]);
        }
    }
}
