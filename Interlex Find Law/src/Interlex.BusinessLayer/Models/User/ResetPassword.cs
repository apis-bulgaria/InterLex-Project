namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ResetPassword
    {

        [Display(Name = "Reset Code")]
        [Required]
        public string Code
        {
            get;
            set;
        }

        [Display(Name = "New password")]
        [Required]
        public string Password
        {
            get;
            set;
        }

        [Display(Name = "Repeat password")]
        [Required, Compare("Password", ErrorMessage = "Passwords are not equal")]
        public string Password2
        {
            get;
            set;
        }
    }
}
