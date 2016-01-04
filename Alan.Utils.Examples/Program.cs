using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alan.Utils.ExtensionMethods;

namespace Alan.Utils.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Alan.Utils.ExtensionMethods.ExReflection.ExToDictionary(new {hello = "name"});
            new {hello="name"}.ExToDictionary();
            XmlConvert.Run();
            Console.ReadKey();
        }
    }



}
