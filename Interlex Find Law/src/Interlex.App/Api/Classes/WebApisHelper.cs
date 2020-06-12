using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Apis.Common.Html.Extension;
using Apis.Common.Extensions;

namespace Interlex.App.Api.Classes
{
    internal static class WebApisHelper
    {
        /// <summary>
        /// Removes images, replaces anchors with spans and removes all king of on.... events (like onmouseover)
        /// </summary>
        /// <param name="html">original html</param>
        /// <returns></returns>
        internal static string SimplifyHtml(String html)
        {
            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var images = htmlDocument.DocumentNode.SafeSelectNodes(".//img");
                images.SafeRemove();

                var anchors = htmlDocument.DocumentNode.SafeSelectNodes(".//a");
                foreach (var anchor in anchors)
                {
                    //anchor.Name = "span";
                    if (!String.IsNullOrEmpty(anchor.GetAttributeValue("href", string.Empty)))
                    {
                        anchor.Attributes["href"].Value = $"{WebApisRequest.webapisdomain}/{anchor.Attributes["href"].Value}";
                        anchor.SetAttributeValue("target", "_blank");
                    }
                }

                // gets any node which contains attribute with name that starts with 'on'
                var eventBaseElements = htmlDocument.DocumentNode.SafeSelectNodes(".//*[./@*[starts-with(local-name(.), 'on')]]");

                foreach (var eventBaseElement in eventBaseElements)
                {
                    var events = eventBaseElement.Attributes.Where(x => x.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase)).ToList();
                    events.SafeRemove();
                }

                var simplifiedHtml = htmlDocument.DocumentNode.OuterHtml;
                simplifiedHtml = NormalizeProductTitle(simplifiedHtml);

                return simplifiedHtml;

            }
            catch (Exception e)
            {
                return html;
            }
        }

        internal static String MapNormalDomainToApisDomain(String normalDomain)
        {
            normalDomain = normalDomain.ToLower();

            switch (normalDomain)
            {
                case "all": return "bgall";
                case "eucl":
                case "natcl": return "bgcl";
                case "eul":
                case "natl": return "bgl";
                default: return normalDomain;
            }
        }

        internal static String MapNormalDomainToApisForEuAct(String normalDomain)
        {
            normalDomain = normalDomain.ToLower();

            if (normalDomain == "all")
            {
                normalDomain = "euall";
            }
            else if (normalDomain == "natcl")
            {
                normalDomain = "bgcl";
            }


            return normalDomain;
        }

        private static String NormalizeProductTitle(String html)
        {
            var titles = new[] { "Aпис - ПРАВО", "Евро Право" };

            foreach (var title in titles)
            {
                if (html.StartsWith(title))
                {
                    html = html.SafeReplaceAtStart(title, String.Empty);
                    html = $"<div class='apis-hint-wrapper'><p class='hint-header'>{title}</p>{html}</div>";

                    break;
                }
            }

            return html;
        }
    }
}