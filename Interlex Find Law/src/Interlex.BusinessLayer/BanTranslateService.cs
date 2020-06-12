using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Interlex.BusinessLayer
{
    public class TranslateServiceJson
    {

        public string Action { get; set; }
        public string SourceLang { get; set; }
        public string TargetLang { get; set; }
        public string Text { get; set; }

    }

    public class Translated
    {
        public string src_tokenized { get; set; }
        public double score { get; set; }
        public int rank { get; set; }
        public string text { get; set; }
    }

    public class Translation
    {
        public List<Translated> translated { get; set; }
        public string translationId { get; set; }
    }

    public class RootObject
    {
        public int errorCode { get; set; }
        public List<Translation> translation { get; set; }
        public string errorMessage { get; set; }
    }

    public class BanTranslateService
    {
        public string Translate(string text, string sourceLanguage)
        {
            string res = "";


            var request = (HttpWebRequest)WebRequest.Create("http://213.191.204.69:9990/btb");

            string postData = GetJsonString(text, sourceLanguage);

            request.Method = "POST";

            request.ContentType = "application/json";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            res = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            res = res.Replace("src-tokenized", "src_tokenized");

            // the professor will change this but BAN members are capable of many things so we can only guess what universal
            // structure format he will use so will leave Vanga stuff out of it and change it after he changes it
            var jsonObject = JsonConvert.DeserializeObject<List<LangTranslationItem>>(res);
            
            if (jsonObject != null && jsonObject.Count > 0)
            {
                res = String.Empty;
                foreach (var translationItem in jsonObject)
                {
                    res += "(";
                    res += translationItem.Text;
                    res += ")";
                    res += " || ";
                }

                res = res.Remove(res.Length - 4);
            }
            else
            {
                res = null; // not returned?
            }

            return res;
        }

        private string GetJsonString(string text, string sourceLanguage)
        {
            TranslateServiceJson obj = new TranslateServiceJson
            {
                Action = "translate",
                SourceLang = sourceLanguage,
                Text = text
            };

            var camelCaseFormatter = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string res = JsonConvert.SerializeObject(obj, camelCaseFormatter);
            return res;
        }

        public class LangTranslationItem 
        {
            public string Lang { get; set; }

            public string Text { get; set; }
        }
    }
}
