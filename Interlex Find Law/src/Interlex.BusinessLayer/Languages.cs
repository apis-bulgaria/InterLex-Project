using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Interlex.BusinessLayer.Models;
using Interlex.DataLayer;
using System.Reflection;
using System.Globalization;
using System.Resources;
using System.Diagnostics;

namespace Interlex.BusinessLayer
{
    public class Languages
    {
        public static List<Alphabet> PopulateAlphabets()
        {
            List<Alphabet> alphabets = new List<Alphabet>();

            foreach (var record in DB.GetMultiDictAlphabetLetters())
            {
                var curLetter = new AlphabetLetter(record["letter"].ToString(), Convert.ToBoolean(record["has_occurences"]));

                if (alphabets.Any(a => a.LangId == Convert.ToInt32(record["lang_id"])))
                {
                    alphabets.Where(a => a.LangId == Convert.ToInt32(record["lang_id"])).FirstOrDefault().Letters.Add(curLetter);
                }
                else
                {
                    var newAlphabetObj = new Alphabet(Convert.ToInt32(record["lang_id"]), new SortedSet<AlphabetLetter>(new AlphabetComparer()));
                    newAlphabetObj.Letters.Add(curLetter);
                    alphabets.Add(newAlphabetObj);
                }
            }
            
            return alphabets;
        }

        public static void RePopulateAlphabetsToCache()
        {
            CacheProvider.Provider.DeleteCacheItem("alphabets");
            CacheProvider.Provider.GetOrSetForever("alphabets", () => Languages.PopulateAlphabets());
        }

        public static SortedSet<AlphabetLetter> GetAlphabetByLangId(int langId)
        {
            var alphabets = CacheProvider.Provider.GetOrSetForever("alphabets", () => Languages.PopulateAlphabets());

            if (alphabets.Any(a => a.LangId == langId))
            {
                return alphabets.Where(a => a.LangId == langId).FirstOrDefault().Letters;
            }
            else
            {
                return alphabets.FirstOrDefault().Letters;
            }
        }

        /// <summary>
        /// Gets all langs from DB ant transforms them to a HashSet from the appropriate model
        /// </summary>
        /// <param name="populateGreekForCyprus">Marks if greek language should be populated again for the entry that is stored in DB for Cyprus</param>
        public static IEnumerable<Language> GetAllLangs(bool populateGreekForCyprus)
        {
            var langs = new HashSet<Language>();

            foreach (var record in DB.GetAllLangs())
            {
                if (Convert.ToInt32(record["id"]) == 28 && !populateGreekForCyprus)
                {
                    continue; // Not populating greek second time if boolean flagged that way
                }

                var curLang = new Language();
                curLang.Id = Convert.ToInt32(record["id"]);
                curLang.IsInterfaceLang = Convert.ToBoolean(record["is_site_lang"]);
                curLang.Text = record["name"].ToString();
                curLang.Code = record["code"].ToString();
                curLang.ShortCode = record["short_lang"].ToString();

                langs.Add(curLang);
            }

            return langs;
        }

        public static IEnumerable<Language> GetAllLangsFromCache()
        {
            var langs = CacheProvider.Provider.GetOrSetForever("languages", () => Languages.GetAllLangs(false));

            return langs;
        }

        public static Language GetLang(int id)
        {
            Language lang = InterfaceLanguages.GetLanguageById(id);
            if (lang == null)
            {
                DataTable dt = DB.GetLang(id);
                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    lang = new Language();

                    lang.Id = id;
                    lang.Code = row["code"].ToString();
                    lang.ShortCode = row["short_lang"].ToString();
                    lang.Text = row["name"].ToString();
                }
            }
            return lang;
        }

        public static IEnumerable<Country> GetCountries(int langId)
        {
            var countries = new List<Country>();

            foreach (var item in DB.GetCountries(langId))
            {
                var country = new Country(
                        int.Parse(item["id"].ToString()),
                        item["code"].ToString(),
                        false,
                        null,
                        item["name"].ToString()
                    );

                if (item["vat_id"] != null && item["vat_id"].ToString() != "")
                {
                    country.VatId = int.Parse(item["vat_id"].ToString());
                }

                if (item["eu"].ToString() == "True")
                {
                    country.IsEu = true;
                }

                countries.Add(country);
            }

            return countries;
        }
    }
}
