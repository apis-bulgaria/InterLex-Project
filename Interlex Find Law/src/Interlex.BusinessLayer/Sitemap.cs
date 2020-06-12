using System;
using System.Collections.Generic;
using Interlex.DataLayer;

namespace Interlex.BusinessLayer
{
    public class Sitemap
    {
        public static ICollection<string> GetDocLinksSitemap()
        {
            var dbRes = DB.GetDocLinksSitemap();
            var links = new List<string>();
            const string linkStart = "http://freecases.eu/Doc";

            foreach (var dataRec in dbRes)
            {
                string docType = Convert.ToInt32(dataRec["doc_type"]) == 1 ? "CourtAct" : "LegalAct";
                int docLangId = Convert.ToInt32(dataRec["doc_lang_id"]);
                links.Add(linkStart + "/" + docType + "/" + docLangId);
            }

            return links;
        }
    }
}
