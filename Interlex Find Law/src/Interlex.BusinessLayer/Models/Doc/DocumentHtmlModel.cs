using AkomaNtosoXml.Xslt.Core.Classes.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    //[Obsolete]
    //public class DocumentHtmlModel
    //{
    //    public string Title { get; set; }
    //    public string Keywords { get; set; }
    //    public string Summary { get; set; }
    //    public string PressRelease { get; set; }
    //    public string Biblio { get; set; }
    //    public string Text { get; set; }

    //    public static explicit operator DocumentHtmlModel(FragmentsResult frag)
    //    {
    //        DocumentHtmlModel r = new DocumentHtmlModel
    //        {
    //            Title = frag.Title,
    //            Keywords = DocumentHtmlModel.GenerateKeywordsHtml(frag.Keywords),
    //            Summary = DocumentHtmlModel.GenerateSummaryHtml(frag.Summaries),
    //            PressRelease = DocumentHtmlModel.GeneratePressRelHtml(frag.PressRelease),
    //            Text = DocumentHtmlModel.GenerateTextHtml(frag.DocumentContent)
    //        };

    //        return r;
    //    }

    //    private static string GenerateTextHtml(Fragment<FragmentParts> fragment)
    //    {
    //        if (fragment == null)
    //        {
    //            return null;
    //        }

    //        StringBuilder res = new StringBuilder();

    //        // Content
    //        StringBuilder ci = new StringBuilder();
    //        foreach (var contImt in fragment.Content)
    //        {
    //            ci.Append(contImt);
    //        }
    //        res.Append(HtmlTagsBuilder.GetHtml(ci.ToString(), Tags.div, "content"));

    //        // Rubrics
    //        StringBuilder r = new StringBuilder();
    //        foreach (var rubric in fragment.Rubric)
    //        {
    //            r.Append(rubric);
    //        }
    //        res.Append(HtmlTagsBuilder.GetHtml(r.ToString(), Tags.div, "rubric"));

    //        return res.ToString();
    //    }

    //    public static string GeneratePressRelHtml(Fragment<FragmentParts> fragment)
    //    {
    //        if (fragment == null)
    //        {
    //            return null;
    //        }

    //        StringBuilder res = new StringBuilder();

    //        return res.ToString();
    //    }

    //    public static string GenerateSummaryHtml(Fragment<System.Collections.Generic.List<SourceFragment>> summaryfragment)
    //    {
    //        if (summaryfragment == null)
    //        {
    //            return null;
    //        }

    //        StringBuilder res = new StringBuilder();

    //        List<SourceFragment> cont = summaryfragment.Content.OrderBy(x => DocumentHtmlModel.GetSourceOrderByText(x.Source)).ToList();
    //        for (int i = 0; i < cont.Count; i++)
    //        {
    //            StringBuilder sourceFragment = new StringBuilder();
    //            // Non translated source fragments
    //            foreach (var nt in cont[i].NonTranslated)
    //            {
    //                res.Append(HtmlTagsBuilder.GetHtml(nt, Tags.div, "non-tanslated"));
    //            }

    //            // Translated source fragments
    //            foreach (var t in cont[i].Translated)
    //            {

    //            }

    //            res.Append(HtmlTagsBuilder.GetHtml(summaryfragment.Rubric, Tags.div));
    //        }

    //        res.Append(HtmlTagsBuilder.GetHtml(summaryfragment.Rubric, Tags.div));

    //        string resSumm = HtmlTagsBuilder.GetHtml(res.ToString(), Tags.summary);
    //        return resSumm;
    //    }

    //    public static string GenerateKeywordsHtml(Fragment<System.Collections.Generic.List<SourceFragment>> keywordsfragment)
    //    {
    //        if (keywordsfragment == null)
    //        {
    //            return null;
    //        }

    //        StringBuilder res = new StringBuilder();

    //        return res.ToString();
    //    }

    //    // TODO - Implement source ordering
    //    public static int GetSourceOrderByText(string summarySource)
    //    {
    //        return 1;
    //    }
    //}

    //public enum Tags
    //{
    //    header,
    //    title,
    //    summary,
    //    strong,
    //    div
    //}

    //public static class HtmlTagsBuilder
    //{
    //    public static string GetHtml(string txt, Tags tag, string className = "")
    //    {
    //        if (className != "")
    //        {
    //            className = " class='" + className + "'";
    //        }

    //        return String.Format("<{0}{2}>\r\n\t{1}\r\n</{0}>", tag.ToString(), txt, className);
    //    }
    //}
}
