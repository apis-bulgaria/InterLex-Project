namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LogError
    {
        public string Date { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string LogContent { get; set; }

        public int LogsCount { get; set; }
    }
}
