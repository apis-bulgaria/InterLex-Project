namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LogSearchError
    {
        public string Search { get; set; }

        public string Username { get; set; }

        public string ExceptionMessage { get; set; }

        public string InnerExceptionMessage { get; set; }
    }
}
