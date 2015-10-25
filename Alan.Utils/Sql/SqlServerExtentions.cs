using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Alan.Utils.ExtensionMethods;


namespace Alan.Utils.Sql
{
    public static class SqlServerExtentions
    {


        public static void ExGetConnection(this string connection, Action<SqlConnection> callback)
        {
            using (SqlConnection cn = new SqlConnection(connection))
            {
                callback(cn);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static DataSet ExQuery(this SqlConnection connection, string sql, object parameters = null)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    var dicts = parameters.ExToDictionary();
                    foreach (var dict in dicts)
                    {
                        command.Parameters.AddWithValue(dict.Key, dict.Value);
                    }
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }

        }

        #region Tuple Query
        public static IEnumerable<T> ExQuery<T>(this SqlConnection connection, string sql, object parameters = null)
            where T : new()
        {
            var tables = connection.ExQuery(sql, parameters).Tables.Cast<DataTable>().ToList();
            if (tables.Count < 1) throw new Exception("返回结果集数量不能小于2");
            return tables[0].ExToModels<T>();
        }

        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> ExQuery<T1, T2>(this SqlConnection connection, string sql, object parameters = null)
            where T1 : new()
            where T2 : new()
        {
            var tables = connection.ExQuery(sql, parameters).Tables.Cast<DataTable>().ToList();
            if (tables.Count < 2) throw new Exception("返回结果集数量不能小于2");

            return Tuple.Create(tables[0].ExToModels<T1>(), tables[1].ExToModels<T2>());

        }

        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> ExQuery<T1, T2, T3>(this SqlConnection connection, string sql, object parameters = null)
    where T1 : new()
    where T2 : new()
    where T3 : new()
        {
            var tables = connection.ExQuery(sql, parameters).Tables.Cast<DataTable>().ToList();
            if (tables.Count < 3) throw new Exception("返回结果集数量不能小于2");

            return Tuple.Create(tables[0].ExToModels<T1>(), tables[1].ExToModels<T2>(), tables[2].ExToModels<T3>());

        }


        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> ExQuery<T1, T2, T3, T4>(this SqlConnection connection, string sql, object parameters = null)
    where T1 : new()
    where T2 : new()
    where T3 : new()
    where T4 : new()
        {
            var tables = connection.ExQuery(sql, parameters).Tables.Cast<DataTable>().ToList();
            if (tables.Count < 4) throw new Exception("返回结果集数量不能小于4");

            return Tuple.Create(
                tables[0].ExToModels<T1>()
                , tables[1].ExToModels<T2>()
                , tables[2].ExToModels<T3>()
                , tables[3].ExToModels<T4>()
                );

        }

        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> ExQuery<T1, T2, T3, T4, T5>(this SqlConnection connection, string sql, object parameters = null)
   where T1 : new()
   where T2 : new()
   where T3 : new()
   where T4 : new()
   where T5 : new()
        {
            var tables = connection.ExQuery(sql, parameters).Tables.Cast<DataTable>().ToList();
            if (tables.Count < 5) throw new Exception("返回结果集数量不能小于5");

            return Tuple.Create(
                tables[0].ExToModels<T1>()
                , tables[1].ExToModels<T2>()
                , tables[2].ExToModels<T3>()
                , tables[3].ExToModels<T4>()
                , tables[3].ExToModels<T5>()
                );

        }
        #endregion



        #region DataRow/DataTable To Model/Dictionary/Dynamic

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
                    var property = properties.FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
                    if (property == null) return;
                    property.SetValue(model, row[key], null);
                };

            columns.ExForEach(SetValue);

            return model;
        }

        public static IEnumerable<T> ExToModels<T>(this DataTable table) where T : new()
        {
            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();

            return table.Rows.Cast<DataRow>().Select(row => row.ExToModel<T>(columns));
        }



        public static IDictionary<string, object> ExToDict(this DataRow row, IEnumerable<string> columns)
        {
            var model = new Dictionary<string, object>();
            columns.Distinct().ExForEach(col => model.Add(col, row[col]));
            return model;
        }

        public static IEnumerable<IDictionary<string, object>> ExToDicts(this DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();

            return table.Rows.Cast<DataRow>().Select(row => row.ExToDict(columns));
        }



        public static dynamic ExToDynamic(this DataRow row, IEnumerable<string> columns)
        {
            IDictionary<string, object> result = new System.Dynamic.ExpandoObject();
            columns.ExForEach(col => result.Add(col, row[col]));
            return result;
        }


        public static IEnumerable<dynamic> ExToDynamics(this DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            return table.Rows.Cast<DataRow>().Select(row => row.ExToDynamic(columns));
        }
        #endregion


        #region To DataTable

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


        public static DataTable ExGetTable(this DataSet set)
        {
            if (set == null || set.Tables.Count < 1) return null;
            return set.Tables[0];
        }

        public static DataTable ExGetTable(this DataSet set, string table)
        {
            if (set == null) return null;
            return set.Tables[table];

        }

    }
}
