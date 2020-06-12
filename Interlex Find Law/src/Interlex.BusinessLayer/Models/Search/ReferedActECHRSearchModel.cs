namespace Interlex.BusinessLayer.Models
{
    using ApisLucene.Classes.Common;
    using ApisLucene.Classes.Eucases.Search;
    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    public class ReferedActECHRSearchModel
    {
        public List<Document> Documents { get; set; }

        public ReferedActECHRSearchModel()
        {
            this.Documents = new List<Document>();
        }

        public int[] GetDocLangIdsResByQuery(string searchQuery, string basePath)
        {
            var realPath = Path.Combine(basePath, ConfigurationManager.AppSettings["SearchWrapper_BasePath"]);
            var wrapper = new EUCasesSearchWrapper(ConfigurationManager.AppSettings["SearchWrapper_BasePath"]);

            var result = wrapper.SearchQuery(searchQuery, true);

            return result;
        }

        public static int[] GetDocLangIdsRes(string applicant, string applicationNumber, string ecli, string basePath)
        {
            var paramsp = new SearchParams();
            paramsp.DocType = DocumentTypeEnum.Judgment;
            paramsp.CaseLawType = ApisLucene.Classes.Common.CaseLawType.ECHR;

            if (!String.IsNullOrEmpty(applicant))
            {
                paramsp.Applicant = applicant;
            }

            if (!String.IsNullOrEmpty(applicationNumber))
            {
                paramsp.DocumentNumber = applicationNumber;
            }

            if (!String.IsNullOrEmpty(ecli))
            {
                paramsp.NationalIdOrEcli = ecli;
            }

            var realPath = Path.Combine(basePath, ConfigurationManager.AppSettings["SearchWrapper_BasePath"]);

            var dic = paramsp.GetParams();
            var searchWrapper = new EUCasesSearchWrapper(realPath);
            var res = searchWrapper.SearchDictBoost(dic, false);

            return res;
        }
    }
}
