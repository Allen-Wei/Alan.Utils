using Alan.Utils.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Alan.Utils.Sql
{
    /// <summary>
    /// DataTable/DataSet相关扩展方法
    /// </summary>
    public static class DataTableSetExtensions
    {

        #region DataRow/DataTable To Model/Dictionary/Dynamic

        /// <summary>
        /// 将DataRow转换成实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static T ExToModel<T>(this DataRow row, IEnumerable<string> columns) where T : new()
        {
            var tType = typeof(T);
            var properties = (from property in tType.GetProperties()
                              where property.CanRead && property.CanWrite
                              select property).ToList();


            T model = new T();
            IDictionary<string, object> dict = model as IDictionary<string, object>;

            Action<string> SetValue;

            if (dict != null)
                SetValue = key => dict.Add(key, row[key]);
            else
                SetValue = key =>
                {
                    //fix can't convert System.DbNull
                    var property = properties.FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
                    if (property == null) return;
                    var value = row[key];
                    if (value == DBNull.Value) return;
                    property.SetValue(model, value, null);
                };

            columns.ExForEach(SetValue);

            return model;
        }

        /// <summary>
        /// 将DataTable转换成实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExToModels<T>(this DataTable table) where T : new()
        {
            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();

            return table.Rows.Cast<DataRow>().Select(row => row.ExToModel<T>(columns));
        }


        /// <summary>
        /// 将DataRow转换成字典
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ExToDict(this DataRow row, IEnumerable<string> columns)
        {
            var model = new Dictionary<string, object>();
            columns.Distinct().ExForEach(col => model.Add(col, row[col]));
            return model;
        }

        /// <summary>
        /// 将DataTable转换成字典集合
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ExToDicts(this DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();

            return table.Rows.Cast<DataRow>().Select(row => row.ExToDict(columns)).ToList();
        }


        /// <summary>
        /// 将DataRow转换成dynamic
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static dynamic ExToDynamic(this DataRow row, IEnumerable<string> columns)
        {
            IDictionary<string, object> result = new System.Dynamic.ExpandoObject();
            columns.ExForEach(col => result.Add(col, row[col]));
            return result;
        }

        /// <summary>
        /// 将DataTable转换成dynamic集合
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ExToDynamics(this DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            return table.Rows.Cast<DataRow>().Select(row => row.ExToDynamic(columns));
        }
        #endregion


        #region Collection To DataTable

        /// <summary>
        /// 将dynamic集合转换成DataTable
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ExDynamicsToTable(this IEnumerable<dynamic> collection)
        {
            DataTable table = new DataTable();

            foreach (var entity in collection)
            {
                var row = table.NewRow();

                foreach (var dict in entity)
                {
                    var key = dict.Key;
                    var value = dict.Value;

                    if (!table.Columns.Contains(key)) table.Columns.Add(key);

                    row[key] = value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// 将实体集合转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ExModelsToTable<T>(this IEnumerable<T> collection) where T : class
        {
            DataTable table = new DataTable();

            var properties = typeof(T).GetProperties().Where(property => property.CanRead).ToList();

            foreach (T entity in collection)
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

            return table;
        }

        /// <summary>
        /// 将字典集合转换成DataTable
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ExDictsToTable(this IEnumerable<Dictionary<string, object>> collection)
        {
            var table = new DataTable();
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

            return table;
        }

        #endregion


        /// <summary>
        /// 获取DataSet所有的DataTable
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static IEnumerable<DataTable> ExGetTables(this DataSet set)
        {
            if (set == null) return null;
            return set.Tables.Cast<DataTable>();
        }

        /// <summary>
        /// 获取DataSet的第一个DataTable
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static DataTable ExGetTable(this DataSet set)
        {
            if (set == null || set.Tables.Count < 1) return null;
            return set.Tables[0];
        }

        /// <summary>
        /// 获取DataSet的指定DataTable
        /// </summary>
        /// <param name="set"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable ExGetTable(this DataSet set, string table)
        {
            if (set == null) return null;
            return set.Tables[table];

        }

        /// <summary>
        /// 将DataTable包裹成DataSet
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataSet ExToDataSet(this DataTable table)
        {
            if (table == null) return null;
            var set = new DataSet();
            set.Tables.Add(table);
            return set;
        }

    }
}
