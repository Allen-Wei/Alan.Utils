using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.Utils.ExtensionMethods
{
    /// <summary>
    /// 字符串相关的扩展方法
    /// </summary>
    public static class ExString
    {


        /// <summary>
        /// 是否是空字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ExIsSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 是否不是空字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ExIsNotSpace(this string value)
        {
            return !value.ExIsSpace();
        }

        public static string ExIfSpace(this string value, string whenSpace)
        {
            return String.IsNullOrWhiteSpace(value) ? whenSpace : value;
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
