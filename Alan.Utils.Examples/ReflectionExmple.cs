using Alan.Utils.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.Utils.Examples
{
    public static class ReflectionExmple
    {

        public static void Run()
        {
            var student = Student.Samples().First();
            Console.WriteLine("Student:{0}", student.ExToJson());
            var worker = Worker.Samples().First();
            Console.WriteLine("Worker:{0}", worker.ExToJson());
            student.ExOverrideInclude(worker, stu => new { stu.Name, stu.Klass, stu.Birthday });
            Console.WriteLine("Override Student:{0}", student.ExToJson());

            student = Student.Samples().Skip(1).First();
            Console.WriteLine("Student:{0}", student.ExToJson());
            worker = Worker.Samples().Skip(1).First();
            Console.WriteLine("Worker:{0}", worker.ExToJson());
            student.ExOverrideExclude(worker, stu => new { stu.Name, stu.Klass, stu.Birthday });
            student.ExSetPropValue("Name", "Thinking.");
            Console.WriteLine("Override Student:{0}", student.ExToJson());


            var names = ExReflection.CollectNames<Student>(stu => new { stu.Klass, stu.Name, stu.Birthday, stu.Gender });



            var values = Student.Samples().FirstOrDefault().ExGetPropValues(0).ToList();
        }

        class People
        {
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
            public bool Gender { get; set; }
            public int Age { get { return DateTime.Now.Year - this.Birthday.Year; } }
        }
        class Student : People
        {
            public string Klass { get; set; }

            public static IEnumerable<Student> Samples()
            {
                return new List<Student>() {
            new Student() { Name = "Alan", Birthday = DateTime.Now.AddYears(-18), Gender = true, Klass = "大一" }
            ,new Student() { Name = "Annr", Birthday = DateTime.Now.AddYears(-21), Gender = false, Klass = "大三" }
            ,new Student() { Name = "Michael", Birthday = DateTime.Now.AddYears(-16), Gender = true, Klass = "高一" }
            };
            }
        }
        class Worker : People
        {
            public decimal Salary { get; set; }


            public static IEnumerable<Worker> Samples()
            {
                return new List<Worker>() {
            new Worker() { Name = "Jobs", Birthday = DateTime.Now.AddYears(-45), Gender = true, Salary= 3000 }
            ,new Worker() { Name = "Bill", Birthday = DateTime.Now.AddYears(-46), Gender = true, Salary = 2900}
            ,new Worker() { Name = "Tom", Birthday = DateTime.Now.AddYears(-50), Gender = false, Salary = 6000 }
            };
            }
        }

    }
}
