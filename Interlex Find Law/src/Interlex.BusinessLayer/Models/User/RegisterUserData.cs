using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Interlex.BusinessLayer.CustomValidators;

namespace Interlex.BusinessLayer.Models
{
    public class RegisterUserData
    {
        [Required]
        [EmailAddress]
        [CheckMailExists]
        public string Mail
        {
            get;
            set;
        }

        [Required]
        public string Password
        {
            get;
            set;
        }

        [Required, Compare("Password", ErrorMessage = "Паролите не съвпадат")]


        public string Password2
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        //public string Fullname
        //{
        //    get;
        //    set;
        //}
    }
}