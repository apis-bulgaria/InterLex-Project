namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interlex.DataLayer;

    public class DocLinksFilter
    {
        public int SiteLangRequestingId { get; set; }
        public int UserRequestingId { get; set; }

        public bool IsOriginApi { get; set; }

        public string Domain { get; set; } = "all";

        public int ToDocLangId { get; set; }

        public string ToDocNumber { get; set; }

        public int? ToParId { get; set; }

        /// <summary>
        /// Search List result page title
        /// </summary>
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string LinkIdsString { get; set; } = String.Empty;

        public string ToParOriginal { get; set; }

        public List<string> ToPars { get; set; }

        public void PopulateLinksToPars()
        {
            this.ToPars = DB.GetLinksToPars(this.LinkIdsString.Split('-').Select(l => int.Parse(l)).ToArray()).ToList();
        }

        public static string GetParNumByParId(int parId)
        {
            return DB.GetParNumByParId(parId);
        }
    }
}
