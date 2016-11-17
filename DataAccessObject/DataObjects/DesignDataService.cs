using Wojtasik.Interfaces;
using System;

namespace Wojtasik.DataAccessObject.DataObjects
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<IDataItem, Exception> callback)
        {
            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}