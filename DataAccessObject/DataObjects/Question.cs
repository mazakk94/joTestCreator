using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class Question: IQuestion
    {
        public int Id
        {
            get;
            set;
        }

        public int Points
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public List<Tuple<string, bool>> Answer
        {
            get;
            set;
        }
    }
}
