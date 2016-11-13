using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class Question : IQuestion
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

        #region constructors

        public Question() { }

        public Question(List<string> questionString)
        {
            this.Id = Int32.Parse(questionString[7]);
            this.Points = Int32.Parse(questionString[6]);
            /* * 0 - content, 1 .. 5 - answer, 6 - points, 7 - id*/
            this.Content = questionString[0]; 
            this.Answer = new List<Tuple<string,bool>>();

            for (int i = 1; i <= 5; i++)
            {
                Answer.Add(ParseTuple(questionString[i]));
            }

        }

        #endregion

        #region methods

        private Tuple<string, bool> ParseTuple(string answer)
        {
            char[] delimiterChar = { '_' };
            string[] splitAnswer = answer.Split(delimiterChar);
            Tuple<string, bool> tupleAnswer;
            bool isTrue;
            isTrue = (splitAnswer.Length < 2) ? false : true;
            if (isTrue)
            {
                isTrue = (splitAnswer[1] == "1") ? true : false;
                tupleAnswer = new Tuple<string, bool>(splitAnswer[0], isTrue);
            }
            else
            {
                tupleAnswer = new Tuple<string, bool>("", false);
            }            
            
            return tupleAnswer;
        }

        #endregion

    }
}
