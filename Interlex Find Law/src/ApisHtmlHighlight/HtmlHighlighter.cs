namespace ApisHtmlHighlight
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using ApisLucene.Classes.Eucases.Highlight.Config.BG;
    using ApisLucene.Classes.Eucases.Highlight.Parser;
    using ApisLucene.Classes.Eucases.Highlight.Tokenizer;
    using ApisLucene.Classes.Eucases.Stemming.BG;
    using ApisLucene.Classes.Eucases.Highlight.Config.DE;    //using Newtonsoft.Json;
    using ApisLucene.Classes.Eucases.Highlight.Config.FR;
    using ApisLucene.Classes.Eucases.Highlight.Config.EN;
    using ApisLucene.Classes.Eucases.Stemming.EN;
    using ApisLucene.Classes.Eucases.Stemming.DE;
    using ApisLucene.Classes.Eucases.Stemming.FR;
    public class HtmlHighlighter
    {
        private EnConfig config;

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

        private string AppendHighlights(string htmlText, List<TextPosition> positions, int count)
        {
            StringBuilder sb = new StringBuilder();
            int startIndex = 0;
            foreach (var item in positions)
            {
                sb.Append(htmlText.Substring(startIndex, item.StartPos - startIndex));
                sb.Append("<font id=\"ss-" + count + "\" class=\"srch_f\">" + htmlText.Substring(item.StartPos, item.Length) + "</font>");
                startIndex = item.StartPos + item.Length;
                count++;
            }

            sb.Append(htmlText.Substring(startIndex));
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new { html = sb.ToString(), count = count });
            return json;
        }

        private string AppendHighlights(string htmlText, List<TextPosition> positions)
        {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="searchText"></param>
        /// <param name="count">The current count the search has reached</param>
        /// <returns>Json with html and count</returns>
        public string HighlightHtml(string htmlText, string searchText, int count)
        {
            var config = new BgConfig();
            var stemmer = new BgStemmer();
            List<TextPosition> highlightPositions = this.GetHighlightPositions(htmlText, config, stemmer, searchText, false);
            string jsonHtmlAndCount = this.AppendHighlights(htmlText, highlightPositions, count);

            return jsonHtmlAndCount;
        }

        public string HighlightHtmlEuroCases(string htmlText, string searchText, int langIdOfDoc)
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

            var highlightPositions = this.GetHighlightPositions(htmlText, config, stemmer, searchText, false);
            string heighlightedHtml = this.AppendHighlights(htmlText, highlightPositions);
            return heighlightedHtml;
        }
    }
}

