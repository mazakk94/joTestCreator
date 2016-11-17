using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wojtasik.Interfaces;

namespace Wojtasik.DataAccessObject.DataObjects
{
    class AnsweredQuestion : IAnsweredQuestion
    {

        public List<int> ChosenAnswers
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

        public List<Tuple<string, bool>> Answer
        {
            get;
            set;
        }

        public AnsweredQuestion()
        {
            ChosenAnswers = new List<int>();
        }


    }
}
