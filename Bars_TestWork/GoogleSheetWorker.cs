using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bars_TestWork.Interface;

namespace Bars_TestWork
{
    public class GoogleSheetWorker : IGoogleSheetWorker
    {
        private readonly string _configFile;
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly List<object> _header;
        private const string AplicationName = "test";
        private const string SpreadSheetId = "1GsID9YFwg0ozFe9uehrdgE9zDPVBiVaeq0_RHY4X5X0";
        private SheetsService _sheetsService;

        public GoogleSheetWorker(string configFile)
        {
            _configFile = configFile;
            var googleKey = GetGoogleKey();
            var credential = GoogleCredential.FromJson(googleKey).CreateScoped(_scopes);

            _header = new List<object>
            {
                "Сервер", "База данных", "Размер в ГБ", "Дата обновления"
            };
            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AplicationName
            });
        }

        private string GetGoogleKey()
        {
            string googleKey;

            using (var streamReader = new StreamReader(_configFile))
            {
                var jsonTextReader = new JsonTextReader(streamReader);
                var jObject = JObject.Load(jsonTextReader);

                googleKey = jObject.GetValue("GoogleSheetKey").ToString();
            }

            return googleKey;
        }

        public void Update(List<IList<DataBaseModel>> data)
        {
            var googleKey = GetGoogleKey();
            var credential = GoogleCredential.FromJson(googleKey).CreateScoped(_scopes);
            var header = new List<object>
            {
                "Сервер", "База данных", "Размер в ГБ", "Дата обновления"
            };
            var dataToWrite = new List<IList<object>>();

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AplicationName
            });
            var sheetsCount = GetSheetsCount();

            if (sheetsCount < data.Count)
            {
                var sheetsName = GetSheetsName();
                var iteratot = 1;

                while(sheetsCount < data.Count)
                {
                    var hasName = sheetsName.FirstOrDefault(s => s.Equals($"Sheet{iteratot}")) != null;

                    if (!hasName)
                    {
                        CreateNewSheet($"Sheet{iteratot}");
                        sheetsCount++;
                    }

                    iteratot++;
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                dataToWrite.Add(header);

                for (int j = 0; j < data[i].Count; j++)
                {
                    dataToWrite.Add(data[i][j].GetValues());
                }

                var bottom = new List<object>
                {
                    "=A2",
                    $"=IF(C{dataToWrite.Count}>0, \"Свободно\", \"Нет Места\")",
                    $"={data[i][0].DiscSize}-SUM(C2:C{dataToWrite.Count})",
                    "=D2"
                };
                dataToWrite.Add(bottom);
                var range = $"Sheet{i + 1}!A1:D{dataToWrite.Count}";

                UpdateEntry(range, dataToWrite);
            }
        }

        string[] GetSheetsName()
        {
            var spreadSheets = _sheetsService.Spreadsheets.Get(SpreadSheetId).Execute();
            var sheetsId = spreadSheets.Sheets.Select(s => s.Properties.Title).ToArray();

            return sheetsId;
        }

        int GetSheetsCount()
        {
            var spreadSheets = _sheetsService.Spreadsheets.Get(SpreadSheetId).Execute();
            var sheetsId = spreadSheets.Sheets.Select(s => s.Properties.SheetId).ToArray().Length;

            return sheetsId;
        }

        void CreateNewSheet(string sheetName)
        {
            var addSheetRequest = new AddSheetRequest();
            var batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();

            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = sheetName;
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = addSheetRequest
            });

            var batchUpdateRequest =
                _sheetsService.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadSheetId);

            batchUpdateRequest.Execute();
        }
        
        void UpdateEntry(string range, List<IList<object>> objects)
        {
            var valueRange = new ValueRange { Values = objects };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }
    }
}