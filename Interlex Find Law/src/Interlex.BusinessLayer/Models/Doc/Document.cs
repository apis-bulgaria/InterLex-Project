namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using AkomaNtosoXml.Xslt.Core.Classes.Model;
    using AkomaNtosoXml.Xslt.Core.Classes.Providers;
    using AkomaNtosoXml.Xslt.Core.Classes.Resolver;
    using Apis.Common.Celex;
    using ApisLucene.Classes.Eucases.Highlight.Config.BG;
    using ApisLucene.Classes.Eucases.Highlight.Config.DE;
    using ApisLucene.Classes.Eucases.Highlight.Config.EN;
    using ApisLucene.Classes.Eucases.Highlight.Config.FR;
    using ApisLucene.Classes.Eucases.Highlight.Parser;
    using ApisLucene.Classes.Eucases.Highlight.Tokenizer;
    using ApisLucene.Classes.Eucases.Stemming.BG;
    using ApisLucene.Classes.Eucases.Stemming.DE;
    using ApisLucene.Classes.Eucases.Stemming.EN;
    using ApisLucene.Classes.Eucases.Stemming.FR;

    public class Document
    {
        private String country;

        #region Class properties

        public int BaseDocLangId { get; set; }

        public int DocLangId { get; set; }

        public string Country
        {
            get => this.country;
            set => this.country = value == "gb" ? "uk" : value;
        }

        public string CountryName { get; set; }

        public int DocType { get; set; }

        public string DocNumber { get; set; }

        public string Title { get; set; }

        public bool HasInLinks { get; set; } = false;

        public string ArticlePathJSON { get; set; }

        public string Text
        {
            get;
            set;
        }

        public string Publisher
        {
            get;
            set;
        }

        public int LangId { get; set; }

        public Dictionary<string, string> Keywords { get; set; }
        public Dictionary<string, string> Summaries { get; set; }

        public string UserDocId { get; set; }

        public IDictionary<int, string> DocLangs { get; set; }

        public DateTime DocDate { get; set; }
        public DateTime? DateOfEffect { get; set; }
        public DateTime? EndDate { get; set; }

        public string LastConsDocNumber { get; set; }

        //used for highlight chain with client-side search in document
        public string SearchedText { get; set; }

        public bool ExactMatch { get; set; }
        public string MultilingualSearchedText { get; set; }

        public int[] ProductIds { get; set; }

        public List<RelatedDoc> RelatedDocNumbers { get; set; }

        private FragmentsResult htmlModel = null;
        public FragmentsResult HtmlModel
        {
            get
            {
                return this.htmlModel;
            }
        }

        public BlobInfo BloblInfo { get; set; }

        public AdditionalInfoEnum AdditionalInfo { get; set; } = AdditionalInfoEnum.None;

        #endregion

        public Document()
        {
            this.Keywords = new Dictionary<string, string>();
            this.Summaries = new Dictionary<string, string>();

            this.RelatedDocNumbers = new List<RelatedDoc>();
        }

        public Document(string strXML, int langIdofDoc, int docLangId, int uiLangId, DocHighlightSearchParams highlightParams)
          : this()
        {
            //this.Keywords = new string[200];
            this.GetDocumentContent(strXML, langIdofDoc, docLangId, uiLangId, highlightParams);
            this.GetDocLangs(docLangId);
            this.BloblInfo = AkomaNtosoPreProcessor.GetBlobInfo(strXML);
        }

        public void SetKeywords(string[,] keywords, int siteLangId)
        {
            var rubrics = new AkomaNtosoPreProcessorConfig { UILanguage = siteLangId }.Rubrics;

            for (int i = 0; i < keywords.Length / 2; i++)
            {
                var key = Doc.GetUnifiedDocInfoSource(keywords[i, 0], this.Publisher, this.AdditionalInfo, rubrics);

                this.Keywords.Add(key, keywords[i, 1]);
            }
        }

        public void SetSummaries(string[,] summaries, int siteLangId)
        {
            var rubrics = new AkomaNtosoPreProcessorConfig { UILanguage = siteLangId }.Rubrics;

            for (int i = 0; i < summaries.Length / 2; i++)
            {
                var key = Doc.GetUnifiedDocInfoSource(summaries[i, 0], this.Publisher, this.AdditionalInfo, rubrics);

                this.Summaries.Add(key, Doc.PrepairDocInfoLinks(summaries[i, 1], this.LangId));
            }
        }

        /// <summary>
        /// Sets related documents
        /// </summary>
        /// <param name="numbers">[i,0] holds Celex entry; [i,1] holds optional DocLangId; [i,2] holds Date</param>
        public void SetRelatedDocNumbers(string[,] numbers)
        {
            for (int i = 0; i < numbers.GetLength(0); i++)
            {
                this.RelatedDocNumbers.Add(new RelatedDoc(numbers[i, 0], numbers[i, 1], numbers[i, 2]));
            }
        }

        private void GetDocumentContent(string strXML, int langIdOfDoc, int docLangId, int uiLangId, DocHighlightSearchParams highlightParams)
        {
            var config = new AkomaNtosoPreProcessorConfig
            {
                DocumentLanguage = langIdOfDoc,
                UILanguage = uiLangId,
                AkomaNtosoXmlId = docLangId,
                ConsolidatedInfo = Doc.GetDocConsVersions(docLangId)
            };

            var docContents = AkomaNtosoPreProcessor.ConvertToFragments(strXML, config);

            //local search initial highlight
            if (highlightParams.SearchText != null)
            {
                this.SearchedText = highlightParams.SearchText;
                this.ExactMatch = highlightParams.ExactMatch;

                if (highlightParams.MultilingualSearchedText != null)
                {
                    this.HighlightForSearch(docContents, highlightParams.SearchText + " " + highlightParams.MultilingualSearchedText, langIdOfDoc, highlightParams.ExactMatch,
                        highlightParams.SearchedCelex, highlightParams.SearchedPars);
                }
                else
                {
                    this.HighlightForSearch(docContents, highlightParams.SearchText, langIdOfDoc, highlightParams.ExactMatch, highlightParams.SearchedCelex, highlightParams.SearchedPars);
                }
            }
            else if (!String.IsNullOrEmpty(highlightParams.SearchedCelex) || (highlightParams.SearchedPars != null && highlightParams.SearchedPars.Count > 0))
            {
                this.HighlightForSearch(docContents, null, langIdOfDoc, highlightParams.ExactMatch, highlightParams.SearchedCelex, highlightParams.SearchedPars);
            }

            // Fix text links
            //docContents.DocumentContent = Regex.Replace(docContents.DocumentContent, @"\<[^\<\>]*?\>", delegate (Match m)
            //{
            //    return "".PadLeft(m.Value.Length, ' ');
            //});

            this.htmlModel = docContents;
        }

        private void GetDocLangs(int docLangId)
        {
            this.DocLangs = Doc.GetDocLangs(docLangId);
        }

        public bool IsTreaty()
        {
            bool hasDocumentNumber = !String.IsNullOrEmpty(this.DocNumber);

            bool isEurlex = !String.IsNullOrEmpty(this.Country)
                ? this.Country == "EU"
                : this.HtmlModel != null
                ? this.HtmlModel.PublisherId == "EURLEX"
                : false;

            bool istreaty = hasDocumentNumber
                && isEurlex
                && this.DocNumber.StartsWith("1");

            return istreaty;
        }

        public bool IsEuAct()
        {
            bool hasDocumentNumber = !String.IsNullOrEmpty(this.DocNumber);

            bool isEurlex = !String.IsNullOrEmpty(this.Country)
                ? this.Country == "EU"
                : this.HtmlModel != null
                ? this.HtmlModel.PublisherId == "EURLEX"
                : false;

            bool isEuAct = hasDocumentNumber
                && isEurlex
                && (this.DocNumber.StartsWith("0")
                || this.DocNumber.StartsWith("1")
                || this.DocNumber.StartsWith("2")
                || this.DocNumber.StartsWith("3")
                || this.DocNumber.StartsWith("4"));

            return isEuAct;
        }

        public bool IsEuFins()
        {
            var isEufins = !String.IsNullOrEmpty(this.HtmlModel?.PublisherId);
            isEufins = isEufins && this.HtmlModel.PublisherId.Equals("EUFINS_DOC", StringComparison.OrdinalIgnoreCase);
            if (!isEufins)
            {
                isEufins = !String.IsNullOrEmpty(this.Publisher) && this.Publisher.Equals("EUFINS_DOC", StringComparison.OrdinalIgnoreCase);
            }

            return isEufins;
        }

        public bool IsEurlex()
        {
            var isEurlex = !String.IsNullOrEmpty(this.HtmlModel?.PublisherId);
            isEurlex = isEurlex && this.HtmlModel.PublisherId == "EURLEX";

            return isEurlex;
        }

        public String BuildEurlexUrl()
        {
            if (this.IsEuFins())
            {
                var celexObject = this.ConvertEuFinsDocumentNumberToCelex();

                return celexObject?.ToEurLexUrl();
            }
            else
            {
                return null;
            }
        }

        public String BuildEurocasesUrl(int prefLangId)
        {
            if (this.IsEuFins())
            {
                var celexObject = this.ConvertEuFinsDocumentNumberToCelex();

                //  var rewriter = new DocLinksRewriter(1, false);

                return "/Doc/Act/" + prefLangId + "/" + celexObject.ToApisHref().Replace("./", String.Empty).Replace("CELEX=", String.Empty);

                //   return rewriter.RewriteDocInLinks(celexObject?.ToApisHref(), "inline_link", false);
            }
            else
            {
                return null;
            }
        }

        public String GetFriendlyLookForEuFinsDocumentNumber()
        {
            if (this.IsEuFins())
            {
                //  example: C-53808C-3309 -> C-53808 / C-3309

                var matches = Regex.Matches(this.DocNumber, @"(c\-\d+)(\/\d+)?", RegexOptions.IgnoreCase);
                if (matches.Count >= 1)
                {
                    var values = matches.OfType<Match>().Select(x => x.Value);
                    var firendlyLook = String.Join(" / ", values);

                    return firendlyLook;
                }
                else
                {
                    return this.DocNumber;
                }
            }
            else
            {
                return this.DocNumber;
            }
        }

        public bool DoesTitleContainsConsolidatedVersionText()
        {
            bool hasTitle = !String.IsNullOrEmpty(this.Title);
            bool doesContainsConsolidatedText = hasTitle
                    && Regex.Match(this.Title, Constant.Constants.TreatiesConsolidatedTextRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Success;

            return doesContainsConsolidatedText;
        }

        public Celex ConvertEuFinsDocumentNumberToCelex()
        {
            // when there is double number in the original document number, we must extract only the first
            // example: C-53808C-3309 -> C-53808
            Func<String, String> fixDoubleNumbers = number =>
            {
                number = number.Replace("/", String.Empty);
                var matches = Regex.Matches(number, @"c\-\d+", RegexOptions.IgnoreCase);
                if (matches.Count > 1)
                    return matches[0].Value;
                else return number;
            };

            try
            {
                const int yearDescriptorLength = 2;
                const int numberDescriptorStartIndex = 2;
                var unifedDocumentNumber = fixDoubleNumbers(this.DocNumber);

                var currentYearLastTwoDigits = $"{DateTime.Now.Year}".Substring(2, 2);

                // C-20290 -> 61990CJ0202

                var courtDescriptor = $"{unifedDocumentNumber[0]}";
                var yearDescriptor = unifedDocumentNumber.Substring(unifedDocumentNumber.Length - yearDescriptorLength, yearDescriptorLength); // reperesent the last two digits in the number
                var numberDescriptor =
                    unifedDocumentNumber.Substring(numberDescriptorStartIndex, unifedDocumentNumber.Length - yearDescriptorLength - numberDescriptorStartIndex); // represents the digits between the court and year descriptor
                var yearLeadDescriptor = yearDescriptor.CompareTo(currentYearLastTwoDigits) <= 0 ? "20" : "19"; // detems if the year of the decision is from the 20th or 21th century

                var celexYear = $"{yearLeadDescriptor}{yearDescriptor}";
                var celexNumber = int.Parse(numberDescriptor).ToString("0000"); // fills with zeros util the fourth charachter
                var celex = $"6{celexYear}{courtDescriptor}J{celexNumber}";
                var celexObject = Celex.TryParse(celex);

                return celexObject;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void RewriteInLineLinks(DocLinksRewriter rewriter)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var htmlContent = this.HtmlModel.GetHtmlContent();

            bool isInDocumentText = false;

            foreach (var item in htmlContent)
            {
                if (item.Key == XsltContentOrigin.DocumentContent)
                {
                    isInDocumentText = true;
                }
                else
                {
                    isInDocumentText = false;
                }

                foreach (var subElement in item.Value)
                {
                    subElement.Html = rewriter.RewriteDocInLinks(subElement.Html, "inline_link", isInDocumentText);
                }
            }


            stopwatch.Stop();
            var b = stopwatch.Elapsed.TotalMilliseconds;
        }

        public void RewriteLinksInDocList(DocLinksRewriter rewriter)
        {
            rewriter.RewriteDocInLinks(this.Title, "inline_link", false);

            // hacking the enumerator cause I am to lazy to itterate over keys, sry, and God said: "let there be memory on the server..."
            var keywords = new Dictionary<string, string>(this.Keywords);
            var summaries = new Dictionary<string, string>(this.Summaries);

            foreach (var keyword in keywords)
            {
                this.Keywords[keyword.Key] = rewriter.RewriteDocInLinks(keyword.Value, "inline_link", false);
            }

            foreach (var summary in summaries)
            {
                this.Summaries[summary.Key] = rewriter.RewriteDocInLinks(summary.Value, "inline_link", false);
            }
        }

        #region Highlights

        private void HighlightForSearch(FragmentsResult fragment, string searchText, int langIdOfDoc, bool exactMatch, string searchedCelex, List<string> searchedPars)
        {
            ApisLucene.Classes.Eucases.Highlight.Config.IConfig config = null;
            ApisLucene.Classes.Eucases.Stemming.IStemmer stemmer = null;

            switch (langIdOfDoc)
            {
                case 1: config = new BgConfig(); stemmer = new BgStemmer(); break;
                case 2: config = new DeConfig(); stemmer = new DeStemmer(); break;
                case 3: config = new FrConfig(); stemmer = new FrStemmer(); break;
                case 4: config = new EnConfig(); stemmer = new EnStemmer(); break;
                default: config = new EnConfig(); stemmer = new EnStemmer(); break;
            }

            var allFragmentContents = fragment.GetHtmlContent();

            foreach (var fragmentContents in allFragmentContents)
            {
                this.HandleFragmentHighlight(fragmentContents.Value.ToList(), config, stemmer, searchText, exactMatch, searchedCelex, searchedPars);
            }

            /*   if (fragment.Keywords != null && fragment.Keywords.Content != null)
               {
                   this.HandleFragmentListHighlight(fragment.Keywords, config, stemmer, searchText, exactMatch, searchedCelex, searchedPars); // TODO
               }

               if (fragment.Summaries != null && fragment.Summaries.Content != null)
               {
                   this.HandleFragmentListHighlight(fragment.Summaries, config, stemmer, searchText, exactMatch, searchedCelex, searchedPars); // TODO
               }*/

            this.HandleTitleHighlight(fragment.Title, config, stemmer, searchText, exactMatch);
        }

        private void HandleTitleHighlight(RubricValuePairFragment titleFragment, ApisLucene.Classes.Eucases.Highlight.Config.IConfig config,
            ApisLucene.Classes.Eucases.Stemming.IStemmer stemmer, string searchText, bool exactMatch)
        {
            var highlightPositions = this.GetHighlightPositions(titleFragment.Value, config, stemmer, searchText, exactMatch);

            /* Eventual links highlight ? */

            string newHTML = this.AppendHighLights(titleFragment.Value, highlightPositions);

            titleFragment.Value = newHTML;
        }

        private void HandleFragmentListHighlight(Fragment<List<SourceFragment>> fragmentList, ApisLucene.Classes.Eucases.Highlight.Config.IConfig config,
            ApisLucene.Classes.Eucases.Stemming.IStemmer stemmer, string searchText, bool exactMatch, string searchedCelex, List<string> searchedPars)
        {
            var desiredFragmentListElement = fragmentList.Content.FirstOrDefault();

            var sourceFr = fragmentList.Content.FirstOrDefault();
            var highlightedFragments = new List<string>();
            var desiredLanguage = sourceFr.LanguageGroups.FirstOrDefault();

            foreach (var item in desiredLanguage)
            {
                string htmlText = item.ToString();
                var highlightPositions = this.GetHighlightPositions(htmlText, config, stemmer, searchText, exactMatch);

                /* Links Highlighting */
                if (!String.IsNullOrEmpty(searchedCelex))
                {
                    var additionalPositions = this.GetLinkHighlightPositions(htmlText, searchedCelex, searchedPars);
                    highlightPositions.AddRange(additionalPositions);
                }

                string newHTML = this.AppendHighLights(htmlText, highlightPositions);
                highlightedFragments.Add(newHTML);
            }

            desiredLanguage.Clear();
            desiredLanguage.AddRange(highlightedFragments.Select(x => new XsltContent { Html = x }));

        }

        private void HandleFragmentHighlight(List<XsltContent> fragment, ApisLucene.Classes.Eucases.Highlight.Config.IConfig config,
            ApisLucene.Classes.Eucases.Stemming.IStemmer stemmer, string searchText, bool exactMatch, string searchedCelex, List<string> searchedPars)
        {
            var highlightedFragments = new List<string>();
            var curFragmentsCount = fragment.Count;

            foreach (var content in fragment)
            {
                string htmlText = content.Html;

                var highlightPositions = this.GetHighlightPositions(htmlText, config, stemmer, searchText, exactMatch);

                /* Links Highlighting */
                if (!String.IsNullOrEmpty(searchedCelex))
                {
                    var additionalPositions = this.GetLinkHighlightPositions(htmlText, searchedCelex, searchedPars);
                    highlightPositions.AddRange(additionalPositions);
                }

                string newHtml = htmlText;

                if (highlightPositions.Count > 0)
                {
                    newHtml = this.AppendHighLights(htmlText, highlightPositions);
                }

                content.Html = newHtml;

                highlightedFragments.Add(newHtml);
            }

            //  fragment.FirstOrDefault().Html

            //  fragment.RemoveRange(0, curFragmentsCount);
            //   fragment.AddRange(highlightedFragments.Select(x => new XsltContent { Html = x }));
        }

        /// <summary>
        /// Used to get matches positions of a link with specified celex in HTML document. Optionally takes "Par".
        /// </summary>
        /// <returns>List of TextPositions that are about to be highlighted</returns>
        private List<TextPosition> GetLinkHighlightPositions(string htmlText, string searchedCelex, List<string> searchedPars)
        {
            if (searchedPars == null)
            {
                searchedPars = new List<string>();
            }

            var positions = BaseParser.FindLink(htmlText, searchedCelex, searchedPars);

            return positions;
        }

        /// <summary>
        /// Used to get matches positions of searched text in HTML document.
        /// </summary>
        /// <param name="htmlText">HTML Text to be searched in</param>
        /// <param name="config">Language configuration</param>
        /// <param name="stemmer">Language stemmer for some words transformations.</param>
        /// <param name="searchText">Searched text</param>
        /// <param name="exactMatch">Flags if texts is about to be matched exactly</param>
        /// <returns></returns>
        private List<TextPosition> GetHighlightPositions(string htmlText, ApisLucene.Classes.Eucases.Highlight.Config.IConfig config,
            ApisLucene.Classes.Eucases.Stemming.IStemmer stemmer, string searchText, bool exactMatch)
        {
            BaseParser parser = new BaseParser(config, new BaseTokenizer(config), stemmer);

            string noTagshtmlText = Regex.Replace(htmlText, @"\<[^\<\>]*?\>", delegate (Match m)
            {
                return "".PadLeft(m.Value.Length, ' ');
            });
            parser.AnalizeText(noTagshtmlText);

            //int freeWords = 30;
            //bool isPhrase = false;

            try
            {
                List<TextPosition> res = parser.FindLema(searchText, !exactMatch);

                return res;
            }
            catch (Exception)
            {
                return new List<TextPosition>();
            }

        }

        private string AppendHighLights(string htmlText, List<TextPosition> positions)
        {
            positions = positions.OrderBy(p => p.StartPos).ToList();
            StringBuilder sb = new StringBuilder();
            int startIndex = 0;
            foreach (var item in positions)
            {
                sb.Append(htmlText.Substring(startIndex, item.StartPos - startIndex));
                sb.Append("<span class=\"highlight\">" + htmlText.Substring(item.StartPos, item.Length) + "</span>");
                startIndex = item.StartPos + item.Length;
            }

            sb.Append(htmlText.Substring(startIndex));

            return sb.ToString();
        }

        #endregion
    }

    public enum AdditionalInfoEnum
    {
        None = 0,
        InternationalAgreement = 1,
        ExpertMaterials = 2
    }
}
