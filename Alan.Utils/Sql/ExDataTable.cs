using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;


namespace Alan.Utils.Sql
{
    public static class ExDataTable
    {
        public static IEnumerable<T> ExToList<T>(this DataTable table) where T : new()
        {
            var models = new List<T>();

            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            foreach (DataRow row in table.Rows)
            {
                models.Add(row.ExToModel<T>(columns));
            }
            return models;
        }

        /// <summary>
        /// 将
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ExToModel<T>(this DataRow row, string[] columns) where T : new()
        {
            var properties = from property in typeof(T).GetProperties()
                             where property.CanRead && property.CanWrite
                             select property;


            var model = new T();
            foreach (var column in columns)
            {
                var property = properties.FirstOrDefault(p => p.Name.ToLower() == column.ToLower());
                if (property == null) continue;
                var value = row[column];
                property.SetValue(model, value, null);
            }

            return model;
        }
    }
}
