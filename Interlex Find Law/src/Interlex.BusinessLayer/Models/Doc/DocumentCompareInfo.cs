using System;
using System.Collections.Generic;
using System.Linq;
using DiffPlex;
using DiffPlex.DiffBuilder;
using System.Diagnostics;
using DiffMatchPatch;

namespace Interlex.BusinessLayer.Models
{
    public class DocumentCompareInfo
    {
        private readonly IInlineDiffBuilder diffBuilder;
        private readonly ISideBySideDiffBuilder sideDiffBuilder;
        private string _originalHtml;
        private string _oldText;
        private string _newText;

        private int _diffCount;
        public int DifferencesCount
        {
            get
            {
                return this._diffCount;
            }
        }

        private string _html;
        public string Html
        {
            get
            {
                return this._html;
            }
        }

        public DocumentCompareInfo(string oldText, string newText, string originalHtml)
        {
            this._oldText = oldText;
            this._newText = newText;
            this._originalHtml = originalHtml;
            this.diffBuilder = new InlineDiffBuilder(new Differ());
            this.sideDiffBuilder = new SideBySideDiffBuilder(new Differ());
        }

        //public void RefreshDiffCount()
        //{
        //    string pattern = @"<diff ";
        //    Regex rgx = new Regex(pattern);
        //    this._diffCount = rgx.Matches(this.Html).Count;
        //}

        public struct DocDiff
        {
            public int startpos;
            public int length;
            public string type;
        }

        public void Compare_temp()
        {
            List<Diff> diffs = new List<Diff>();

            var diffList1 = new List<DocDiff>();
            var diffList2 = new List<DocDiff>();

            diff_match_patch DIFF = new diff_match_patch();

            diffs = DIFF.diff_main(this._oldText, this._newText);
            DIFF.diff_cleanupSemanticLossless(diffs);      // <--- see note !

            //     diffList1 = collectDiffs(this._oldText, this._newText, diffs);

            var finalText = collectDiffs(this._oldText, this._newText, diffs);
            this._html = finalText;

            //    diffList2 = collectDiffs(RTB2);

            //     paintChunks(RTB1, chunklist1);
            //  paintChunks(RTB2, chunklist2);
        }

        private string collectDiffs(string oldText, string newText, List<Diff> diffs)
        {
            oldText = string.Empty;
            //  RTB.Text = "";
            List<DocDiff> chunkList = new List<DocDiff>();
            foreach (Diff d in diffs)
            {
                // if (RTB == RTB2 && d.operation == DiffMatchPatch.Operation.DELETE) continue;  // **
                //  if (RTB == RTB1 && d.operation == DiffMatchPatch.Operation.INSERT) continue;  // **

                if (oldText == newText && d.operation == DiffMatchPatch.Operation.DELETE)
                {
                    continue;
                }
                if (oldText == newText && d.operation == DiffMatchPatch.Operation.INSERT)
                {
                    continue;
                }

                DocDiff ch = new DocDiff();
                int length = oldText.Length;

                ch.startpos = length;
                ch.length = d.text.Length;
                if (d.operation == DiffMatchPatch.Operation.INSERT)
                {
                    oldText = oldText + "<span class=\"ins diff\">" + d.text + "</span>";
                    ch.type = "ins";
                }
                else if (d.operation == DiffMatchPatch.Operation.DELETE)
                {
                    oldText = oldText + "<span class=\"del diff\">" + d.text + "</span>";
                    ch.type = "del";
                }
                else
                {
                    oldText = oldText + d.text;
                }

                //   ch.type = oldText == newText ? colors1[(int)d.operation] : colors2[(int)d.operation];
                chunkList.Add(ch);
            }

            return oldText;
        }

        public void Compare()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var compareModel = diffBuilder.BuildDiffModel(this._oldText, this._newText);
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            /*   for (int i = 0; i < compareModel.NewText.Lines.Count; i++)
               {
                   if (compareModel.NewText.Lines[i].Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted)
                   {
                       compareModel.NewText.Lines[i].Text = "<span class=\"del diff\">" + compareModel.NewText.Lines[i].Text + "</span>";
                   }
                   else if (compareModel.NewText.Lines[i].Type == DiffPlex.DiffBuilder.Model.ChangeType.Inserted)
                   {
                       compareModel.NewText.Lines[i].Text = "<span class=\"ins diff\">" + compareModel.NewText.Lines[i].Text + "</span>";
                   }
                   else if (compareModel.NewText.Lines[i].Type == DiffPlex.DiffBuilder.Model.ChangeType.Modified)
                   {
                       compareModel.NewText.Lines[i].Text = "<span class=\"mod diff\">" + compareModel.NewText.Lines[i].Text + "</span>";
                   }
               }*/

            var htmlTextSplitted = this._originalHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = compareModel.Lines.Count - 1; i >= 0; i--)
            {
                try
                {
                    var currentLine = compareModel.Lines[i];
                    if (currentLine.Type == DiffPlex.DiffBuilder.Model.ChangeType.Inserted)
                    {
                        htmlTextSplitted[i] = "<span class=\"ins diff\">" + htmlTextSplitted[i] + "</span>";
                    }
                    else if (currentLine.Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted)
                    {
                        htmlTextSplitted[i] = "<span class=\"del diff\">" + htmlTextSplitted[i] + "</span>";
                    }
                    else if (currentLine.Type == DiffPlex.DiffBuilder.Model.ChangeType.Modified)
                    {
                        htmlTextSplitted[i] = "<span class=\"mod diff\">" + htmlTextSplitted[i] + "</span>";
                    }
                }
                catch (Exception) 
                {
                    continue;
                }
            }

            // var changedCompareLines = compareModel.Lines.Where(l => l.Type != DiffPlex.DiffBuilder.Model.ChangeType.Unchanged).Select(l.);
            /* foreach (var line in changedCompareLines)
             {
                 if (line.Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted)
                 {
                     line.Text = "<span class=\"del\">" + line.Text + "</span>";
                 }
                 else if (line.Type == DiffPlex.DiffBuilder.Model.ChangeType.Inserted)
                 {
                     line.Text = "<span class=\"ins\">" + line.Text + "</span>";  
                 }
             }*/

            //  this._html = String.Join("", compareModel.Lines.Select(l => l.Text));
            //  this._html = String.Join("", compareModel.NewText.Lines.Select(l => l.Text));
            this._html = String.Join(Environment.NewLine, htmlTextSplitted);
              this._diffCount = compareModel.Lines.Where(l => l.Type != DiffPlex.DiffBuilder.Model.ChangeType.Unchanged && l.Type != DiffPlex.DiffBuilder.Model.ChangeType.Imaginary).Count();

            /* HtmlDiff.HtmlDiff diffHelper = new HtmlDiff.HtmlDiff(this._oldXml, this._newXml);
             //this._html = diffHelper.Build();

             this._html = Regex.Replace(diffHelper.Build(), @"<diff class='ins'>(\s*)</diff>", "");

             string pattern = @"<diff ";
             Regex rgx = new Regex(pattern);
             this._diffCount = rgx.Matches(this.Html).Count;*/
        }
    }
}