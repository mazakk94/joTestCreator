using Wojtasik.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wojtasik.DataAccessObject.DataObjects
{
    public class DataItem : IDataItem
    {
        public DataItem(string title)
        {
            Title = title;
        }

        public string Title
        {
            get;
            set;
        }


    }
}
