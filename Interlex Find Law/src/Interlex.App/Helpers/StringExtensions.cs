namespace Interlex.App.Helpers
{
    using System;
    using System.Globalization;

    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this String input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + input.Substring(1, input.Length - 1);
        }

        public static String ToCssClass(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str ?? String.Empty;
            }

            if (str.StartsWith("."))
            {
                return str;
            }
            else
            {
                return "." + str;
            }
        }

        public static String ToCssId(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str ?? String.Empty;
            }

            if (str.StartsWith("#"))
            {
                return str;
            }
            else
            {
                return "#" + str;
            }
        }
    }
}