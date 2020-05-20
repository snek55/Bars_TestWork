using Microsoft.Extensions.Configuration;

namespace Bars_TestWork
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("configFile.json")
                .Build();
            var connectionString = configuration["ConnectionString"];
            var sizes = new DbWorker(connectionString).GetSizes();

            while (true)
        }
    }
}
