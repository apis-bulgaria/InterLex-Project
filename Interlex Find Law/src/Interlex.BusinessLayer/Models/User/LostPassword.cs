namespace Interlex.BusinessLayer.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using Interlex.DataLayer;
    using System.Collections.Generic;

    public class LostPassword
    {
        //[Display(Name="Username")]
        //public string Username { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public static DataRow GetUser(string email)
        {
            return DB.GetUser(email);
        }

        public static void InsertPasswordReset(int userId, string code) 
        {
            DB.InsertPasswordReset(userId, code);
        }

        public static DataRow GetPasswordReset(string code) 
        {
            return DB.GetPasswordReset(code);
        }

        public static void UpdatePasswordResetExpiry(string code) 
        {
            DB.UpdatePasswordResetExpiry(code);
        }
    }
}
