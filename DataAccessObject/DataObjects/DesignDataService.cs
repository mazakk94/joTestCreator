using Interfaces;
using System;

namespace DataAccessObject.DataObjects
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<IDataItem, Exception> callback)
        {
            var item = new DataObjects.DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}