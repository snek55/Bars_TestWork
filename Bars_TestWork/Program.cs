using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Bars_TestWork
{
    class Program
    {
        private static string[] _connectionStrings;
        private static string[] _diskSizes;

        static void Main(string[] args)
        {
            InitConfigVariables();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var dbList = new List<IList<DataBaseModel>>(_connectionStrings.Length);

            Console.WriteLine("Press \"Esc\" to exit.\n");

            Task infinitySycle = new Task(() =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Завершение программы");
                        return;
                    }

            for (int i = 0; i < connectStrToServers.Length; i++)
            {
                var serverModels = new DbWorker(connectStrToServers[i]).GetDbServerModels();

                foreach (var serverModel in serverModels)
                {
                    serverModel.ServerName = $"Server{i}";
                    dbList.Add(serverModel);
                }
                }
            });

            infinitySycle.Start();

            do
            {
                if (Console.ReadKey().Key != ConsoleKey.Escape)
                {
                    cancellationTokenSource.Cancel();
                    Thread.Sleep(100);
                }
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
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
