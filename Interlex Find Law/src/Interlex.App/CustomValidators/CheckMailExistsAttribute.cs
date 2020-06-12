using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;
using Interlex.BusinessLayer;

namespace Interlex.App.CustomValidators
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CheckMailExistsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var email = (String)value;
            bool result = UserMng.ExistsEmail(email);
            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Потребителското име/email е вече използвано";
        }
    }
}