using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class Test : ITest
    {
        public string Name
        {
            get;
            set;
        }

        public TimeSpan Length
        {
            get;
            set;
        }

        public int MaximumPoints
        {
            get;
            set;
        }

        public List<int> QuestionsIds
        {
            get;
            set;
        }

        public List<IQuestion> Question
        {
            get;
            set;
        }
        
        public int Id
        {
            get;
            set;
        }
    }
}
