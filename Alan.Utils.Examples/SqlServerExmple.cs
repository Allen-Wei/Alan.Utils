﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alan.Utils.Sql;


namespace Alan.Utils.Examples
{
    public static class SqlServerExmple
    {
        public static void Run()
        {
            var cnString = ConfigurationSettings.AppSettings["Connection"];
            SqlConnection connection = new SqlConnection(cnString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"
select * from Roles
";
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            adapter.Dispose();
            command.Dispose();
            if (connection.State == ConnectionState.Open) connection.Close();

            var roles = table.ExToList<Role>();
        }



        public class Role
        {
            public int KeyId { get; set; }
            public string Name { get; set; }
            public string Remark { get; set; }
        }
        public class Depart
        {
            public int KeyId { get; set; }
            public string Name { get; set; }
            public string Remark { get; set; }
        }
    }
}
