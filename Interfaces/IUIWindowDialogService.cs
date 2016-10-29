using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUIWindowDialogService
    {
        bool? ShowDialog(string title, object datacontext);
    }
}

