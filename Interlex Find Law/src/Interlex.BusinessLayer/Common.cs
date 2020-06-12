using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;
using UnidecodeSharpFork;
using Interlex.BusinessLayer.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;

namespace Interlex.BusinessLayer
{
    public class Common
    {
        public static string ComputeMD5(string input)
        {
            byte[] hash;
            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            }

            return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string Transliterate(string input)
        {
            return input.Unidecode();
        }

        bool invalid = false;
        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public static SearchBox FixSearchBoxFiltersDatesForJSON(SearchBox searchBoxFilters)
        {
            if (searchBoxFilters.Cases != null)
            {
                if (searchBoxFilters.Cases.DateFrom == null)
                {
                    searchBoxFilters.Cases.DateFrom = new DateTime(1, 1, 1);
                }

                if (searchBoxFilters.Cases.DateTo == null)
                {
                    searchBoxFilters.Cases.DateTo = new DateTime(1, 1, 1);
                }
            }

            if (searchBoxFilters.Law != null)
            {
                if (searchBoxFilters.Law.DateFrom == null)
                {
                    searchBoxFilters.Law.DateFrom = new DateTime(1, 1, 1);
                }

                if (searchBoxFilters.Law.DateTo == null)
                {
                    searchBoxFilters.Law.DateTo = new DateTime(1, 1, 1);
                }
            }

            if (searchBoxFilters.Finances != null)
            {
                if (searchBoxFilters.Finances.DateFrom == null)
                {
                    searchBoxFilters.Finances.DateFrom = new DateTime(1, 1, 1);
                }

                if (searchBoxFilters.Finances.DateTo == null)
                {
                    searchBoxFilters.Finances.DateTo = new DateTime(1, 1, 1);
                }
            }

            return searchBoxFilters;
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static bool CheckRequestOriginIsGoogle(string userAgent, string ip)
        {
            if (userAgent.ToLower().Contains("googlebot") || userAgent.ToLower().Contains("google.com/bot.html")
                || userAgent.ToLower().Contains("mediapartners-google") || ip.StartsWith("66.249"))
            {
                return true;
            }

            return false;
        }

        public static bool CheckRequestOriginIsBotSoft(string userAgent)
        {
            List<string> botsList = new List<string>()
            {
                "googlebot", "dotbot", "mj12bot", "bingbot", "ahrefsbot", "seoscanners", "seznambot", "bot"
            };
            var ual = userAgent.ToLower();

            if (botsList.Exists(x => ual.Contains(x)))
            {
                return true;
            }
           
            return false;
        }

        public static bool CheckRequestOriginIsBot(string userAgent, string ip)
        {
            List<string> botCrawlersList = new List<string>()
            {
                "bot","crawler","spider","80legs","baidu","yahoo! slurp","ia_archiver","mediapartners-google",
                "lwp-trivial","nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne",
                "atn_worldwide","atomz","bjaaland","ukonline","calif","combine","cosmos","cusco",
                "cyberspyder","digger","grabber","downloadexpress","ecollector","ebiness","esculapio",
                "esther","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
                "gcreep","golem","griffon","gromit","gulliver","gulper","whowhere","havindex","hotwired",
                "htdig","ingrid","informant","inspectorwww","iron33","teoma","ask jeeves","jeeves",
                "image.kapsi.net","kdd-explorer","label-grabber","larbin","linkidator","linkwalker",
                "lockon","marvin","mattie","mediafox","merzscope","nec-meshexplorer","udmsearch","moget",
                "motor","muncher","muninn","muscatferret","mwdsearch","sharp-info-agent","webmechanic",
                "netscoop","newscan-online","objectssearch","orbsearch","packrat","pageboy","parasite",
                "patric","pegasus","phpdig","piltdownman","pimptrain","plumtreewebaccessor","getterrobo-plus",
                "raven","roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au",
                "searchprocess","senrigan","shagseeker","site valet","skymob","slurp","snooper","speedy",
                "curl_image_client","suke","www.sygol.com","tach_bw","templeton","titin","topiclink","udmsearch",
                "urlck","valkyrie libwww-perl","verticrawl","victoria","webscout","voyager","crawlpaper",
                "webcatcher","t-h-u-n-d-e-r-s-t-o-n-e","webmoose","pagesinventory","webquest","webreaper",
                "webwalker","winona","occam","robi","fdse","jobo","rhcs","gazz","dwcp","yeti","fido","wlm",
                "wolp","wwwc","xget","legs","curl","webs","wget","sift","cmc"
            };

            string ual = userAgent.ToLower();

            return botCrawlersList.Exists(x => ual.Contains(x));
        }

        public static bool ValidateCaptcha(string token)
        {
            bool res = false;

            var request = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify");
            var secret = ConfigurationManager.AppSettings["RecaptchaServer"];

            var postData = $"secret={secret}";
            postData += "&response=" + token;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var responseObj = JsonConvert.DeserializeObject<dynamic>(responseString);
            res = responseObj.success;

            return res;
        }

    }
}
