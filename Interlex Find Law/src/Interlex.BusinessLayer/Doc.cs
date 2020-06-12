using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Interlex.DataLayer;
using Interlex.BusinessLayer.Models;
using Interlex.BusinessLayer.Entities;
using AkomaNtosoXml.Xslt.Core.Classes.Resolver;
using ApisHtmlHighlight;
using Apis.Common.Celex;
using System.Data;
using Apis.Common.Extensions;
using System.Threading.Tasks;
using AkomaNtosoXml.Xslt.Core.Classes.Model;

namespace Interlex.BusinessLayer
{
    public class Doc
    {
        private static Dictionary<char, char> AlphabetsDict { get; } = new Dictionary<char, char>() 
        // consider these being several arrays, tuples or complex objects when integrating other languages
        {
            {'a', 'а'}, {'A', 'А'},
            {'b', 'б'}, {'B', 'Б'},
            {'c', 'в'}, {'C', 'В'},
            {'d', 'г'}, {'D', 'Г'},
            {'e', 'д'}, {'E', 'Д'},
            {'f', 'е'}, {'F', 'Е'},
            {'g', 'ж'}, {'G', 'Ж'},
            {'h', 'з'}, {'H', 'З'},
            {'i', 'и'}, {'I', 'И'},
            {'j', 'й'}, {'J', 'Й'},
            {'k', 'к'}, {'K', 'К'},
            {'l', 'л'}, {'L', 'Л'},
            {'m', 'м'}, {'M', 'М'},
            {'n', 'н'}, {'N', 'Н'},
            {'o', 'о'}, {'O', 'О'},
            {'p', 'п'}, {'P', 'П'},
            {'q', 'р'}, {'Q', 'Р'},
            {'r', 'с'}, {'R', 'С'},
            {'s', 'т'}, {'S', 'Т'},
            {'t', 'у'}, {'T', 'У'},
            {'u', 'ф'}, {'U', 'Ф'},
            {'v', 'х'}, {'V', 'Х'},
            {'w', 'ц'}, {'W', 'Ц'},
            {'x', 'ч'}, {'X', 'Ч'},
            {'y', 'ш'}, {'Y', 'Ш'},
            {'z', 'щ'}, {'Z', 'Щ'}
           // { "LETZTE", "LAST"},{ "ПОСЛЕДНО", "LAST"}, 
        };

        static Doc()
        {
            ClassifiersProvider.PopulateClassifiersFromCache();
            AkomaNtosoPreProcessor.ClassificationService = ClassifiersProvider.Service;
        }

        public static Document GetDocByDocNumber(string docNumber, int langId, int? userId)
        {
            Document doc = null;
            var row = DB.GetDocByDocNumber(docNumber, langId, userId);
            if (row != null)
            {
                doc = new Document();
                doc.DocNumber = docNumber;
                doc.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                doc.DocType = Convert.ToInt32(row["doc_type"]);
                doc.Title = row["title"].ToString();
                doc.Publisher = row["publisher"].ToString();
                doc.Country = row["country"].ToString();
                doc.LangId = Convert.ToInt32(row["lang_id"]);
            }
            return doc;
        }

        public static Document GetDocument(int docLangId)
        {
            Document doc = null;
            var row = DB.GetDocument(docLangId);
            if (row != null)
            {
                doc = new Document();
                doc.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                doc.DocNumber = row["doc_number"].ToString();
                doc.DocType = Convert.ToInt32(row["doc_type"]);
                doc.Title = row["title"].ToString();
                doc.Country = row["country"].ToString();
            }
            return doc;
        }

        public static Document GetDocByDocIdentifier(string docIdentifier)
        {
            Document doc = null;
            var row = DB.GetDocByDocIdentifier(docIdentifier);
            if (row != null)
            {
                doc = new Document();
                doc.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                doc.DocType = Convert.ToInt32(row["doc_type"]);
                doc.Title = row["title"].ToString();
                doc.LangId = Convert.ToInt32(row["lang_id"]);
                doc.DocNumber = row["doc_number"].ToString();
                //doc = new Document(doc.)

                //doc.HtmlModel = 
            }
            return doc;
        }

        public static Document GetDocHintByDocIdentifier(string docIdentifier, string toPar, int uiLangid)
        {
            Document d = null;
            var row = DB.GetDocHintByIdentifier(docIdentifier, toPar);
            if (row != null && row["doc_lang_id"].ToString() != "")
            {
                d = new Document();
                d.Title = row["title"].ToString();
                d.DocNumber = row["doc_number"].ToString();
                d.Country = row["country"].ToString().ToLower();
                d.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                d.DocType = Convert.ToInt32(row["doc_type"]);
                d.LangId = Convert.ToInt32(row["lang_id"]);

                if (row["content_status"].ToString() == "xslt_process")
                {
                    string parXml = row["par_text"].ToString();
                    d.Text = Doc.TransformFullDocumentForHint(parXml, d.LangId, d.DocLangId, uiLangid);
                    d.BloblInfo = AkomaNtosoPreProcessor.GetBlobInfo(parXml);
                }
                else if (row["content_status"].ToString() == "xslt_process")
                {
                    d.Text = row["par_text"].ToString();
                }
            }

            return d;
        }

        /// <summary>
        /// Process part of Akoma Ntoso xml document. By example: article fragment
        /// </summary>
        /// <param name="strXML"></param>
        /// <param name="titles">Current language title names</param>
        /// <returns></returns>
        public static string TransformXml(string strXML, int langId, int docLangId, int uiLangId)
        {
            var config = new AkomaNtosoPreProcessorConfig
            {
                DocumentLanguage = langId,
                UILanguage = uiLangId,
                AkomaNtosoXmlId = docLangId,
            };

            return AkomaNtosoPreProcessor.ConvertToHtml(strXML, config);
        }

        public static string TransformFullDocumentForHint(string strXML, int langId, int docLangId, int uiLangId)
        {
            var config = new AkomaNtosoPreProcessorConfig
            {
                DocumentLanguage = langId,
                UILanguage = uiLangId,
                AkomaNtosoXmlId = docLangId
            };

            return AkomaNtosoPreProcessor.ConvertToHintHtml(strXML, config);
        }

        public static string TransformFullDocumentForHint(string strXML, int langId, int docLangId, int uiLangId, string citatedEId)
        {
            var config = new AkomaNtosoPreProcessorConfig
            {
                DocumentLanguage = langId,
                UILanguage = uiLangId,
                AkomaNtosoXmlId = docLangId
            };

            return AkomaNtosoPreProcessor.ConvertToHintHtml(strXML, config, citatedEId);
        }

        public static void AddRecentDoc(int userId, int docId, int maxCount, int productId)
        {
            DB.AddRecentDoc(userId, docId, maxCount, productId);
        }

        public static void AddOpenedDoc(int userId, int docId, int productId)
        {
            DB.AddOpenedDoc(userId, docId, productId);
        }

        public static void SetRecentDocPin(int id, bool pinned)
        {
            DB.SetRecentDocPin(id, pinned);
        }

