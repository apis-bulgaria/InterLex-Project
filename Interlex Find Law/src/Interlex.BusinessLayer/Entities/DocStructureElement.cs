using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Entities
{
    public class DocStructureElement
    {
        public string title
        {
            get;
            set;
        }

        public string id
        {
            get;
            set;
        }

        public string key
        {
            get;
            set;
        }


        public bool folder
        {
            get;
            set;
        }


        public bool lazy
        {
            get;
            set;
        }

        public DocStructureElement[] children
        {
            get;
            set;
        }

    }
}