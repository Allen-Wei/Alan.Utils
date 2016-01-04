using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Alan.Utils.ExtensionMethods;


namespace Alan.Utils.Sql
{
    public static class SqlServerExtentions
    {

        /// <summary>
        /// 获取 SqlConnection 对象
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="callback"></param>
        public static void ExGetConnection(this string connection, Action<SqlConnection> callback)
        {
            using (SqlConnection cn = new SqlConnection(connection))
            {
                callback(cn);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL查询使用的参数</param>
        /// <returns></returns>
        public static DataSet ExQuery(this SqlConnection connection, string sql, Dictionary<string, object> parameters)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> dict in parameters)
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

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="connection">SQLConnection 连接对象</param>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static DataSet ExQuery(this SqlConnection connection, string sql)
        {
            return connection.ExQuery(sql, new Dictionary<string, object>());

        }

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="connection">SqlConnection 连接对象</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL查询参数</param>
        /// <returns></returns>
        public static DataSet ExQuery(this SqlConnection connection, string sql, object parameters)
        {
            if (parameters == null) return connection.ExQuery(sql, new Dictionary<string, object>());
            return connection.ExQuery(sql, parameters.ExToDictionary());

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
                , tables[4].ExToModels<T5>()
                );

        }


        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> ExQuery<T1, T2, T3, T4, T5, T6>(this SqlConnection connection, string sql, object parameters = null)
   where T1 : new()
   where T2 : new()
   where T3 : new()
   where T4 : new()
   where T5 : new()
   where T6 : new()
        {
            var tables = connection.ExQuery(sql, parameters).Tables.Cast<DataTable>().ToList();
            if (tables.Count < 6) throw new Exception("返回结果集数量不能小于6");

            return Tuple.Create(
                tables[0].ExToModels<T1>()
                , tables[1].ExToModels<T2>()
                , tables[2].ExToModels<T3>()
                , tables[3].ExToModels<T4>()
                , tables[4].ExToModels<T5>()
                , tables[5].ExToModels<T6>()
                );

        }


        #endregion


        public static IEnumerable<T> ExQuery<T>(this SqlConnection connection, string sql, int take, int skip)
            where T : new()
        {
            StringBuilder pagingSql = new StringBuilder();
            pagingSql.Append("declare @_temp table (_Id bigint identity(1,1) not null ");
            var names = Alan.Utils.ExtensionMethods.ExReflection.ExGetPropNames<T>();

            return new List<T>();
        }

        public static string GetMatchedSqlType(Type propertyType)
        {
            var matched = SqlTypeMap.FirstOrDefault(dict => dict.Key == propertyType);
            return matched.Value;
        }
        private static Dictionary<Type, string> SqlTypeMap
        {
            get
            {
                return new Dictionary<Type, string>()
                {
                    { typeof(int), "int" }
                    , { typeof(long), "long" }
                    , { typeof(byte), "byte" }
                    , { typeof(short), "smallint" }
                    , { typeof(DateTime), "datetime" }
                    , { typeof(string), "varchar(max)" }
                    , { typeof(decimal), "money" }
                    , { typeof(double), "float" }
                    , { typeof(float), "float" }
                    , { typeof(Guid), "uniqueidentityfier" }
                };
            }
        }
    }
}
