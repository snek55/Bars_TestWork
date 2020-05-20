using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Bars_TestWork
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("configFile.json")
                .Build();
            var connectStrToServers = configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToArray()
                .Select(cs => cs.Value)
                .ToArray();
            var dbList = new List<DataBaseModel>(connectStrToServers.Length);

            while (true)
            {
                //TODO: timeout
            }
        }
    }
}
