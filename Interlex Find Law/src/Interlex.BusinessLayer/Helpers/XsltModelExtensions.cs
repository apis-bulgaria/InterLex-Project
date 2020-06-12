namespace Interlex.BusinessLayer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AkomaNtosoXml.Xslt.Core.Classes.Providers;

    public static class XsltModelExtensions
    {
        public static String GetHtmlFriendlySource(this SourceFragment sourceFragment) => Doc.GetHtmlFriendlySource(sourceFragment.Source);
    }
}
