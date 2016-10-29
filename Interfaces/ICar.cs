using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICar
    {
        int CarID { get; set; }
        string Name { get; set; }
        IProducer Producer { get; set; }
        float Price { get; set; }
        string Color { get; set; }

    }
}
