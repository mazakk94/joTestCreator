﻿using System;
using Interfaces;

namespace DataAccessObject.DataObjects
{
    public class DataService : IDataService
    {
        public void GetData(Action<IDataItem, Exception> callback)
        {
            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }
    }
}