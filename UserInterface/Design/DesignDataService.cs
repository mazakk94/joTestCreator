using Interfaces;
using System;

namespace DataAccessObject.DataObjects
{
    public class DesignDataService : IDataService
    {
        /*
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }*/

        public void GetData(Action<IDataItem, Exception> callback)
        {
            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}