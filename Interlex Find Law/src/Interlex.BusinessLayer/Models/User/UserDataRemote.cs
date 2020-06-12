namespace Interlex.BusinessLayer.Models
{
    using Newtonsoft.Json;
    using System.Data;
    using Interlex.DataLayer;

    public class UserDataRemote
    {
        public string Username { get; set; }

        public string Products { get; set; }

        public string PasswordHash { get; set; }

        public UserDataRemote(string username, string products, string passwordHash)
        {
            this.Username = username;
            this.Products = products;
            this.PasswordHash = passwordHash;
        }

        public string GetInformationJSON()
        {
            var serializedObject = JsonConvert.SerializeObject(this).Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
            return serializedObject;
        }

        public static UserDataRemote GetUserInformationRemote(string username)
        {
            var dataFromDB = DB.GetUserInformationRemote(username);

            if (dataFromDB == null)
            {
                return null;
            }

            var data = new UserDataRemote(username, dataFromDB["products_string"].ToString(), dataFromDB["password_hash"].ToString());
            return data;
        }
    }
}
