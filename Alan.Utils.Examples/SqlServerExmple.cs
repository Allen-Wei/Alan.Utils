using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
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
            var connection = new SqlConnection(cnString);


            var result = connection.ExQuery("select * from roles");
            //var dicts = result.ExGetTable().ExToDicts().ToList();
            var dynamics = result.ExGetTable().ExToDynamics().ToList();
            var models = result.ExGetTable().ExToModels<dynamic>().ToList();

            dynamics = connection.ExQuery<dynamic>("select * from roles").ToList();
        }



        public class Role
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Remark { get; set; }
        }
        public class Depart
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Remark { get; set; }
        }
    }
}
