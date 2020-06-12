using Interlex.BusinessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class SellerData
    {
        public int? SellerId { get; set; }

        public string Code { get; set; }

        public string Username { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }
        public string FullNameEN { get; set; }

        public int UserId { get; set; }

        public UserTypes UserType { get; set; }

        public int? ParentSellerId { get; set; }

        public int? CountryId { get; set; }
        public string CountryName { get; set; }

        public SellerData() 
        {
            // this.Clients = new List<ClientData>();
        }

      //  public List<ClientData> Clients { get; }
    }
}
