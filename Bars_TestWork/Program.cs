using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bars_TestWork
{
    class Program
    {
        private const string ConfigFile = "configFile.json";
        private const int SleepTime = 1000;
        private static string[] _connectionStrings;
        private static string[] _diskSizes;

        static void Main(string[] args)
        {
            InitConfigVariables();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Console.WriteLine("Press \"Esc\" to exit.\n");

            Task.Run(AsyncWork, cancellationToken);

            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    cancellationTokenSource.Cancel();
                    Thread.Sleep(100);
                    break;
                }
            }
        }

        private static async Task AsyncWork()
        {
            var dbList = new List<IList<DataBaseModel>>(_connectionStrings.Length);

            while (true)
            {
                Console.WriteLine("Starting update");

                for (int i = 0; i < _connectionStrings.Length; i++)
                {
                    var serverModels = new DbWorker(_connectionStrings[i]).GetDbServerModels();

                    foreach (var serverModel in serverModels)
                    {
                        serverModel.ServerName = $"Server{i + 1}";
                        serverModel.DiscSize = _diskSizes[i];
                    }

                    dbList.Add(serverModels);
                }

                new GoogleSheetWorker(ConfigFile).FormatingAndSendData(dbList);
                dbList = new List<IList<DataBaseModel>>();

                Console.WriteLine($"Waiting {SleepTime / 1000} seconds");
                await Task.Delay(SleepTime);
            }
        }

        private static void InitConfigVariables()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(ConfigFile)
                .Build();
            _connectionStrings = configuration
                .GetSection("Servers")
                .GetChildren()
                .Select(s => s.GetSection("ConnectionString").Value)
                .ToArray();
            _diskSizes = configuration
                .GetSection("Servers")
                .GetChildren()
                .Select(s => s.GetSection("DiskSize").Value)
                .ToArray();
        }
    }
}
