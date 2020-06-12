namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Period} Count: {Count}")]
    public class DocumentInfoForPeriod
    {
        public YearMonth Period { get; private set; }
        public int Count { get; private set; }

        public DocumentInfoForPeriod(YearMonth period, int count)
        {
            this.Period = period;
            this.Count = count;
        }
    }
}
