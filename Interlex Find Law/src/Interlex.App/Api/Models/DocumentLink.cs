using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Interlex.App.Api.Models
{
    [DebuggerDisplay("{Title}")]
    internal abstract class DocumentLink
    {
        internal String Title { get; private set; }
        internal String OfficialUrl { get; private set; }
        internal String Publisher { get; private set; }
        
        internal DocumentLink(String title, String officialUrl, String publisher)
        {
            this.Title = title;
            this.OfficialUrl = officialUrl;
            this.Publisher = publisher;
        }

        internal abstract String GetUrl();
        internal abstract bool IsLegislation();
        internal abstract bool IsCase();
    }
}