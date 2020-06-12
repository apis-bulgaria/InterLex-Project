using Apis.Common.Classes.Common;
using System;
using System.Linq;

namespace Interlex.App.Helpers
{
    public static class GeneralHelper
    {
        /// <summary>
        /// Determs if the summary text/html is valid. A valid summary is any text containing atleast one letter.
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public static bool IsValidSummaryForDocumentListVisualization(String summary)
        {
            String trimmedSummary = (summary == null ? String.Empty : summary).Trim();
            String taglessSummary = ApisStringHelper.StripTags(trimmedSummary);

            bool isValidSummary = taglessSummary != null;
            isValidSummary &= !String.IsNullOrEmpty(taglessSummary.Trim());
            isValidSummary &= taglessSummary.Any(Char.IsLetter);

            return isValidSummary;
        }
    }
}