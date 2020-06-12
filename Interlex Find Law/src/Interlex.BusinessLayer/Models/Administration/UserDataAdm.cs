using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class UserDataAdm : UserData
    {
        public string ConfirmPassword { get; set; }

        private List<object> _userTypes;
        public List<object> UserTypes 
        {
            get
            {
                if (_userTypes == null)
                    _userTypes = UserMng.GetUserTypes();
                    return _userTypes;
            }
        }

        private List<object> _clients;
        public List<object> Clients
        {
            get
            {
                var sellerId = this.SellerId;
                if (sellerId == null)
                {
                    sellerId = -1;
                }
                if (_clients == null)
                    _clients = UserMng.GetClients(-1);
                return _clients;
            }
        }

        public UserDataAdm()
        {
        }

        public UserDataAdm(UserData ud)
            : base(ud)
        {
        }
    }
}
