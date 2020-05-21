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
                .GetSection("DbConnectionStrings")
                .GetChildren()
                .ToArray()
                .Select(cs => cs.Value)
                .ToArray();
            var dbList = new List<DataBaseModel>(connectStrToServers.Length);

            for (int i = 0; i < connectStrToServers.Length; i++)
            {
                var serverModels = new DbWorker(connectStrToServers[i]).GetDbServerModels();

                foreach (var serverModel in serverModels)
                {
                    serverModel.ServerName = $"Server{i}";
                    dbList.Add(serverModel);
                }
            }

            while (true)
            {
                //TODO: timeout
            }
        }
    }
}
