using System.Collections.Generic;

namespace Bars_TestWork.Interface
{
    /// <summary>
    /// Интерфейс класса работающего с PostgreSQL
    /// </summary>
    public interface IDbWorker
    {
        /// <summary>
        /// Метод открывающий соединение, формирующий запрос и формирующий определенный формат
        /// </summary>
        /// <returns>Возвращает полученные данные</returns>
        List<DataBaseModel> GetDbServerModels();
    }
}