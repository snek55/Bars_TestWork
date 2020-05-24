using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bars_TestWork
{
    public class DataBaseModel
    {
        public string ServerName { private get; set; }
        public string DataBaseName { private get; set; }
        public double Size { private get; set; }
        public DateTime UpdateTime { private get; set; }
        public string DiscSize { get; set; }

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