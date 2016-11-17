using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wojtasik.Interfaces
{
    public interface IQuestion
    {
        int Id { get; set; }
        int Points { get; set; }
        string Content { get; set; }        
        List<Tuple<string, bool>> Answer { get; set; }
        
        //BitmapImage Image { get; set; }
    }
}
