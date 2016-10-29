using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class Producer : IProducer
    {

        public int ProducerID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
