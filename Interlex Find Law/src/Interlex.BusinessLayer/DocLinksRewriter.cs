namespace Interlex.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.IO;

    public class DocLinksRewriter
    {
        private static readonly Lazy<Dictionary<String, String>> nationalLegislationMapProvider =
            new Lazy<Dictionary<string, string>>(isThreadSafe: true, valueFactory: () => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        private static readonly Object nationalLegislationInitLock = new object();

        private const string IN_LINKS_MATCHING_PATTERN = "<(?:ref|a)[^<>]+?(?:((?:href|class)\\=\"[^\"]+)\")[^<>]+?(?:((?:href|class)\\=\"[^\"]+)\")[^<>]*>";
        private readonly string[] IN_LINKS_CLASSES_TO_FIND_INDIVIDUAL = { "doc-info-link", "d-c-apisweb", "d-apisweb" };
        private readonly string[] IN_LINKS_CLASSES_TO_FIND_GROUPPED = { "d-apis", "d-ref", "d-inline" };
        private readonly string[] VALID_LINK_TYPES = { "celex", "doclangid", "docnr", "hdappnr", "guid", "base", "natlegi" };
        private const string REWRITE_IGNORE_CLASS = "rewrite-ignore";

        private int? langId;
        private string appRootFolder;
        private bool linksInNewTab = true;

        public DocLinksRewriter(int? langId, string appRootFolder, bool linksInNewTab = true, String pathToNationalLegislationMap = "")
        {
            this.langId = langId;
            this.appRootFolder = appRootFolder;
            this.linksInNewTab = linksInNewTab;

            InitNationalLegislationMapIfNeeded(pathToNationalLegislationMap);
        }

        public string RewriteDocInLinks(string originalHTML, string linkClass, bool isInDocumentText)
        {
            var Regex = new Regex(IN_LINKS_MATCHING_PATTERN);

            var recognizedLinks = Regex.Matches(originalHTML);
            var newHtml = originalHTML;
            if (recognizedLinks.Count > 0)
            {
                newHtml = Regex.Replace(originalHTML, new MatchEvaluator(delegate (Match match)
                {
                    return this.RegexReplacementInLinks(match, isInDocumentText);
                }));
            }

            return newHtml;
        }

        private string RegexReplacementInLinks(Match m, bool isInDocumentText)
        {
            var wholeLink = m.Groups[0].ToString();
            wholeLink = wholeLink.Replace("&amp;amp;", "&");
            wholeLink = wholeLink.Replace("&amp;", "&");
            var linkClass = String.Empty;
            if (m.Groups[1].ToString().Contains("class="))
            {
                linkClass = m.Groups[1].ToString();
            }
            else
            {
                linkClass = m.Groups[2].ToString();
            }

            linkClass = linkClass.Replace("class=\"", String.Empty);
            var originalClass = linkClass;

            // checking if it is inlink
            if (linkClass.Contains(REWRITE_IGNORE_CLASS)
                || !((IN_LINKS_CLASSES_TO_FIND_INDIVIDUAL.Any(linkClass.Contains)) || (IN_LINKS_CLASSES_TO_FIND_GROUPPED.All(linkClass.Contains))))
            {
                return wholeLink;
            }

            var linkHref = String.Empty;
            if (m.Groups[2].ToString().Contains("href="))
            {
                linkHref = m.Groups[2].ToString();
            }
            else
            {
                linkHref = m.Groups[1].ToString();
            }

            linkHref = linkHref.Replace("href=\"", String.Empty);
            linkHref = linkHref.Replace("./", String.Empty).Replace("&amp;amp;", "&").Replace("&amp;", "&");
            wholeLink = wholeLink.Replace("./", String.Empty);
            var originalHref = linkHref;

            if (linkHref.StartsWith("web.apis.bg") || linkHref.StartsWith("http://web.apis.bg") || linkHref.StartsWith("https://web.apis.bg") || linkHref.StartsWith("http://www.web.apis.bg") || linkHref.StartsWith("https://www.web.apis.bg"))
            {
                wholeLink = this.ApplyApisWebStyle(wholeLink);
                return wholeLink;
            }

            // preparing information
            var linkParts = linkHref.Split('&').Where(lp => !String.IsNullOrEmpty(lp)).ToArray();
            var toDocParts = linkParts[0].Split('=');
            string linkType = toDocParts[0].ToLower();

            // checking valid link type
            if (!VALID_LINK_TYPES.Any(lt => lt.ToLower() == linkType))
            {
                linkClass += " type-undef";
                wholeLink = wholeLink.Replace(originalClass, linkClass);

                return wholeLink;
                // throw new ArgumentException("INVALID IN_LINK TYPE"); // consider making custom exception; consider logging this somewhere
            }

            string toDocNumber = toDocParts[1];
            wholeLink = this.AppendDataAttribute(wholeLink, "dn", toDocNumber);

            string[] toParArr = null;
            string toParTitle = null;
            string toPar = null;
            if (linkParts.Length > 1 && !String.IsNullOrEmpty(linkParts[linkParts.Length - 1])) // consider checking for any empty linkpart
            {
                toParArr = linkParts[1].Split('=');
                toParTitle = toParArr[0].ToLower();
                toPar = toParArr[1];
            }

            if (linkType != "base")
            {
                linkClass = linkClass + " type-" + linkType;

                if (linkType == "celex" && toPar != null && this.CheckToParContainsFaultyName(toPar) || linkType == "natlegi")
                {
                    linkClass = linkClass + " no-hint";
                }
            }
            else
            {
                linkClass = linkClass + " type-apisweb";


                wholeLink = this.ApplyApisWebStyle(wholeLink);
            }

            linkClass = linkClass + " inline_link";
            wholeLink = wholeLink.Replace(originalClass, linkClass);

            // adding toPar data attribute
            if (!String.IsNullOrEmpty(toPar))
            {
                // adding toPar always except for doclangid; TODO: check this!
                if (linkType != "doclangid")
                {
                    wholeLink = this.AppendDataAttribute(wholeLink, "topar", toPar);
                }
                else if (toParTitle.ToLower() != "searchid") // if not -> toPar is later appended to href
                {
                    wholeLink = this.AppendDataAttribute(wholeLink, "topar", toPar);
                }
            }

            var hasNoConsClass = false;
            if (linkClass.Contains("d-no-cons"))
            {
                hasNoConsClass = true;
            }

            //TODO: check for consolidated
            var newHref = this.ConstructInLinkHrefBasedOnType(linkType, toDocNumber, toParTitle, toPar, linkHref, hasNoConsClass);

            wholeLink = wholeLink.Replace(originalHref, newHref);

            //add target blank
            if (this.linksInNewTab)
            {
                wholeLink = wholeLink.Replace(">", "target=\"_blank\" >");
            }

            if (this.appRootFolder != "" && this.appRootFolder != "/" && !newHref.StartsWith("http"))
            {
                wholeLink = wholeLink.Replace("href=\"", "href=\""+this.appRootFolder);
            }

            return wholeLink;
        }

        private string ConstructInLinkHrefBasedOnType(string linkType, string toDocNumber, string toParTitle, string toPar, string originalLink, bool hasNoConsClass)
        {
            toDocNumber = toDocNumber.Replace("/", "_");

            var constructedHref = new StringBuilder();

            // using ifs; switch is ugly.
            if (linkType == "celex" || linkType == "docnr" || linkType == "hdappnr")
            {
                constructedHref.Append("/Doc/Act/");

                if (linkType == "celex" && !hasNoConsClass)
                {
                    constructedHref.Append("LastCons/");
                }

                constructedHref.Append(this.langId + "/" + toDocNumber.Replace('/', '_').Replace('\\', '_'));

                if (!String.IsNullOrEmpty(toPar))
                {
                    constructedHref.Append("/" + toPar);
                    if (linkType == "celex" && toDocNumber.StartsWith("6"))
                    {
                        constructedHref.Append("#" + toPar);
                    }
                }
            }
            else if (linkType == "guid")
            {
                constructedHref.Append("/Doc/ActByIdentifier/" + toDocNumber);

                if (!String.IsNullOrEmpty(toPar))
                {
                    constructedHref.Append("/" + toPar);
                }
            }
            // v.o [30.05.2017] added handler for the national legislation references
            else if (linkType == "natlegi")
            {
                if (nationalLegislationMapProvider.Value.ContainsKey(toDocNumber))
                {
                    constructedHref.Append(nationalLegislationMapProvider.Value[toDocNumber]);
                    if (!String.IsNullOrEmpty(toPar))
                    {
                        constructedHref.Append($"#{toPar}");
                    }
                }
                else
                {
                    constructedHref.Append(originalLink);
                }
            }
            else if (linkType == "doclangid")
            {
                constructedHref.Append("/Doc/Act/Id/" + toDocNumber);

                if (!String.IsNullOrEmpty(toPar))
                {
                    if (toParTitle.ToLower() == "searchid")
                    {
                        constructedHref.Append("/" + toPar);
                    }
                }
            }
            else if (linkType == "base")
            {
                constructedHref.Append("http://web.apis.bg/p.php?" + originalLink);
            }
            else
            {
                throw new ArgumentException("INVALID IN_LINK TYPE"); // consider making custom exception; consider logging this somewhere
            }

            return constructedHref.ToString();
        }

        private string AppendDataAttribute(string originalLink, string dataName, string value)
        {
            var newLink = originalLink.Replace(">", "data-" + dataName + "=\"" + value + "\" >");
            return newLink;
        }

        private string ApplyApisWebStyle(string originalLink)
        {
            var newLink = originalLink;

            if (!originalLink.Contains("style="))
            {
                newLink = originalLink.Replace(">", "style=\"color: #F7941F;\" >");
            }

            return newLink;
        }

        private bool CheckToParContainsFaultyName(string toPar)
        {
            var r = false;

            if (toPar.Contains("part") || toPar.Contains("tit") || toPar.Contains("chap") || toPar.Contains("sec"))
            {
                r = true;
            }

            return r;
        }

        private static void InitNationalLegislationMapIfNeeded(String pathToNationalLegislationMap)
        {
            lock (nationalLegislationInitLock)
            {
                var isRealPath = !String.IsNullOrEmpty(pathToNationalLegislationMap) && File.Exists(pathToNationalLegislationMap);
                var isNotAlreadyInitied = !nationalLegislationMapProvider.IsValueCreated;

                if (isRealPath && isNotAlreadyInitied)
                {
                    try
                    {
                        var pairsQuery = from line in File.ReadAllLines(pathToNationalLegislationMap)
                                         let split = line.Split('~')
                                         select new
                                         {
                                             code = split[0],
                                             url = split[1]
                                         };

                        foreach (var pair in pairsQuery)
                        {
                            nationalLegislationMapProvider.Value.Add(key: pair.code, value: pair.url);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
