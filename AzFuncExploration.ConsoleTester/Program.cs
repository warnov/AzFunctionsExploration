using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzFuncExploration.ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // var functionKey = "YOUR FUNCTION DEFAULT KEY HERE";
            //var url = "https://YOURAPPNAME.azurewebsites.net/api/YOURFUNCTIONNAME";
            var url = "http://localhost:7071/api/TableStorageReader?customerReference=Cliente1&settingName=StringConnectionMainDB";
            WebClient wc = new WebClient();
            Console.WriteLine("Calling the function");
            var result = wc.DownloadString($"{url}");
            Console.WriteLine($"{result}\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
