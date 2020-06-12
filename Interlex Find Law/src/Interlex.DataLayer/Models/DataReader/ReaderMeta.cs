using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EuFins.DataReader
{
    public class ReaderMeta
    {
        public ReaderMeta()
        {
            this.MapPropertyToPgreColumn = new List<PropMeta>();
        }

        public string PostgreSqlQuery { get; set; }

        public List<PropMeta> MapPropertyToPgreColumn { get; set; }
    }

    public class PropMeta
    {
        public string PropName { get; set; }

        public string PropType { get; set; }

        public string PrgeColumn { get; set; }
    }
}