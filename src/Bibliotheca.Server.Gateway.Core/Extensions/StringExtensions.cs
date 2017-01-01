using System.Text.RegularExpressions;

namespace Bibliotheca.Server.Gateway.Core.Extensions
{
    public static class StringExtensions
    {
        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string ToAlphanumeric(this string s)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9-]");
            return rgx.Replace(s, string.Empty);
        }
    }
}
