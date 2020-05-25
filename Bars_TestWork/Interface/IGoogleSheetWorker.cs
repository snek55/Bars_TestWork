using System.Collections.Generic;

namespace Bars_TestWork.Interface
{
    /// <summary>
    /// Интерфейс класса по работе с Google API v4.
    /// </summary>
    public interface IGoogleSheetWorker
    {
        /// <summary>
        /// Метод который подготавливает данные к необходимому формату и отправка в таблицу. 
        /// </summary>
        /// <param name="data">Данные которые нужно внести в таблицу</param>
        void FormatingAndSendData(List<IList<DataBaseModel>> data);
    }
}