using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Interlex.BusinessLayer.Models;
using Interlex.DataLayer;
using System.Globalization;

namespace Interlex.BusinessLayer
{
    public class CourtActBL : Doc
    {
        public static CourtAct GetCourtAct(
            int id,
            int uiLangId,
            int userId,
            DocHighlightSearchParams highlightParams,
            int productId,
            bool showFreeDocuments = true)
        {
            CourtAct courtAct = null;
            DataTable dt = DB.GetDocument(id, userId, productId);
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];

                courtAct = new CourtAct();
                courtAct.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                courtAct.DocType = Convert.ToInt32(row["doc_type"]);
                courtAct.LangId = Convert.ToInt32(row["lang_id"]);
                courtAct.Title = row["title"].ToString();
                
                
                // refactor
                courtAct =
                    new CourtAct(
                        GetDocText(id, courtAct.DocType, productId, showFreeDocuments), 
                        courtAct.LangId, 
                        courtAct.DocLangId,
                        uiLangId,
                        highlightParams);

                courtAct.DocLangId = Convert.ToInt32(row["doc_lang_id"]);
                courtAct.DocType = Convert.ToInt32(row["doc_type"]);
                courtAct.DocNumber = row["doc_number"].ToString();
                courtAct.LangId = Convert.ToInt32(row["lang_id"]);
                courtAct.HasInLinks = Convert.ToBoolean(row["has_in_links"]);
                courtAct.Title = row["title"].ToString();
                courtAct.UserDocId = row["user_doc_id"].ToString();
                courtAct.ProductIds = (int[])row["products"];
                courtAct.Country = row["country"].ToString();
                courtAct.BaseDocLangId = int.Parse(row["base_act_id"].ToString());
                courtAct.ExactMatch = highlightParams.ExactMatch;
            }

            return courtAct;
        }

        /// <summary>
        /// Represents map from db language id to culture info. If the language id is not mapped or exception occures, the default value will be returend
        /// </summary>
        /// <param name="langaugeId"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static CultureInfo LanguageIdToCultureInfo(int langaugeId, CultureInfo @default)
        {
            /*
            1	bul
            2	deu
            3	fra
            4	eng
            5	ita
            6	est
            7	gre
            8	gle
            9	lav
            10	lit
            11	hun
            12	mlt
            13	dan
            14	cze
            15	spa
            16	srp
            17	hrv
            18	swe
            19	fin
            20	slv
            21	slo
            22	rum
            23	por
            24	pol
            25	dut
            26	isl
            27	nor
            28	grc
            */

            switch (langaugeId)
            {
                case 1: return CultureInfo.GetCultureInfo("bg");
                case 2: return CultureInfo.GetCultureInfo("de");
                case 3: return CultureInfo.GetCultureInfo("fr");
                case 4: return CultureInfo.GetCultureInfo("en");
                case 5: return CultureInfo.GetCultureInfo("it");
                default: return @default;
            }
        }

        /// <summary>
        /// Safe execution of rubrics provider for the translations of the langauge of the document
        /// Safe call is wrapped over try..catch clause for safe exection.
        /// </summary>
        /// <param name="langaugeId"></param>
        /// <param name="titlesInDocumentLanguageProvider"></param>
        /// <returns>The loaded resource for the language id of the document or Empty dictionary on error.</returns>
        private static IDictionary<String, String> SafeTitlesInDocumentLanguageProvider(int langaugeId, Func<CultureInfo, IDictionary<String, String>> titlesInDocumentLanguageProvider)
        {
            try
            {
                // represnts a culture by the langauge of the document which is used to load the resources for the specifed culture
                CultureInfo currentCultureByDocumentLanguage = LanguageIdToCultureInfo(langaugeId, CultureInfo.GetCultureInfo("en"));

                return titlesInDocumentLanguageProvider(currentCultureByDocumentLanguage);
            }
            catch (Exception e)
            {
                return new Dictionary<String, String>();
            }
        }
    }
}
