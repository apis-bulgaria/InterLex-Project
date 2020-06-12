namespace Interlex.App.ViewModels
{
    using System.Collections.Generic;
    using Interlex.BusinessLayer.Entities;
    using Interlex.BusinessLayer.Models;

    public class Vat32006L0112
    {
        public Document Document { get; set; }
        public List<DocContentItem> DocContents { get; set; }

        public Vat32006L0112(Document document, List<DocContentItem> docContents)
        {
            this.Document = document;
            this.DocContents = docContents;
            this.SetDeepestArticleClass(this.DocContents);
        }

        private void SetDeepestArticleClass(List<DocContentItem> docContents)
        {
            foreach (var docContentItem in docContents)
            {
                if(docContentItem.children == null || docContentItem.children.Count == 0)
                {
                    docContentItem.extraClasses = "article-for-preview";
                    continue;
                }

                this.SetDeepestArticleClass(docContentItem.children);
            }
        }
    }
}