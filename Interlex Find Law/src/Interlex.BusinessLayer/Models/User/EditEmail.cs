namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using Interlex.BusinessLayer.CustomValidators;
    using Interlex.DataLayer;

    public class EditEmail
    {

        [Display(Name = "New email")]
        [Required]
        public string Email
        {
            get;
            set;
        }

        [Display(Name = "Repeat email")]
        [Required]
        public string Email2
        {
            get;
            set;
        }

        [Display(Name = "Password")]
        [Required]
        public string Password
        {
            get;
            set;
        }

        public static bool ChangeEmail(int userId, string email, string password)
        {
            return DB.UserChangeEmail(userId, email, password);
        }
    }
}
