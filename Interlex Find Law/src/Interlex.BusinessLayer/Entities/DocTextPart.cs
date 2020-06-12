using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interlex.BusinessLayer.Enums;

namespace Interlex.BusinessLayer.Entities
{
    public class DocTextPart
    {
        public string DocTextPartId
        {
            get;
            set;
        }

        public bool HasPractice
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Html
        {
            get;
            set;
        }

        public DocPartTypes PartType
        {
            get;
            set;
        }



    }
}