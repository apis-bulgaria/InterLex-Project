using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Apis.Common.Extensions;
using Interlex.BusinessLayer.Models;
using Interlex.App.Api.Models;
using Newtonsoft.Json;

namespace Interlex.App.Api.Classes
{
    internal static class WebApisRequest
    {
        internal static readonly String webapisdomain = "http://web.apis.bg";
        // internal static readonly string webapisdomain = "http://87.121.111.213";

        internal class WebApisDocLinksResponse
        {
            public List<DocumentLink> Links { get; set; }
            public int Full_Count { get; set; }
            public String All_Url { get; set; }
            public String Caption { get; set; }
        }

        private class link_wrapper_json_friendly
        {
            public string doc_caption { get; set; }
            public string all_url { get; set; }
            public int full_count { get; set; }
            public List<link_json_friendly> links { get; set; }
        }

        private class link_json_friendly
        {
            public string title { get; set; }
            public string unique_id { get; set; }
            public string domainIs { get; set; }
            public string doc_url { get; set; }
        }

        internal static WebApisDocLinksResponse GetDocLinks(String domain, int limit, String code, String @base, String toPar)
        {
            var url = $"{webapisdomain}/euLinksChecker.php?domain={domain}&limit={limit}&code={code}&base={@base}&toPar={toPar}";

            try
            {
                var responseContentJson = GetAsString(url);

                var deserilizedResponseContent = JsonConvert.DeserializeObject<link_wrapper_json_friendly>(responseContentJson);

                var linksResult = deserilizedResponseContent
                    .links
                    .Select(x => new WebApisDocumentLink(title: x.title, officialUrl: $"{webapisdomain}/{x.doc_url}", publisher: String.Empty, uniqueId: x.unique_id, domain: x.domainIs));

                var result = new WebApisDocLinksResponse
                {
                    All_Url = $"{webapisdomain}/{deserilizedResponseContent.all_url}",
                    Full_Count = deserilizedResponseContent.full_count,
                    Links = new List<DocumentLink>(linksResult),
                    Caption = deserilizedResponseContent.doc_caption ?? String.Empty,
                };

                return result;
            }
            catch (Exception e)
            {
                // TODO: log

                return null;
            }
        }

        internal static WebApisDocLinksResponse GetEuDocLinks(String domain, int limit, String celex, String toPar)
        {
            var url = $"{webapisdomain}/euLinksChecker.php?&limit={limit}&celex={celex}&domain={domain}&toPar={toPar}";
            try
            {
                var responseContentJson = GetAsString(url);

                var deserilizedResponseContent = JsonConvert.DeserializeObject<link_wrapper_json_friendly>(responseContentJson);

                var linksResult = deserilizedResponseContent
                    .links
                    .Select(x => new WebApisDocumentLink(
                        title: x.title,
                        officialUrl: $"{webapisdomain}/{x.doc_url}",
                        publisher: String.Empty, uniqueId: x.unique_id, domain: x.domainIs));

                var result = new WebApisDocLinksResponse
                {
                    All_Url = $"{webapisdomain}/{deserilizedResponseContent.all_url}",
                    Full_Count = deserilizedResponseContent.full_count,
                    Links = new List<DocumentLink>(linksResult),
                    Caption = deserilizedResponseContent.doc_caption ?? String.Empty,
                };

                return result;
            }
            catch (Exception e)
            {
                // TOOD: log

                return null;
            }
        }

        internal static String Cite(String code, String @base, CiteType citeType)
        {
            try
            {
                var citate = citeType == CiteType.LongCite ? "long" : "short";

                var url = $"{webapisdomain}/euLinksChecker.php?cite=1&citeType={citate}&code={code}&base={@base}";

                var content = GetAsString(url);

                // use anonymous template for deserializtion purpouse. the result does not fit well in 'link_wrapper_json_friendly'
                var template = new { links = new List<link_json_friendly>() };

                var jsonWrapper = JsonConvert.DeserializeAnonymousType(content, template);

                var result = jsonWrapper.links?.FirstOrDefault()?.title;

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        internal static String Hint(String code, String @base, String toPar)
        {
            try
            {
                var url = $"{WebApisRequest.webapisdomain}/euLinksChecker.php?hint=1";
                if (@base.IgnoreCaseEquals("apev"))
                {
                    url = $"{url}&celex={code}";
                }
                else
                {
                    url = $"{url}&code={code}&base={@base}";
                }

                if (String.IsNullOrEmpty(toPar) == false)
                {
                    url = $"{url}&toPar={toPar}";
                }

                var content = GetAsString(url);

                // use anonymous template for deserializtion purpouse. the result does not fit well in 'link_wrapper_json_friendly'
                var template = new { links = new List<link_json_friendly>() };

                var jsonWrapper = JsonConvert.DeserializeAnonymousType(content, template);

                var result = jsonWrapper.links?.FirstOrDefault()?.title;

                result = WebApisHelper.SimplifyHtml(result);

                return result;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        internal static String Hint(int uniqueId, int dbIndex, String toPar)
        {
            try
            {
                var url = $"{WebApisRequest.webapisdomain}/euLinksChecker.php?hint=1&unqiueId={uniqueId}&db_index={dbIndex}";
                if (String.IsNullOrEmpty(toPar) == false)
                {
                    url = $"{url}&toPar={toPar}";
                }

                var content = GetAsString(url);

                // use anonymous template for deserializtion purpouse. the result does not fit well in 'link_wrapper_json_friendly'
                var template = new { links = new List<link_json_friendly>() };

                var jsonWrapper = JsonConvert.DeserializeAnonymousType(content, template);

                var result = jsonWrapper.links?.FirstOrDefault()?.title;

                result = WebApisHelper.SimplifyHtml(result);

                return result;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        internal static WebApisDocLinksResponse SearchByText(String domain, String text, int limit)
        {
            try
            {
                // http://87.121.111.213/euLinksChecker.php?search=1&limit=2&searchText=вносители

                var url = $"{WebApisRequest.webapisdomain}/euLinksChecker.php?search=1&limit={limit}&searchText={System.Net.WebUtility.UrlEncode(text)}&domain={domain}";

                var responseContentJson = GetAsString(url);
                var deserilizedResponseContent = JsonConvert.DeserializeObject<link_wrapper_json_friendly>(responseContentJson);

                var linksResult = deserilizedResponseContent
                    .links
                    .Select(x => new WebApisDocumentLink(title: x.title, officialUrl: $"{webapisdomain}/{x.doc_url}", publisher: String.Empty, uniqueId: x.unique_id, domain: x.domainIs));

                var result = new WebApisDocLinksResponse
                {
                    All_Url = $"{webapisdomain}/{deserilizedResponseContent.all_url}",
                    Full_Count = deserilizedResponseContent.full_count,
                    Links = new List<DocumentLink>(linksResult)
                };

                return result;
            }
            catch (Exception e)
            {
                // TODO: log

                return null;
            }
        }

        private static String GetAsString(String url)
        {
            using (var client = new HttpClient())
            {
                var result = client.GetStringAsync(url).Result;

                return result;
            }

        }
    }
}