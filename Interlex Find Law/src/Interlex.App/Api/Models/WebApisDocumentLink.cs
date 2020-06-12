using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apis.Common.Extensions;
using Interlex.App.Api.Classes;

namespace Interlex.App.Api.Models
{
    internal class WebApisDocumentLink : DocumentLink
    {
        internal static String CreateApisUrl(String uniqueId)
        {
            return $"{WebApisRequest.webapisdomain}/p.php?i={uniqueId}&b=0";
        }

        internal static String CreateApisUrl(String code, String @base)
        {
            return $"{WebApisRequest.webapisdomain}/p.php?Base={@base}&DocCode={code}";
        }

        private readonly string uniqueId;
        private readonly string domain;

        internal WebApisDocumentLink(string title, string officialUrl, string publisher, string uniqueId, string domain) : base(title, officialUrl, publisher)
        {
            this.uniqueId = uniqueId;
            this.domain = domain;
        }

        internal override string GetUrl()
        {
            return this.OfficialUrl;

            // return CreateApisUrl(this.uniqueId);
        }

        internal override bool IsCase()
        {
            return this.domain.IsIn(StringComparison.OrdinalIgnoreCase, "bgcl", "eucl");
        }

        internal override bool IsLegislation()
        {
            return this.domain.IsIn(StringComparison.OrdinalIgnoreCase, "bgl", "eul");
        }
    }
}