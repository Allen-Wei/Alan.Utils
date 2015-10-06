using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.Utils.ExtensionMethods
{
    public static class ExString
    {


        public static bool ExIsSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool ExIsNotSpace(this string value)
        {
            return !value.ExIsSpace();
        }

        public static string ExIfSpace(this string value, string trueValue)
        {
            return String.IsNullOrWhiteSpace(value) ? trueValue : value;
        }

        public static string ExIfNotSpace(this string value, string trueValue)
        {
            return (!String.IsNullOrWhiteSpace(value)) ? trueValue : value;
        }

        public static string ExPaddingRight(this string str, int length, char padChar)
        {
            return length <= 0 ? str : str.PadRight(length, padChar);
        }

    }
}
