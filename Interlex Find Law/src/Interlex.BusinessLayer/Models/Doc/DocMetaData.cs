using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interlex.BusinessLayer.Models
{
    public class DocMetadata
    {
        
        
        public string TypeOfLegislation
        {
            get;
            set;
        }

        public string TerritorialApplication
        {
            get;
            set;
        }

        public string Agent
        {
            get;
            set;
        }

        public string Subagent
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public DateTime DocumentDate
        {
            get;
            set;
        }

        public DateTime PublicationDate
        {
            get;
            set;
        }
        public DateTime ForceDate
        {
            get;
            set;
        }

        public DateTime NoForceDate
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string RelatedTo
        {
            get;
            set;
        }

        public string ChangedBy
        {
            get;
            set;
        }

        public string BasisFor
        {
            get;
            set;
        }

        public string BasedOn
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string ShortTitle 
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public string PublicationReference
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Publisher
        {
            get;
            set;
        }
        

    }
}