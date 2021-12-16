using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AMS.Web.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            string str = text;
            if (maxLength > 0)
            {
                int length = maxLength - suffix.Length;
                if (length <= 0)
                {
                    return str;
                }
                if ((text != null) && (text.Length > maxLength))
                {
                    int nextSpace = text.LastIndexOf(" ", length);
                    return (text.Substring(0, (nextSpace > 0) ? nextSpace : length).TrimEnd(new char[0]) + suffix);
                }
            }
            return str;
        }

        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}