using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public interface IDataService
    {
        void GetData(Action<IDataItem, Exception> callback);
    }
}  
