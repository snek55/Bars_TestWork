using System.Collections.Generic;

namespace Bars_TestWork.Interface
{
    public interface IGoogleSheetWorker
    {
        void FormatingAndSendData(List<IList<DataBaseModel>> data);
    }
}