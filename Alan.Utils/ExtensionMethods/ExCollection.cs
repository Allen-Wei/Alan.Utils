using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.Utils.ExtensionMethods
{
    public static class ExCollection
    {

        /// <summary>
        /// 集合遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ExForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
        /// <summary>
        /// 集合遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ExForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (T element in source)
                action(element, index++);
        }


    }
}
