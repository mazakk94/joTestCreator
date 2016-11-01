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

        public Question() { }

        public Question(List<string> questionString)
        {
            this.Id = Int32.Parse(questionString[7]);
            this.Points = Int32.Parse(questionString[6]);/*
            Content = "Yellow electric mouse?", Points = 1, Answer = new List<Tuple<string,bool>>
                    { 
                        new Tuple<string, bool>("Electabuzz", false), 
                        new Tuple<string, bool>("Zapdos", false), 
                        new Tuple<string, bool>("Pikachu", true)
                    }
        }
            */


        }
    }
}
