using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interlex.DataLayer;

namespace Interlex.BusinessLayer.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int LicenseCnt { get; set; }
        /// <summary>
        /// Used for model binding
        /// </summary>
        public bool Selected { get; set; }

        public bool? IsActive { get; set; }

        public bool Demo { get; set; }
    }
}
