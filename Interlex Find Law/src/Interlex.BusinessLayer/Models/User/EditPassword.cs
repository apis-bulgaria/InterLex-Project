namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using Interlex.BusinessLayer.CustomValidators;
    using Interlex.DataLayer;

    public class EditPassword
    {

        [Display(Name="New password")]
        [Required]
        public string Password
        {
            get;
            set;
        }

        [Display(Name="Repeat password")]
        [Required]
        public string Password2
        {
            get;
            set;
        }

        public static bool ChangePassword(int userId, string password) 
        {
            bool isSuccesfful = true;
            try
            {
                DB.UserChangePassword(userId, password);
            }
            catch (Exception) 
            {
                isSuccesfful = false;
            }

            if (isSuccesfful)
            {
                return true;                
            }

            return false;
        }
    }
}
