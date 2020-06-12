using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Interlex.App.Api.Models
{
    [DebuggerDisplay("{SourceName}")]
    internal class LinksToHtmlModel
    {
        internal static LinksToHtmlModel Create(IReadOnlyCollection<DocumentLink> links, int totalCount, int limit, String allLinksUrl, String sourceName)
        {
            return new LinksToHtmlModel(links, totalCount, limit, allLinksUrl, sourceName);
        }

        internal IReadOnlyCollection<DocumentLink> Links { get; private set; }
        internal int TotalCount { get; private set; }
        internal int Limit { get; private set; }
        internal String AllLinksUrl { get; private set; }
        internal String SourceName { get; private set; }

        internal LinksToHtmlModel(IReadOnlyCollection<DocumentLink> links, int totalCount, int limit, String allLinksUrl, String sourceName)
        {
            this.Links = links;
            this.TotalCount = totalCount;
            this.AllLinksUrl = allLinksUrl;
            this.Limit = limit;
            this.SourceName = sourceName;
        }
    }
}