        public static RecentDocuments GetRecentDocs(int userId, int siteLangId, bool? pinned, int? docType, RecentDocDatePeriod period, string orderBy, string orderDir, int productId)
        {
            RecentDocuments rd = new RecentDocuments();
            foreach (var row in DB.GetRecentDocs(userId, siteLangId, pinned, docType, (int)period, orderBy, orderDir, productId))
            {
                RecentDoc doc = new RecentDoc();
                doc.RecentDocId = Convert.ToInt32(row["id"]);
                doc.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                doc.Country = row["country"].ToString();
                doc.DocType = Convert.ToInt32(row["doc_type"]);
                doc.LangId = Convert.ToInt32(row["lang_id"]);
                doc.DocDate = Convert.ToDateTime(row["doc_date"]);
                doc.Title = row["full_title"].ToString();
                doc.DocNumber = row["doc_number"].ToString();
                doc.Pinned = Convert.ToBoolean(row["pinned"]);
                // doc.OpenDate = Convert.ToDateTime(row["open_date"]);
                doc.OpenDate = row["open_date"].ToString();
                doc.Publisher = row["publisher"].ToString();

                // Keywords
                Type keywordsT = row["keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    doc.SetKeywords((string[,])row["keywords"], siteLangId);

                // Summaries
                Type summariesT = row["summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    doc.SetSummaries((string[,])row["summaries"], siteLangId);

                //doc.Keywords = (String[])row["keywords"];
                doc.UserDocId = row["user_doc_id"].ToString();


                rd.Items.Add(doc);
            }

            rd.Filters.Pinned = pinned;
            rd.Filters.DocType = docType;
            rd.Filters.Period = period;
            rd.OrderBy = orderBy;
            rd.OrderDir = orderDir;

            return rd;
        }

        public static DocumentsForPeriod GetEuFinsNewDocumentsForPeriod(int siteLangId, int userId, YearMonth startPeriod, YearMonth endPeriod, IReadOnlyCollection<DocumentInfoForPeriod> periodsInfo)
        {
            var startDate = YearMonth.ToStartDate(startPeriod);
            var endDate = YearMonth.ToEndDate(endPeriod);

            var documents = new List<Document>();
            foreach (var row in DB.GetNewEuFinsDocuments(siteLangId, userId, startDate, endDate))
            {
                var document = new Document();
                document.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                document.Country = row["country"].ToString();
                document.DocType = Convert.ToInt32(row["doc_type"]);
                document.LangId = Convert.ToInt32(row["lang_id"]);
                document.DocDate = Convert.ToDateTime(row["doc_date"]);
                document.Title = row["full_title"].ToString();
                document.DocNumber = row["doc_number"].ToString();
                document.Publisher = row["publisher"].ToString();

                // Keywords
                Type keywordsT = row["keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    document.SetKeywords((string[,])row["keywords"], siteLangId);

                // Summaries
                Type summariesT = row["summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    document.SetSummaries((string[,])row["summaries"], siteLangId);

                documents.Add(document);
            }

            var periodResult = new DocumentsForPeriod(startPeriod, endPeriod, documents, periodsInfo);

            return periodResult;
        }


        public static IOrderedEnumerable<DocumentInfoForPeriod> GetEuFinsDocumentsPeriods(int siteLangId)
        {
            var records = DB.GetEuFinsDocumentsPeriods(siteLangId);
            var dateOfeffects = records.Select(r => DateTime.Parse($"{r["date_of_effect"]}"));
            var periods = dateOfeffects.Select(YearMonth.Create);

            var periodsInfo = periods
                .GroupBy(x => x, (period, elements) => new DocumentInfoForPeriod(period, elements.Count()))
                .OrderByDescending(x => x.Period);

            return periodsInfo;
        }

        public static List<Document> GetNewDocs(int userId, int siteLangId, int productId)
        {
            List<Document> l = new List<Document>();
            foreach (var row in DB.GetNewDocs(userId, siteLangId, productId))
            {
                Document doc = new Document();
                doc.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                doc.Country = row["country"].ToString();
                doc.DocType = Convert.ToInt32(row["doc_type"]);
                doc.LangId = Convert.ToInt32(row["lang_id"]);
                doc.DocDate = Convert.ToDateTime(row["doc_date"]);
                doc.Title = row["full_title"].ToString();
                doc.DocNumber = row["doc_number"].ToString();
                doc.Publisher = row["publisher"].ToString();

                // Keywords
                Type keywordsT = row["keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    doc.SetKeywords((string[,])row["keywords"], siteLangId);

                // Summaries
                Type summariesT = row["summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    doc.SetSummaries((string[,])row["summaries"], siteLangId);

                doc.UserDocId = row["user_doc_id"].ToString();


                l.Add(doc);
            }

            return l;
        }

        public static string GetDocText(int docLangId, int docType, bool plainXml, int productId, bool showFreeDocuments)
        {
            //string str = DB.GetDocText(docLangId, plainXml);
            //return AddDocMetaTags(DB.GetDocText(docLangId, plainXml, productId), docType);

            return DB.GetDocText(docLangId, plainXml, productId, showFreeDocuments);
        }

        public static void TestDocText()
        {
            var docLangIds = new List<int>();
            foreach (var row in DB.GetAllDocLangIds())
            {
                docLangIds.Add(Convert.ToInt32(row["doc_lang_id"]));
            }

            var good = new List<int>();
            var faulty = new List<int>();

            Parallel.ForEach(docLangIds, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (docLangId) =>
             {
                 try
                 {
                     DB.GetDocTextTest(docLangId);
                     good.Add(docLangId);
                 }
                 catch (Exception)
                 {
                     faulty.Add(docLangId);
                 }
             });

            var debugVar = 5;
        }

        /// <summary>
        /// Не се използва при зареждане на документи, защото е проблем при голям обем от текст (OutOfMemory)
        /// </summary>
        /// <param name="strXml"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        private static string AddDocMetaTags(string strXml, int docType)
        {
            StringBuilder sb = new StringBuilder(strXml);
            //sb.Append(strXml);

            if (docType == 1)
                sb.Insert(0, "<judgment name=\"doc_judgment\">").Append("</judgment>");
            else if (docType == 2)
                sb.Insert(0, "<act name=\"doc_act\">").Append("</act>");
            else if (docType == 3)
                sb.Insert(0, "<doc name=\"doc\">").Append("</doc>");

            //sb.Insert(0, "<akomaNtoso xmlns:eucases=\"http://eucases/proprietary\" xmlns=\"http://docs.oasis-open.org/legaldocml/ns/akn/3.0/CSD11\">").Append("</akomaNtoso>");

            //return sb.ToString();

            return "<akomaNtoso xmlns:eucases=\"http://eucases/proprietary\" xmlns=\"http://docs.oasis-open.org/legaldocml/ns/akn/3.0/CSD11\">" + sb.ToString() + "</akomaNtoso>";
        }

        // cool document text
        public static string GetDocText(int docId, int docType, int productId, bool showFreeDocuments)
        {
            return GetDocText(docId, docType, false, productId, showFreeDocuments);
        }

        public static int GetDocParIdByEid(string articleEid, int docLangId)
        {
            return DB.GetDocParIdByEid(articleEid, docLangId);
        }

        public static string GetModHint(string callerCelex, string docNumber, int langIdFromDoc, int siteLangId, int userId, string modType)
        {
            var isCallerCelexEuFinsMss = callerCelex?.StartsWith("eufinsdocument", StringComparison.OrdinalIgnoreCase) == true;
            if (isCallerCelexEuFinsMss)
            {
                callerCelex = Celex.ExtractFirst(callerCelex)?.Value ?? callerCelex;
            }


            String meta = GetDocMeta(docNumber, langIdFromDoc, siteLangId, userId);
            var modificationType = (ModificationType)Enum.Parse(typeof(ModificationType), modType);

            var config = new AkomaNtosoPreProcessorConfig
            {
                UILanguage = siteLangId,
                AkomaNtosoXmlId = langIdFromDoc
            };

            var result = AkomaNtosoPreProcessor.ConvertToHintHtml(meta, config, modificationType, callerCelex);

            return result;
        }

        public static int? GetLastConsDocLangId(string docNumber, int langIdFromDoc, int siteLangId, int userId, bool withText)
        {
            return DB.GetLastConsDocLangId(docNumber, langIdFromDoc, siteLangId, userId, withText);
        }

        public static string GetDocMeta(string docNumber, int langIdFromDoc, int siteLangId, int userId)
        {
            return DB.GetDocMeta(docNumber, langIdFromDoc, siteLangId, userId);
        }

        public static Document ParHint(int docLangId, int productId, int uiLangId, string appRootFolder)
        {
            Document d = Doc.GetDocument(docLangId);

            //string parXml = AddDocMetaTags(DB.GetDocText(docLangId, true), d.DocType);
            //string parXml = DB.GetDocText(docLangId, true);
            string parXml = DB.GetDocText(docLangId, true, productId);

            string pattern = "<a class=\"d-apis d-ref d-inline\" href=\"./(.*?)\"(.*?)>";
            d.Text = Regex.Replace(Doc.TransformFullDocumentForHint(parXml, d.LangId, docLangId, uiLangId), pattern, m => "<a class=\"d-apis d-ref d-inline\" href=\"" + appRootFolder + ParseLink(m.Groups[1].Value, d.LangId, false) + "\" " + m.Groups[2].Value + " target=\"_blank\">", RegexOptions.IgnoreCase);

            return d;
        }

        /// <summary>
        /// Prepair paragraph content for display
        /// </summary>
        /// <returns>Ready to output paragraph html</returns>
        public static Document GetParText(string linkType, string docNumber, string toPar, int? LangIdFromDoc, int? userId, int siteLangId, bool lastCons, bool lastConsWithText, bool replaceWithExternalLinks = false, bool addHtmlBodyTags = false)
        {
            string html = String.Empty;
            int? langId = null;

            Document d = new Document();

            // Цитат към практика в eur-lex
            if (docNumber[0] == '6' && (!String.IsNullOrEmpty(toPar) && toPar != "undefined"))
            {
                d = Doc.GetDocByDocNumber(docNumber, (LangIdFromDoc.HasValue ? LangIdFromDoc.Value : siteLangId), userId);
                if (d != null)
                {
                    langId = d.LangId;
                    d.ProductIds = Doc.GetDocProducts(d.DocLangId);
                    string docXml = Doc.GetDocText(d.DocLangId, d.DocType, true, d.ProductIds.FirstOrDefault(), true);
                    html = Doc.TransformFullDocumentForHint(docXml, langId.Value, d.DocLangId, siteLangId, toPar);
                }
                else
                {
                    d = new Document();
                }
            }


            // default 
            if (String.IsNullOrEmpty(html))
            {
                var row = DB.GetParText(linkType, docNumber, toPar, LangIdFromDoc, userId, siteLangId, lastCons, lastConsWithText);


                d.Title = row["title"].ToString();
                d.DocNumber = row["doc_number"].ToString(); // When has consolidated versions we pass the last one so doc_number changes to last consolidated version doc_number
                d.Country = row["country"].ToString().ToLower();

                //string title = par[3];
                if (!String.IsNullOrEmpty(row["doc_lang_id"].ToString()))
                {
                    d.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                    d.DocType = Convert.ToInt32(row["doc_type"]);
                    langId = Convert.ToInt32(row["lang_id"]);

                    // v.o. 2016.09.01 added langid set to the original document
                    if (langId != null)
                        d.LangId = langId.Value;

                    if (!String.IsNullOrEmpty(d.Title)) // long link
                    {
                        // title + text
                        html = "<p><b>"
                            + d.Title
                            + "</b></p>"
                            + Doc.TransformXml(
                                //"<akomaNtoso xmlns:eucases=\"http://eucases/proprietary\" xmlns=\"http://docs.oasis-open.org/legaldocml/ns/akn/3.0/CSD11\">"+ 
                                row["par_text"].ToString(),
                                //+ "</akomaNtoso>",
                                langId.Value,
                                d.DocLangId,
                                siteLangId);

                        if (addHtmlBodyTags)
                            html = "<html><body>" + html + "</body></html>";
                    }
                    else // short link. Get full document
                    {
                        //string parXml = AddDocMetaTags(row["par_text"].ToString(), d.DocType);
                        html = Doc.TransformFullDocumentForHint(row["par_text"].ToString(), langId.Value, d.DocLangId, siteLangId);
                    }
                }
                else
                {
                    // External link to eur-lex.eu
                    html = row["par_text"].ToString();
                }
            }

            // Fix inline links
            if (!langId.HasValue)
                langId = (LangIdFromDoc.HasValue) ? LangIdFromDoc.Value : siteLangId;

            if (!String.IsNullOrEmpty(html))
            {
                //string langShortCode = Languages.GetLang(langId.Value).ShortCode.ToUpper();
                string pattern = "<a class=\"d-apis d-ref d-inline\" href=\"./(.*?)\"(.*?)>";
                html = Regex.Replace(html, pattern, m => "<a class=\"d-apis d-ref d-inline\" href=\"" + ParseLink(m.Groups[1].Value, langId.Value, replaceWithExternalLinks) + "\" " + m.Groups[2].Value + " target=\"_blank\">", RegexOptions.IgnoreCase);
                //string pattern = "href=\"./CELEX=(.*?)&amp;(.*?)\"";
                //html = Regex.Replace(html, pattern, m => "href=\"/Doc/Act/" + langId.Value + "/" + m.Groups[1] + "\"", RegexOptions.IgnoreCase);
            }
            d.Text = html;

            return d;
        }

        public static string PrepairDocInfoLinks(string html, int langId)
        {
            string pattern = "<eucases:ref href=\"./([^\\<\\>]*?)\" class=\"(apis|apisweb)\">([\\W\\w]*?)</eucases:ref>";
            html = Regex.Replace(html, pattern, m => "<a class=\"apis doc-info-link\" href=\"" + m.Groups[1].Value + "\" target=\"_blank\">" + m.Groups[3].Value + "</a>", RegexOptions.IgnoreCase);
            return html;
        }

        private static string ParseLink(string href, int langId, bool replaceWithExternalLinks)
        {
            string url;
            string celex = "";
            string toPar = "";
            string[] arr = href.Split(new string[] { "&amp;" }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                string[] param = arr[0].Split('=');
                if (param.Length == 2)
                    celex = param[1]; // CELEX 
                if (arr.Length == 2) // ToPar argument exists
                {
                    param = arr[1].Split('=');
                    if (param.Length == 2)
                        toPar = param[1]; // ToPar 
                }
            }
            if (replaceWithExternalLinks == true)
            {
                url = "http://eur-lex.europa.eu/legal-content/" + Languages.GetLang(langId).ShortCode.ToUpper() + "/ALL/?uri=CELEX:" + celex;
            }
            else
            {
                url = "/Doc/Act/" + langId.ToString() + "/" + celex + (!String.IsNullOrEmpty(toPar) ? "/" + toPar : "");
            }

            return url;
        }

        public static Dictionary<string, string> GetEIDfromToPar(int docLangId, string toPar)
        {
            Dictionary<string, string> result = null;
            var row = DB.GetEIDfromToPar(docLangId, toPar);
            if (row != null)
            {
                result = new Dictionary<string, string>();
                result.Add("eid", row["eid"].ToString());
                result.Add("num", row["num"].ToString());
            }
            return result;
        }

        public static string CheckEID(int docLangId, string eid, string num)
        {
            return DB.CheckEID(docLangId, eid, num);
        }

        public static string GetDocNumberByDocLangId(int langId)
        {
            return DB.GetDocNumberByDocLangId(langId);
        }

        public static List<DocLink> GetDocInLinks(string docNumber, string toPar, string filterDomain, int? userId, int siteLangId, int limit, out int totalCount)
        {
            List<DocLink> l = new List<DocLink>();
            totalCount = -1;
            foreach (var r in DB.GetDocInLinks(docNumber, toPar, filterDomain, userId, siteLangId, limit))
            {
                if (totalCount == -1)
                    totalCount = Convert.ToInt32(r["total_count"]); // one time
                l.Add(new DocLink()
                {
                    DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                    DocType = Convert.ToInt32(r["doc_type"]),
                    Title = r["title"].ToString(),
                    OriginalLink = r["original_link"].ToString()
                });
            }

            if (totalCount == -1)
                totalCount = 0;

            return l;
        }

        public static List<DocLink> GetDocInLinks(int toDocLangId, int? toParId, string filterDomain, int? userId, int siteLangId, int limit, out int totalCount)
        {
            List<DocLink> l = new List<DocLink>();
            totalCount = -1;
            foreach (var r in DB.GetDocInLinks(toDocLangId, toParId, filterDomain, userId, siteLangId, limit))
            {
                if (totalCount == -1)
                    totalCount = Convert.ToInt32(r["total_count"]); // one time
                l.Add(new DocLink()
                {
                    DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                    DocType = Convert.ToInt32(r["doc_type"]),
                    Title = r["title"].ToString(),
                    OriginalLink = r["original_link"].ToString(),
                    Publisher = r["publisher"].ToString()
                });
            }

            if (totalCount == -1)
                totalCount = 0;

            return l;
        }

        public static int[] GetDocInLinks(string docNumber, string toPar, string domain, int userId, int siteLangId, int limit)
        {
            List<int> l = new List<int>();

            foreach (var r in DB.GetDocInLinks(docNumber, toPar, domain, userId, siteLangId, limit))
            {
                l.Add(Convert.ToInt32(r[0]));
            }

            return l.ToArray();
        }

        public static int[] GetDocInLinks(int toDocLangId, int? toParId, int productId, int[] linkIds, string subTitle, bool showFreeDocuments)
        {
            List<int> l = new List<int>();
            foreach (var r in DB.GetDocInLinks(toDocLangId, toParId, productId, linkIds, subTitle, showFreeDocuments))
            {
                l.Add(Convert.ToInt32(r[0]));
            }

            return l.ToArray();
        }

        public static List<DocLink> GetDocListByIds(int[] docLangIds, string filterDomain)
        {
            List<DocLink> l = new List<DocLink>();
            foreach (var r in DB.GetDocListByIds(docLangIds, filterDomain))
            {
                l.Add(new DocLink()
                {
                    DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                    DocType = Convert.ToInt32(r["doc_type"]),
                    Title = r["title"].ToString(),
                    OriginalLink = r["original_link"].ToString(),
                    Publisher = r["publisher"].ToString()
                });
            }
            return l;
        }

        public static string[] GetDocLinksTitle(int docLangId, int? toParId)
        {
            string[] result = new string[2];
            var row = DB.GetDocLinksTitle(docLangId, toParId);
            if (row != null)
            {
                result[0] = row["doc_number"].ToString();
                result[1] = row["par_title"].ToString();
            }
            return result;
        }

        public static string[] GetDocLinksTitle(string docNumber, string toPar, int siteLangId, int userId)
        {
            string[] result = new string[2];
            var row = DB.GetDocLinksTitle(docNumber, toPar, siteLangId, userId);
            if (row != null)
            {
                result[0] = row["doc_number"].ToString();
                result[1] = row["par_title"].ToString();
            }

            return result;
        }

        public static string CreateEurLexLink(string celex, int langId)
        {
            Language lang = Languages.GetLang(langId);
            string shortCode = lang.ShortCode.ToUpper();
            return CreateEurLexLink(celex, shortCode);
        }

        public static string CreateEurLexLink(string celex, string langShortCode)
        {
            return "http://eur-lex.europa.eu/legal-content/" + langShortCode + "/ALL/?uri=CELEX:" + celex;
        }

        private static List<DocContentItem> GetDocContentChildren(int parentDocParId, List<DocContentItem> l)
        {
            List<DocContentItem> children = new List<DocContentItem>();

            foreach (DocContentItem item in l.Where(i => i.parent_id == parentDocParId))
            {
                item.children = GetDocContentChildren(item.id, l);
                children.Add(item);
            }

            return children;
        }

        public static List<DocContentItem> GetDocContents(int docLangId)
        {
            var contentItems = DB.GetDocContents(docLangId).Select(Extensions.ToDocContentItem).ToList();
            var parentToChildrenMap = contentItems
                .Where(x => x.parent_id != null)
                .GroupBy(x => x.parent_id)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var contentItem in contentItems)
            {
                if (parentToChildrenMap.ContainsKey(contentItem.id))
                {
                    contentItem.children = parentToChildrenMap[contentItem.id];
                }
            }

            var rootContentItems = contentItems.Where(x => x.parent_id == null).ToList();

            return rootContentItems;
        }

        [Obsolete("Use GetDocContents")]
        public static List<DocContentItem> _GetDocContents(int docLangId)
        {
            List<DocContentItem> l = new List<DocContentItem>();
            foreach (var r in DB.GetDocContents(docLangId))
            {
                l.Add(new DocContentItem()
                {
                    id = Convert.ToInt32(r["doc_par_id"]),
                    parent_id = r["parent_doc_par_id"] as int?,
                    eid = r["eid"].ToString(),
                    title = r["num"].ToString() + " " + r["heading"].ToString() ?? "",
                    tooltip = r["num"].ToString() + " " + r["heading"].ToString() ?? ""
                });
            }

            List<DocContentItem> docContents = new List<DocContentItem>();
            foreach (DocContentItem item in l.Where(i => i.parent_id == null))
            {
                item.children = GetDocContentChildren(item.id, l);
                docContents.Add(item);

            }

            return docContents;
        }

        public static IDictionary<int, string> GetDocLangs(int docLangId)
        {
            var docLangs = new Dictionary<int, string>();
            var langsFromDB = DB.GetDocLangs(docLangId);

            foreach (var lang in langsFromDB)
            {
                docLangs.Add(int.Parse(lang["doc_lang_id"].ToString()), lang["short_lang"].ToString());
            }

            return docLangs;
        }

        public static int GetDocLanguageByDocLangId(int docLangId)
        {
            return DB.GetDocLanguageByDocLangId(docLangId);
        }

        public static Dictionary<string, string> GetLinkInfo(string docNumber, string toPar, int? userId, int langId)
        {
            Dictionary<string, string> linkInfo = null;
            var row = DB.GetLinkInfo(docNumber, toPar, userId, langId);
            if (row != null)
            {
                linkInfo = new Dictionary<string, string>();
                linkInfo.Add("doc_lang_id", row["doc_lang_id"].ToString());
                linkInfo.Add("doc_type", row["doc_type"].ToString());
                linkInfo.Add("title", row["title"].ToString());
                linkInfo.Add("doc_par_id", row["doc_par_id"].ToString());
                linkInfo.Add("par_title", row["par_title"].ToString());
            }
            return linkInfo;
        }

        public static List<DocVersion> GetDocVersions(int docLangId, int langId)
        {
            List<DocVersion> l = new List<DocVersion>();
            foreach (var r in DB.GetDocVersions(docLangId, langId))
            {
                l.Add(new DocVersion()
                {
                    DocNumber = r["doc_number"].ToString(),
                    DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                    PublDate = r["doc_date"] as DateTime?
                });
            }

            return l;
        }

        public static List<DocVersion> GetJudgmentRelatedDocs(int docLangId, int langId)
        {
            List<DocVersion> l = new List<DocVersion>();
            foreach (var r in DB.GetJudgmentRelatedDocs(docLangId, langId))
            {
                l.Add(new DocVersion()
                {
                    DocNumber = r["doc_number"].ToString(),
                    DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                    PublDate = r["doc_date"] as DateTime?
                });
            }

            return l;
        }

        private static readonly HashSet<string> parts = new HashSet<string> { "part", "tit", "chap", "sec", "art", "pt", "par", "al", "let", "ind", "sent", "rec", "ann" };
        private static readonly HashSet<string> headingParts = new HashSet<string>() { "part", "tit", "chap", "sec", "ann" };

        private static List<Tuple<string, string>> SplitHrefParts(string toPar)
        {
            string[] arr = toPar.Split('_');
            List<Tuple<string, string>> res = new List<Tuple<string, string>>();
            string pattern = "(" + String.Join("|", parts) + ")";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (string item in arr)
            {
                Match m = reg.Match(item);
                if (m.Success)
                {
                    res.Add(new Tuple<string, string>(m.Value, item.Substring(m.Index + m.Length)));
                }
                else
                {
                    res.Add(new Tuple<string, string>(string.Empty, item));
                }
            }

            return res;
        }

        private static readonly Dictionary<string, string> partValuesDict = new Dictionary<string, string>()
        {
            //roman valuse - ??? may be funcion here
            { "I", "1"}, { "II", "2"}, { "III", "3"}, { "IV", "4"}, { "V", "5"}, { "VI", "6"}, { "VII", "7"},
            { "VIII", "8"}, { "IX", "9"}, { "X", "10"}, { "XI", "11"}, { "XII", "12"}, { "XIII", "13"},
            
            //leters always in english
            { "А", "A"}, { "Б", "B"}, { "В", "C"}, { "Г", "D"}, { "Д", "E"}, { "Е", "F"}, { "Ж", "G"}, { "З", "H"}, { "И", "I"},

            //others
            { "ERSTE", "1"}, { "ZWEITER", "2"}, { "LETZTE", "LAST"},
            { "ВТОРО", "2"}, { "ПЪРВО", "1"}, { "ПОСЛЕДНО", "LAST"},
            { "ПЪРВИ", "1"}, { "ВТОРИ", "2"}, { "ТРЕТИ", "LAST"}
        };
        private static string FixPartValue(string partValue)
        {
            partValue = partValue.ToUpper().Trim();
            if (partValuesDict.ContainsKey(partValue))
            {
                partValue = partValuesDict[partValue];
            }
            return partValue;
        }
        private static string NormalizePalCaption(string toParCaption, IDictionary<string, string> titles, HashSet<string> normDocStructure)
        {
            string res = toParCaption.ToLower();

            List<Tuple<string, string>> hrefParts = SplitHrefParts(res);
            if (hrefParts.Count > 0)
            {
                Tuple<string, string> firstPart = hrefParts[0];
                string pattern = "(" + String.Join("|", parts) + ")";
                Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

                string fixedPartValue = FixPartValue(firstPart.Item2);

                if (firstPart.Item1 == "rec")
                    return string.Empty;

                if (normDocStructure.Count > 0 && firstPart.Item1 == "art")
                {
                    if (!normDocStructure.Contains(fixedPartValue))
                        return string.Empty;
                }

                res = reg.Replace(firstPart.Item1, new MatchEvaluator(delegate (Match match) { return ToParLinkCaption(match, titles); })) + fixedPartValue;
                if (headingParts.Contains(firstPart.Item1))
                {
                    return string.Empty;
                }

                for (int i = 1; i < Math.Min(hrefParts.Count, 3); i++)
                {
                    if (parts.Contains(hrefParts[i].Item1))
                    {
                        res += "." + FixPartValue(hrefParts[i].Item2);
                    }
                    else
                    {
                        break;
                    }
                }

                if (normDocStructure.Count > 0 && res.IndexOf(".") > -1)
                {
                    string artTmp = res;
                    foreach (string title in titles.Values)
                    {
                        artTmp = artTmp.Replace(title, "").Trim();
                    }

                    if (!normDocStructure.Contains(artTmp))
                    {
                        res = res.Substring(0, res.IndexOf("."));
                    }

                }

            }
            return res;
        }

        private static string NormalizeSingleArticle(string artCaption)
        {
            artCaption = artCaption.ToUpper();
            foreach (var key in partValuesDict.Keys)
            {
                artCaption = artCaption.Replace(key, partValuesDict[key]);
            }

            return artCaption;
        }

        private static HashSet<string> CorrectDocStructure(List<string> docStructure)
        {

            HashSet<string> res = new HashSet<string>();
            foreach (var item in docStructure)
            {
                string s = NormalizeSingleArticle(item);

                if (!res.Contains(s))
                    res.Add(s);
            }
            return res;
        }

        public static List<PalProvision> GetActProvisionsLinkedByCases(string docNumber, IDictionary<string, string> titles, string numSimple)
        {
            int docLangId = DB.GetActProvisionsCorrectDocLangId(docNumber, numSimple);

            if (docLangId == 0)
            {
                return new List<PalProvision>();
            }

            return GetActProvisionsLinkedByCases(docLangId, titles, numSimple);
        }

        public static List<PalProvision> GetActProvisionsLinkedByCases(int docLangId, IDictionary<string, string> titles, string numSimple)
        {
            //List<string> rawDocStructure = new List<string>()
            //{
            //    "1", "1.1", "1.2", "1.2.A", "1.2.B", "1.2.C", "1.2.D", "1.3", "2", "2.1", "2.2", "3", "3.1", "3.2", "4", "4.1", "4.2", "5", "5.1A", "5.1A.B",
            //    "5.1A.C", "5.2", "5.3", "5.4", "5.5", "5.6", "5.7", "5.7.A", "5.7.B", "6", "6.1", "6.2", "6.3", "6.4", "7", "8", "9", "9.1", "9.1.A", "9.1.B",
            //    "9.1.C", "9.2", "10", "11", "11.1", "11.2", "11.3", "12", "12.1", "12.2", "13", "13.1", "13.2", "13.3", "13.4", "13.5", "14", "14.1", "14.1.A",
            //    "14.1.B", "14.2", "14.2.A", "14.2.B", "14.3", "14.4", "14.5", "15", "15.1", "15.1.A", "15.1.B", "15.1.C", "15.2", "15.3", "16", "16.1", "16.2",
            //    "16.3", "17", "17.1", "17.2", "17.3", "18", "18.1", "18.2", "19", "19.1", "19.2", "19.2.A", "19.2.B", "20", "20.1", "20.2", "21", "21.1",
            //    "21.2", "22", "22.1", "22.2", "22.3", "22.4", "22.5", "23", "23.1", "23.1.A", "23.1.B", "23.1.C", "23.2", "23.3", "23.4", "23.5", "24", "25",
            //    "26", "26.1", "26.2", "26.3", "26.4", "27", "27.1", "27.2", "28", "28.1", "28.2", "28.3", "29", "30", "30.1", "30.2", "31", "32", "33", "33.1",
            //    "33.2", "33.3", "34", "34.1", "34.2", "34.3", "34.4", "35", "35.1", "35.2", "35.3", "36", "37", "37.1", "37.2", "38", "38.1", "38.2", "39", "39.1",
            //    "39.2", "40", "40.1", "40.2", "40.3", "41", "42", "42.1", "42.2", "43", "43.1", "43.2", "43.3", "43.4", "43.5", "44", "45", "45.1", "45.2", "46",
            //    "46.1", "46.2", "46.3", "47", "47.1", "47.2", "47.3", "48", "48.1", "48.2", "49", "50", "51", "52", "53", "53.1", "53.2", "54", "55", "55.1", "55.2",
            //    "56", "57", "57.1", "57.2", "57.3", "57.4", "58", "59", "59.1", "59.2", "60", "60.1", "60.1.A", "60.1.B", "60.1.C", "60.2", "60.3", "61", "62", "63",
            //    "63.1", "63.2", "63.3", "63.4", "64", "64.1", "64.2", "65", "65.1", "65.1.A", "65.1.B", "65.1.C", "65.2", "66", "66.1", "66.2", "66.2.A", "66.2.B",
            //    "67", "68", "68.1", "68.2", "69", "70", "70.1", "70.2", "71", "71.1", "71.2", "71.2.A", "71.2.B", "72", "73", "74", "74.1", "74.2", "75", "75.1", "75.2", "76",
            //};

            List<string> rawDocStructure = Doc.GetEurLexArticles(docLangId);

            HashSet<string> normDocStructure = CorrectDocStructure(rawDocStructure);
            //normDocStructure.Clear();

            IEnumerable<IDataRecord> records = null;

            if (String.IsNullOrEmpty(numSimple))
            {
                records = DB.GetActProvisionsLinkedByCases(docLangId);
            }
            else
            {
                records = DB.GetActProvisionsLinkedByCases(docLangId, numSimple);
            }

            var l = Doc.NormalizeProvisions(records, normDocStructure, titles, false);

            return l;
        }


        public static List<PalProvision> GetDocArticleProvisions(int docLangId, string eid, IDictionary<string, string> titles, int productId, bool showFreeDocuments)
        {
            List<string> rawDocStructure = Doc.GetEurLexArticles(docLangId);

            HashSet<string> normDocStructure = CorrectDocStructure(rawDocStructure);
            //normDocStructure.Clear();

            var records = DB.GetDocArticleProvisions(docLangId, eid, productId, showFreeDocuments);

            var l = Doc.NormalizeProvisions(records, normDocStructure, titles, true);

            var langId = Doc.GetDocLanguageByDocLangId(docLangId);
            l = Doc.NormalizeProvisionsByLang(l, langId);

            l = l.OrderBy(x => x.SortTitle).ToList();

            return l;
        }

        private static List<PalProvision> NormalizeProvisions(IEnumerable<IDataRecord> records, HashSet<string> normDocStructure, IDictionary<string, string> titles, bool isArticleProvisions)
        {
            List<PalProvision> l = new List<PalProvision>();

            foreach (var r in records)
            {
                string toPar = r["to_par"].ToString();

                List<int> linkIds = new List<int>();
                string title = NormalizePalCaption(toPar, titles, normDocStructure);
                // check if title is normalized
                // One interval in title is expected and used in razor view!
                if (string.IsNullOrEmpty(title) || title.IndexOf(" ") < 0)
                {
                    continue;
                }
                string[] arr = toPar.Split('_');
                if (arr.Length > 1)
                {
                    linkIds.Add(Convert.ToInt32(r["link_id"]));
                }

                var curPalProvision = new PalProvision()
                {
                    ToDocParId = Convert.ToInt32(r["to_doc_par_id"]),
                    LinkIds = linkIds,
                    Title = title,
                    DBOrder = Convert.ToInt32(r["ord"]),
                    ToParOriginal = toPar
                };


                l.Add(curPalProvision);
            }

            if (isArticleProvisions)
            {
                var realProvisionFound = 0;
                if (l.Count(e => e.ToDocParId != -1) > 0)
                {
                    realProvisionFound = l.Where(e => e.ToDocParId != -1).FirstOrDefault().ToDocParId;
                }

                if (realProvisionFound != 0)
                {
                    for (int i = 0; i < l.Count; i++)
                    {
                        if (l[i].ToDocParId == -1)
                        {
                            l[i].ToDocParId = realProvisionFound;
                        }
                    }
                }

            }

            Dictionary<string, PalProvision> distinctProvisions = new Dictionary<string, PalProvision>();
            foreach (PalProvision pp in l)
            {
                if (!distinctProvisions.ContainsKey(pp.Title))
                {
                    distinctProvisions.Add(pp.Title, pp);
                }
                else
                {
                    distinctProvisions[pp.Title].LinkIds.AddRange(pp.LinkIds);
                }

            }
            l = distinctProvisions.Select(x => x.Value).ToList();//.OrderBy(x => x.Title).ToList();

            // Търсене на практика към цял параграф (doc_par_id). Не са нужни LinkIds
            foreach (PalProvision pl in l)
            {
                if (pl.Title.IndexOf(".") == -1)
                    pl.LinkIds = new List<int>();
            }

            l.ForEach(x =>
            {
                string[] parts = x.Title.Split('.').Skip(1).ToArray();
                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];
                    int result;
                    if (int.TryParse(part, out result))
                        parts[i] = part.PadLeft(6, '0') + " "; // интервал вместо буква
                    else // Има буква
                    {
                        if (part.Length == 1)
                            parts[i] = part.PadLeft(7, '9');
                        else // by ex: 1a, 1)
                            parts[i] = part.PadLeft(7, '0');
                    }
                }
                x.SortTitle = "dbord-" + x.DBOrder.ToString().PadLeft(6, '0') + "." + String.Join(".", parts);
            });

            l = l.OrderBy(x => x.SortTitle).ToList();

            return l;
        }

        private static List<PalProvision> NormalizeProvisionsByLang(List<PalProvision> inputRecords, int desiredLangId)
        {
            var res = new List<PalProvision>();

            for (int i = 0; i < inputRecords.Count; i++)
            {
                var item = inputRecords[i];
                var originalTitleSplit = item.Title.Split(' ');
                item.Title = originalTitleSplit[0] + " " + TransformParTitleBetweenLanguages(originalTitleSplit[1], desiredLangId);
                item.SortTitle = item.Title; // Check if there is a case when they are not identical

                var previouslyAddedRecord = res.Where(p => p.Title == item.Title).FirstOrDefault();

                if (previouslyAddedRecord != null)
                {
                    previouslyAddedRecord.LinkIds.AddRange(item.LinkIds);
                }
                else
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private static string TransformParTitleBetweenLanguages(string input, int langId)
        {
            if (langId == 1)
            {
                return AlphabetsDict.Aggregate(input, (current, value) =>
                current.Replace(value.Key, value.Value));
            }
            else // consider additional checks and cases
            {
                return AlphabetsDict.Aggregate(input, (current, value) =>
                current.Replace(value.Value, value.Key));
            }
            /*else
            {
                return input; // can not transform at the moment
            }*/
        }

        public static string ToParLinkCaption(Match match, IDictionary<string, string> titles)
        {
            string title_key = "UI_ParTitle_" + match.Value;
            if (titles.ContainsKey(title_key))
                return titles[title_key] + " ";
            return match.Value;
        }

        public static List<Document> GetPALDocList(int[] docLangId)
        {
            List<Document> l = new List<Document>();
            try
            {
                foreach (var r in DB.GetPALDocList(docLangId))
                {
                    l.Add(new Document()
                    {
                        DocNumber = r["doc_number"].ToString(),
                        DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                        Title = r["title"].ToString()
                    });
                }
            }
            catch
            {
                //TODO. Timeout possible
            }

            return l;
        }

        public static List<Document> GetReferedActECHRDocList(int[] docLangId)
        {
            var l = new List<Document>();

            foreach (var r in DB.GetReferedActECHRDocList(docLangId))
            {
                l.Add(new Document()
                {
                    DocNumber = r["doc_number"].ToString(),
                    DocLangId = Convert.ToInt32(r["doc_lang_id"]),
                    Title = r["title"].ToString()
                });
            }

            return l;
        }

        public static int? GetDocLangIdByIdentifier(string docIdentifier)
        {
            int? docLangId = null;
            string result = DB.GetDocLangIdByIdentifier(docIdentifier);
            if (!String.IsNullOrEmpty(result))
                docLangId = Convert.ToInt32(result);

            return docLangId;
        }

        public static int[] GetDocProducts(int docLangId)
        {
            return DB.GetDocProducts(docLangId);
        }

        public static List<AkomaNtosoXml.Xslt.Core.Classes.Helpers.XsltConsolidatedInfo> GetDocConsVersions(int docLangId)
        {
            List<AkomaNtosoXml.Xslt.Core.Classes.Helpers.XsltConsolidatedInfo> l = new List<AkomaNtosoXml.Xslt.Core.Classes.Helpers.XsltConsolidatedInfo>();
            foreach (var r in DB.GetDocConsVersions(docLangId))
            {
                l.Add(new AkomaNtosoXml.Xslt.Core.Classes.Helpers.XsltConsolidatedInfo()
                {
                    Celex = r["celex"].ToString(),
                    Date = (r["date"] != DBNull.Value) ? (DateTime?)r["date"] : null,
                    DocId = (r["doc_lang_id"] != DBNull.Value) ? Convert.ToInt32(r["doc_lang_id"]) : (int?)null,
                    IsLastVersion = false
                });
            }

            if (l.Count > 0)
                l.Last().IsLastVersion = true;

            return l;
        }

        public static List<ConsVersion> GetDocConsVersionsList(int docLangId)
        {
            List<ConsVersion> l = new List<ConsVersion>();
            foreach (var r in DB.GetDocConsVersions(docLangId))
            {
                l.Add(new ConsVersion()
                {
                    Celex = r["celex"].ToString(),
                    Date = (r["date"] != DBNull.Value) ? (DateTime?)r["date"] : null,
                    DocLangId = (r["doc_lang_id"] != DBNull.Value) ? Convert.ToInt32(r["doc_lang_id"]) : (int?)null,
                    LeadDocLangId = (r["lead_doc_lang_id"] != DBNull.Value) ? Convert.ToInt32(r["lead_doc_lang_id"]) : (int?)null
                });
            }

            if (l.Count > 0)
                l.Last().IsLast = true;

            return l;
        }

        public static List<string> GetEurLexArticles(int docLangId)
        {
            return DB.GetEurLexArticles(docLangId).Select(r => r["article"].ToString()).ToList();
        }

        /// <summary>
        /// Checks if a documents is avaiable for sysdemo
        /// </summary>
        /// <param name="docLangId">DocLangId of the document</param>
        /// <returns></returns>
        public static bool IsDemoDoc(int docLangId)
        {
            return DB.IsDemoDoc(docLangId);
        }

        public static bool IsInternationalStandartDoc(int docLangId)
        {
            const string INTERNATIONAL_STANDARTS_CLASSIFIER_IDENTIFIER = "F87F25B58DE34F68B2FE7670AEA7CC3C";

            return DB.CheckDocumentIsClassifiedBy(docLangId, INTERNATIONAL_STANDARTS_CLASSIFIER_IDENTIFIER);
        }

        /// <summary>
        /// Flags documents as avaiable for sysdemo
        /// </summary>
        /// <param name="docLangIds">DocLangIds of the documents</param>
        /// <returns>Documents updated count</returns>
        public static int AddDemoDocs(int[] docLangIds)
        {
            return DB.AddDemoDocs(docLangIds);
        }

        //public static string GetParText(string docNumber, string toPar, int userId, int siteLangId)
        //{
        //    return DB.GetParText(docNumber, toPar, userId, siteLangId);
        //}

        public static string GetDocParByParId(int docLangId, int docParId, int siteLangId, int langIdFromDoc, string searchText, string appRootFolder, bool parseLinks = false) // parseLinks for hints
        {
            string parXml = DB.GetDocParByParId(docParId, docLangId);
            string html = Doc.TransformXml("<akomaNtoso xmlns:eucases=\"http://eucases/proprietary\" xmlns=\"http://docs.oasis-open.org/legaldocml/ns/akn/3.0/CSD11\">"
                    + parXml
                    + "</akomaNtoso>",
                    langIdFromDoc,
                    docLangId,
                    siteLangId);

            if (!String.IsNullOrEmpty(searchText))
            {
                var htmlHeighlighter = new HtmlHighlighter();
                html = htmlHeighlighter.HighlightHtmlEuroCases(html, searchText, langIdFromDoc);
            }

            if (parseLinks)
            {
                string pattern = "<a class=\"d-apis d-ref d-inline\" href=\"./(.*?)\"(.*?)>";
                html = Regex.Replace(html, pattern, m => "<a class=\"d-apis d-ref d-inline\" href=\"" + appRootFolder + ParseLink(m.Groups[1].Value, langIdFromDoc, false) + "\" " + m.Groups[2].Value + " target=\"_blank\">", RegexOptions.IgnoreCase);
            }

            return html;
        }

        public static string GetDocumentNumberByDocLangId(int docLangId)
        {
            return DB.GetDocumentNumberByDocLangId(docLangId);
        }

        public static string GetDocArtTextByDocParId(int docLangId, int docParId, int siteLangId, int langIdFromDoc, string searchText, bool parseLinks = false)
        {
            string parXml = DB.GetDocArtTextByDocParId(docParId);
            string html = Doc.TransformXml("<akomaNtoso xmlns:eucases=\"http://eucases/proprietary\" xmlns=\"http://docs.oasis-open.org/legaldocml/ns/akn/3.0/CSD11\">"
                    + parXml
                    + "</akomaNtoso>",
                    langIdFromDoc,
                    docLangId,
                    siteLangId);

            if (!String.IsNullOrEmpty(searchText))
            {
                var htmlHeighlighter = new HtmlHighlighter();
                html = htmlHeighlighter.HighlightHtmlEuroCases(html, searchText, langIdFromDoc);
            }

            if (parseLinks)
            {
                string pattern = "<a class=\"d-apis d-ref d-inline\" href=\"./(.*?)\"(.*?)>";
                html = Regex.Replace(html, pattern, m => "<a class=\"d-apis d-ref d-inline\" href=\"" + ParseLink(m.Groups[1].Value, langIdFromDoc, false) + "\" " + m.Groups[2].Value + " target=\"_blank\">", RegexOptions.IgnoreCase);
            }

            return html;
        }

        public static IEnumerable<RefDocsPair> GetOldArticleEuRefDocs(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            var dbRefDocsPairs = DB.GetOldArticleEuRefDocs(userId, docLangId, siteLangId, docParIds, langId);
            return FillRefDocsPairs(dbRefDocsPairs, siteLangId);
        }

        public static IEnumerable<RefDocsPair> GetOldArticleFinsRefDocs(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            var dbRefDocsPairs = DB.GetOldArticleFinsRefDocs(userId, docLangId, siteLangId, docParIds, langId);
            return FillRefDocsPairs(dbRefDocsPairs, siteLangId);
        }

        public static IEnumerable<RefDocsPair> GetNewArticleRefDocs(int userId, int docLangId, int siteLangId, int[] docParIds, int langId)
        {
            var dbRefDocsPairs = DB.GetNewArticleRefDocs(userId, docLangId, siteLangId, docParIds, langId);
            return FillRefDocsPairs(dbRefDocsPairs, siteLangId);
        }

        private static IEnumerable<RefDocsPair> FillRefDocsPairs(IEnumerable<IDataRecord> dbRefDocsPairs, int siteLangId)
        {
            var refDocsPairs = new List<RefDocsPair>();
            foreach (var r in dbRefDocsPairs)
            {
                int mainDocLangId = Convert.ToInt32(r["main_doc_lang_id"].ToString());
                int refDocLangId = Convert.ToInt32(r["doc_lang_id"].ToString());
                int productOrder = Convert.ToInt32(r["product_order"].ToString());
                int articleOrder = Convert.ToInt32(r["article_order"].ToString());
                var document = GetDocument(refDocLangId);
                document.Publisher = r["publisher"].ToString();

                var mainDocument = GetDocument(mainDocLangId);                

                document.Title = r["full_title"].ToString();
                Type keywordsT = r["doc_keywords"].GetType();
                if (keywordsT.IsArray && keywordsT.IsAssignableFrom(typeof(string[,])))
                    document.SetKeywords((string[,])r["doc_keywords"], siteLangId);

                Type summariesT = r["doc_summaries"].GetType();
                if (summariesT.IsArray && summariesT.IsAssignableFrom(typeof(string[,])))
                    document.SetSummaries((string[,])r["doc_summaries"], siteLangId);

                document.DocNumber = r["doc_number"].ToString();

                refDocsPairs.Add(new RefDocsPair(mainDocument, document, r["article_info"].ToString(), productOrder, articleOrder));
            }

            return refDocsPairs;
        }

        public static string GetPathToArticle(int docParId)
        {
            return DB.GetPathToArticle(docParId);
        }

        public static IEnumerable<ArticleCorrelation> GetArticlesCorrelation(int userId, int siteLangId, int docParId, int langId)
        {
            var articlesCorrelation = new List<ArticleCorrelation>();
            var dbArticlesCorellation = DB.GetArticlesCorrelation(userId, siteLangId, docParId, langId);
            foreach (var r in dbArticlesCorellation)
            {
                string docNumber = r["doc_number"].ToString();
                int docLangId = Convert.ToInt32(r["doc_lang_id"].ToString());
                string articleData = r["article_data"].ToString();

                articlesCorrelation.Add(new ArticleCorrelation(docNumber, docLangId, articleData));
            }

            return articlesCorrelation;
        }

        public static String GetUnifiedDocInfoSource(String originalSource, String publisher, AdditionalInfoEnum additionalInfo, IDictionary<String, String> rubrics)
        {
            // by request from Ico K.
            //  the change is not made in the database because the table depends on the update
            //  and if we change the value from 'Document' to 'Original source' in the mssql database we will need to update all documents in the database
            //  which will take months to perform

            var unified = originalSource;
            if (unified == "Document")
            {
                if (publisher == "Jure")
                {
                    unified = "Jure";
                }
                else if (publisher == "Dec.Nat")
                {
                    unified = "DecNat";
                }
                else if (publisher == "Interlex editor tool database" && additionalInfo != AdditionalInfoEnum.ExpertMaterials)
                {
                    unified = "Interlex";
                }
                else if (publisher == "JuriFast")
                {
                    unified = "JuriFast";
                }
                else
                {
                    unified = rubrics["XsltOriginalSource"];
                }
            }

            return unified;
        }

        public static String GetHtmlFriendlySource(String source)
        {
            return source?.Replace(" ", "-");
        }

        public static String GetHtmlFriendlyEscapedSource(String source)
        {
            source = GetHtmlFriendlySource(source)?.Replace("&", "_AND_");

            return source;
        }
    }
}
