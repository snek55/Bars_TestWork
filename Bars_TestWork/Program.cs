using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bars_TestWork
{
    /// <summary>
    /// Основной класс программы. Управляет основными функциями приложения,
    /// а также считывает настройки конфиг файла.
    /// </summary>
    class Program
    {
        private const string ConfigFile = "configFile.json";
        private const int SleepTime = 1000;
        private static string[] _connectionStrings;
        private static string[] _diskSizes;

        /// <summary>
        /// Входная точка программы. Создается и запускается второй поток для основной работы программы.
        /// Производится проверка нажатых клавиш с клавиатуры, в случае нажатого 'Esc' осуществляется закрытие потока
        /// и завершение программы.
        /// </summary>
        /// <param name="args">Не используется</param>
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

        /// <summary>
        /// Основной метод который вызывает класс по работе с бд и класс по работе с таблицами Google.
        /// В полученные данные дополняет поля с названием Сервера и размером физического носителя
        /// на котором установлен данные сервер. 
        /// </summary>
        /// <returns>Возвращает асинхронный объект задачи</returns>
        private static async Task AsyncWork()
        {
            var dbList = new List<IList<DataBaseModel>>(_connectionStrings.Length);
            var goggleKeyString = GetRawJsonString("GoogleSheetKey");

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

                new GoogleSheetWorker(goggleKeyString).FormatingAndSendData(dbList);
                dbList = new List<IList<DataBaseModel>>();

                Console.WriteLine($"Waiting {SleepTime / 1000} seconds");
                await Task.Delay(SleepTime);
            }
        }

        /// <summary>
        /// Считывает файл указанный в переменной ConfigFile в секциях ConnectionString и DiskSize.
        /// Присваивает значения переменным _connectionStrings и _diskSizes.
        /// </summary>
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

        /// <summary>
        /// Получает данные из Конфигурационного файла хранящий данные в виде Json
        /// и возвращает их в виде Json строки.
        /// Имя файла берется из переменной ConfigFile.
        /// </summary>
        /// <param name="sectionName">Название секции которую нужно получить</param>
        /// <returns>Возвращает строку в формате Json</returns>
        private static string GetRawJsonString(string sectionName)
        {
            string jsonString;

            using (var streamReader = new StreamReader(ConfigFile))
            {
                var jsonTextReader = new JsonTextReader(streamReader);
                var jObject = JObject.Load(jsonTextReader);

                jsonString = jObject.GetValue(sectionName).ToString();
            }

            return jsonString;
        }
    }
}
