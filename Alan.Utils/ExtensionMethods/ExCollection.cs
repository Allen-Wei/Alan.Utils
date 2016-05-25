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


        #region 数据分组

        /// <summary>
        /// 将数据按每组 countPerGroup 个进行分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据</param>
        /// <param name="countPerGroup">每组数量</param>
        /// <returns></returns>
        public static List<IEnumerable<T>> ExGroup<T>(this IEnumerable<T> source, int countPerGroup)
        {
            List<IEnumerable<T>> groups = new List<IEnumerable<T>>();
            if (!source.Any()) return groups;

            var groupsCount = Math.Ceiling((double)source.Count() / countPerGroup);

            for (var index = 0; index < groupsCount; index++)
            {
                var group = source.Skip(index * countPerGroup).Take(countPerGroup);
                groups.Add(group);
            }
            return groups;
        }

        /// <summary>
        /// 将数据分割成 groupsCount 组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据</param>
        /// <param name="groupsCount">分割成多少组</param>
        /// <returns></returns>
        public static List<IEnumerable<T>> ExSplit<T>(this IEnumerable<T> source, int groupsCount)
        {
            List<IEnumerable<T>> groups = new List<IEnumerable<T>>();
            if (!source.Any()) return groups.ToList();

            var sourceCount = source.Count();
            if (sourceCount <= groupsCount)
            {
                groups.Add(source);
                return groups;
            }

            int countPerGrouop = (int)Math.Ceiling((double)sourceCount / groupsCount);

            for (int index = 0; index < groupsCount; index++)
            {
                var group = source.Skip(index * countPerGrouop).Take(countPerGrouop);
                groups.Add(group);
            }

            return groups;
        }


        /// <summary>
        /// 将数据分割成指定部分
        /// via http://stackoverflow.com/questions/438188/split-a-collection-into-n-parts-with-linq 
        /// 这个方法会打乱原始数据的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ExSplitUnOrder<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                         group item by i++ % parts into part
                         select part;
            return splits;
        }

        #endregion

    }
}
