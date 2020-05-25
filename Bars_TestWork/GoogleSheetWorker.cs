using Bars_TestWork.Interface;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Linq;

namespace Bars_TestWork
{
    public class GoogleSheetWorker : IGoogleSheetWorker
    {
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly List<object> _header;
        private const string AplicationName = "test";
        private const string SpreadSheetId = "1GsID9YFwg0ozFe9uehrdgE9zDPVBiVaeq0_RHY4X5X0";
        private SheetsService _sheetsService;

        /// <summary>
        public GoogleSheetWorker(string googleKeyString)
        {
            if(string.IsNullOrWhiteSpace(googleKeyString))
                return;

            var credential = GoogleCredential.FromJson(googleKeyString).CreateScoped(_scopes);

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

        /// <summary>
        void PrepareSheet(in int count)
        {
            var sheetsName = GetSheetsName();
            var sheetsCount = sheetsName.Length;

            if (sheetsCount < count)
            {
                var iterator = 1;

                while (sheetsCount < count)
                {
                    var notHasName = sheetsName.FirstOrDefault(s => s.Equals($"Sheet{iterator}")) == null;

                    if (notHasName)
                    {
                        CreateNewSheet($"Sheet{iterator}");
                        sheetsCount++;
                    }

                    iterator++;
                }
            }
        }

        public void FormatingAndSendData(List<IList<DataBaseModel>> data)
        {
            PrepareSheet(data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                var dataToWrite = new List<IList<object>>();
                dataToWrite.Add(_header);

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

        /// <summary>
        /// Метод создания дового листа в текущем документе. В текущем методе заполняются необходимыполя
        /// и отправляется запрос.
        /// </summary>
        /// <param name="sheetName">Имя листа который следует создать</param>
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

        /// <summary>
        void UpdateEntry(string range, List<IList<object>> data)
        {
            var valueRange = new ValueRange { Values = data };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }
    }
}