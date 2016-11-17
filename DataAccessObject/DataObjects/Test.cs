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
        public int Id
        {
            get;
            set;
        }

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

        

        public Test() { }


        public Test(List<string> TestData, IEnumerable<IQuestion> list, List<int> NewTestQuestionsIds, int id)
        {
            this.Name = TestData[2];            
            this.Length = new TimeSpan(Int32.Parse(TestData[1])/60, Int32.Parse(TestData[1]) % 60, 0);
            this.MaximumPoints = Int32.Parse(TestData[0]);
            this.QuestionsIds = NewTestQuestionsIds; 
            this.Question = list.ToList();            
            this.Id = id;
        }


    }
}

