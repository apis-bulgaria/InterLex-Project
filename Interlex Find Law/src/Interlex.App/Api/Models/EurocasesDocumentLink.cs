using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interlex.BusinessLayer.Models;

namespace Interlex.App.Api.Models
{
    internal class EurocasesDocumentLink : DocumentLink
    {

        private readonly string baseUrl;
        private readonly int docLangId;
        private readonly int documentType;

        internal EurocasesDocumentLink(string title, string originalUrl, string publisher, string baseUrl, int documentType, int docLangId) : base(title, originalUrl, publisher)
        {
            this.baseUrl = baseUrl;
            this.documentType = documentType;
            this.docLangId = docLangId;
        }
        internal static EurocasesDocumentLink FromDocLink(DocLink docLink, String baseUrl)
        {
            return new EurocasesDocumentLink(title: docLink.Title,
                baseUrl: baseUrl,
                docLangId: docLink.DocLangId,
                documentType: docLink.DocType,
                originalUrl: docLink.OriginalLink,
                publisher: docLink.Publisher);
        }

        internal static IReadOnlyCollection<EurocasesDocumentLink> FromDocLinks(IEnumerable<DocLink> docLinks, String baseUrl)
        {
            return docLinks.Select(x => FromDocLink(x, baseUrl)).ToList();
        }

        internal override string GetUrl()
        {
            var docTypeUri = this.documentType == 1 ? "CourtAct" : "LegalAct";

            return $"{this.baseUrl}/Doc/{docTypeUri}/{this.docLangId}";
        }

        internal override bool IsCase()
        {
            return this.documentType == 1;
        }

        internal override bool IsLegislation()
        {
            return this.documentType == 2;
        }
    }
}