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
using Alan.Utils.ExtensionMethods;


namespace Alan.Utils.Examples
{
    public static class SqlServerExmple
    {
        public static void Run()
        {
            var model = new Role();
            var properties = model.GetType().GetProperties().ToList();
            var dbtypes = properties.Select(pro => SqlServerExtentions.GetMatchedSqlType(pro.GetType())).ToList();


            var cnString = ConfigurationSettings.AppSettings["Connection"];
            var connection = new SqlConnection(cnString);

            var sql = @"
select * from Roles where RoleName = @roleName
select * from Departments where DepartmentName = @departName
";

            var tuples = connection.ExQuery<Role, Depart>(sql, new { roleName = "Root", departName = "HR" }.ExToDictionary());
            foreach (var role in tuples.Item1)
            {
                Console.WriteLine(role);
            }
            foreach (var depart in tuples.Item2)
            {
                Console.WriteLine(depart);
            }

            /*
            bool PlaceOrder(int userId, int productId, int count)
            {
                //Old use
                var user = connection.FindOne<User>("select * from Users where UserId = @id", new{ id = userId });
                if(user == null) return false;
                var project = connection.FindOne<Project>("select * from Products where ProductId = @id", new { id = projectId} );
                if(project == null) return false;
                var warehouse = connection.FindOne<Warehouse>("select * from Warehouses where ProjectId = @id" , new { id = projectId });
                if(warehouse.Count < count) return false;

                
                //New use
                var sql = @"
                    select * from Users where UserId = @userId
                    select * from Products where ProductId = @productId
                    select * from Warehouse where ProductId = @productId
                ";
                var items = connection.ExQuery<User, Project, Warehouse>(sql, new { userId, productId });
                if(items.Item1.Count <= 0 || items.Item2.Count <= 0 || items.Item3.Count <=0 || items.Item4.Count <= count) return false;
            }
            */

        }



        public class Role
        {
            public int Id { get; set; }
            public string RoleName { get; set; }
            public string Remark { get; set; }
            public override string ToString()
            {
                return String.Format("Role: {0} {1} {2}", this.Id, this.RoleName, this.Remark);
            }
        }
        public class Depart
        {
            public int Id { get; set; }
            public string DepartmentName { get; set; }
            public string Remark { get; set; }
            public override string ToString()
            {
                return String.Format("Depart: {0} {1} {2}", this.Id, this.DepartmentName, this.Remark);
            }
        }
    }
}
