using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class AnsweredQuestion : IAnsweredQuestion
    {

        public Tuple<int> ChosenAnswers
        {
            get;
            set;
        }

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

        public Tuple<string, bool> Answer
        {
            get;
            set;
        }


        List<Tuple<string, bool>> IQuestion.Answer
        {
            get;
            set;
        }
    }
}
