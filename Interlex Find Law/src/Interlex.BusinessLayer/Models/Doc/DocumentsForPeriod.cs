namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Apis.Common.Extensions;
    using Enums;

    [DebuggerDisplay("{Period} Items: {Items.Count}")]
    public class DocumentsForPeriod
    {
        public YearMonth StartPeriod { get; private set; }
        public YearMonth EndPeriond { get; private set; }
        public IReadOnlyCollection<Document> Items { get; private set; }
        public IReadOnlyCollection<DocumentInfoForPeriod> PeriodsInfo { get; set; }

        public DocumentsForPeriod(YearMonth startPeriod, YearMonth endPeriond, IReadOnlyCollection<Document> items, IReadOnlyCollection<DocumentInfoForPeriod> periodsInfo)
        {
            this.StartPeriod = startPeriod;
            this.EndPeriond = endPeriond;
            this.Items = items.ToList();
            this.PeriodsInfo = periodsInfo;
        }

        public IEnumerable<IGrouping<int, DocumentInfoForPeriod>> GetLastGroupPeriodByYear()
        {
            return this.PeriodsInfo.GroupBy(x => x.Period.Year).FirstOrDefault()?.ToEnumerable() ?? Enumerable.Empty<IGrouping<int, DocumentInfoForPeriod>>();
        }

        public IEnumerable<IGrouping<int, DocumentInfoForPeriod>> GetRestGroupPeriodsByYear()
        {
            return this.PeriodsInfo.GroupBy(x => x.Period.Year).Skip(1);
        }

        public bool IsAnyRestGroupInPeriod()
        {
            return this.GetRestGroupPeriodsByYear().Any(x => x.Key == this.StartPeriod.Year || x.Key == this.EndPeriond.Year);
        }
    }
}
