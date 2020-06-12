namespace Interlex.App.Helpers
{
    using System;
    using System.Text;
    using System.Web;

    public enum SearchBoxClearType
    {
        Simple = 1,
        Classifier = 2,
        Enactment = 3,
        Provision = 4,
        ECHRProvision = 5,
        FinancesDocumentTypes = 6,
        MultilingualDictionary = 7,
        EnactmentIndex = 8,
        ProvisionIndex = 9
    }

    public class EuroCasesClear : IHtmlString
    {
        private readonly SearchBoxClearType clearType;
        private readonly string clearableElementId;
        
        public EuroCasesClear(SearchBoxClearType clearType, string clearableElementId)
        {
            this.clearType = clearType;
            this.clearableElementId = clearableElementId;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($@"<span class=""fa-stack eucs-clear eucs-clear-{this.clearType}"" data-element=""{this.clearableElementId}"">");
            builder.Append(@"<i class=""fa fa-square-o fa-stack-2x""></i>");
            builder.Append(@"<i class=""fa fa-times fa-stack-1x""></i>");
            builder.Append("</span>");

            return builder.ToString();
        }

        public string ToHtmlString()
        {
            return this.ToString();
        }
    }
}