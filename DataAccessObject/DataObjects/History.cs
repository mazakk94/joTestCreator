using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class History : Test, IHistory
    {
        #region variables

        public int Id
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }

        public IUser User
        {
            get;
            set;
        }

        public DateTime When
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public List<IAnsweredQuestion> ChosenAnswers
        {
            get;
            set;
        }

        #endregion

        #region test properties

        public int TestId
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
              
        #endregion

        public History()
        {

        }

        public History(ITest test)
        {
            this.TestId = test.Id;
            this.Name = test.Name;
            this.MaximumPoints = test.MaximumPoints;
            this.Question = test.Question;
            this.QuestionsIds = test.QuestionsIds;
            this.ChosenAnswers = new List<IAnsweredQuestion>(this.Question.Count);
            for(int i = 0; i < this.Question.Count; i++)
                this.ChosenAnswers.Add(new AnsweredQuestion());
        }
    }
}
