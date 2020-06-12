using AkomaNtosoXml.Xslt.Core.Classes.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.App.Helpers
{
    public static class XsltFragmentExtensions
    {
        public static LanguageFragment FirstWithLangauge(this IEnumerable<LanguageFragment> languageFragments, String langauge)
        {
            return languageFragments.FirstOrDefault(x => x.Language.Value.Equals(langauge, StringComparison.OrdinalIgnoreCase));
        }

    }
}