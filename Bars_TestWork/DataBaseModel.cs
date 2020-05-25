using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bars_TestWork
{
    /// <summary>
    /// Модель для данных получаемых из базы данных
    /// </summary>
    public class DataBaseModel
    {
        /// <summary>
        /// Имя сервера на котором хранилась база
        /// </summary>
        public string ServerName { private get; set; }
        /// <summary>
        /// Имя базы данных
        /// </summary>
        public string DataBaseName { private get; set; }
        /// <summary>
        /// Размер базы данных
        /// </summary>
        public double Size { private get; set; }
        /// <summary>
        /// Дата когда были получены текущие сведения
        /// </summary>
        public DateTime UpdateTime { private get; set; }
        /// <summary>
        /// Размер дискового пространства на котором хранится база данных
        /// </summary>
        public string DiscSize { get; set; }

        /// <summary>
        /// Метод формирования текущих данных в формате List
        /// </summary>
        /// <returns>Возвращает список типа object</returns>
        public List<object> GetValues()
        {
            var values = new List<object>();

            values.Add(ServerName);
            values.Add(DataBaseName);
            values.Add(Size);
            values.Add(UpdateTime.ToString("d", CultureInfo.CurrentCulture));

            return values;
        }
    }
}