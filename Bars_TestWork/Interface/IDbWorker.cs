using System.Collections.Generic;

namespace Bars_TestWork.Interface
{
    public interface IDbWorker
    {
        List<DataBaseModel> GetDbServerModels();
    }
}