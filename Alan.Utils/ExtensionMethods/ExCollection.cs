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

        public static DataSet ExDynamicToDataSet(this IEnumerable<dynamic> collection, string tableName = "_default")
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add(tableName);

            foreach (var entity in collection)
            {
                var row = table.NewRow();

                foreach (var dict in entity)
                {
                    var key = dict.Key;
                    var value = dict.Value;

                    if (!table.Columns.Contains(key))
                    {
                        table.Columns.Add(key);
                    }

                    row[key] = value;

                }
                table.Rows.Add(row);
            }

            return dataSet;
        }

        public static DataSet ExEntitiesToDataSet<T>(this IEnumerable<T> collection, string tableName = "_default") where T : class
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add(tableName);

            var properties = typeof(T).GetProperties().ToList();

            foreach (var entity in collection)
            {
                var row = table.NewRow();

                properties.ForEach(prop =>
                {
                    var key = prop.Name;
                    var value = prop.GetValue(entity, null);

                    if (!table.Columns.Contains(key)) table.Columns.Add(key);

                    row[key] = value;
                });

                table.Rows.Add(row);
            }

            return dataSet;
        }

        public static DataSet ExDictToDataSet(this IEnumerable<Dictionary<string, object>> collection, string tableName = "_default")
        {
            var dataSet = new DataSet();
            var table = dataSet.Tables.Add(tableName);
            collection.ExForEach(dicts =>
            {
                var row = table.NewRow();
                dicts.ExForEach(dict =>
                {
                    var key = dict.Key;
                    var value = dict.Value;
                    if (!table.Columns.Contains(key)) table.Columns.Add(key);
                    row[key] = value;
                });
                table.Rows.Add(row);
            });

            return dataSet;
        }


        public static IEnumerable<dynamic> ExTableToDynamic(this DataTable table)
        {
            var result = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                dynamic rowDynamic = new System.Dynamic.ExpandoObject();
                foreach (DataColumn column in table.Columns)
                {
                    rowDynamic[column.ColumnName] = row[column];
                }
                result.Add(rowDynamic);
            }
            return result;
        }

        public static IEnumerable<Dictionary<string, object>> ExTableToDict(this DataTable table)
        {
            var dicts = new List<Dictionary<string, object>>();
            foreach(DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach(DataColumn column in table.Columns)
                {
                    dict[column.ColumnName] = row[column];
                }
                dicts.Add(dict);
            }
            return dicts;
        }


    }
}
