using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interlex.BusinessLayer.Enums;

namespace Interlex.BusinessLayer.Models
{
    public class DocumentData
    {
        public string DocumentId
        {
            get;
            set;
        }

        public DocType DocumentType
        {
            get;
            set;
        }

        public string SearchString
        {
            get;
            set;
        }

        public int FoundOccurences
        {
            get;
            set;
        }
    }
}