using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interlex.BusinessLayer.Entities;

namespace Interlex.BusinessLayer.Models
{
    public class PracticeByArticleShort
    {
        public string DocumentId
        {
            get;
            set;
        }

        public string ArticleId
        {
            get;
            set;
        }

        public List<DetailedPractice> DetailedPractice
        {
            get;
            set;
        }

        public PracticeByArticleShort()
        {
            DetailedPractice = new List<DetailedPractice>();

            DetailedPractice.Add(new DetailedPractice()
            {
                DetailId = "1",
                DetailName = "1.",
                HasMore = true,
                SearchItems = new List<SearchListItem>()
            });
            DetailedPractice[0].SearchItems.Add(new SearchListItem() { DocumentInfo = new Document { DocLangId = 1, Title = "Document 1" } });
            DetailedPractice[0].SearchItems.Add(new SearchListItem() { DocumentInfo = new Document { DocLangId = 2, Title = "Document 2" } });
            DetailedPractice.Add(new DetailedPractice()
            {
                DetailId = "2",
                DetailName = "2.",
                HasMore = true,
                SearchItems = new List<SearchListItem>()
            });
            DetailedPractice[1].SearchItems.Add(new SearchListItem() { DocumentInfo = new Document { DocLangId = 1, Title = "Document 1" } });
            DetailedPractice[1].SearchItems.Add(new SearchListItem() { DocumentInfo = new Document { DocLangId = 2, Title = "Document 2" } });
        }
    }
}