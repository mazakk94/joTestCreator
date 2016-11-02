using DataAccessObject.DataObjects;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class DAO : IDAO
    {
        
        private List<IProducer> _producers;
        private List<ICar> _cars;
        

        private List<IAnsweredQuestion> _answeredQuestions;
        private List<IHistory> _histories;
        private List<IQuestion> _questions;
        private List<ITest> _tests;
        private List<IUser> _users;

        SQLiteConnection connection;
        
        public DAO()
        {           
            
            _producers = new List<IProducer>()
            {
                new DataObjects.Producer(){ ProducerID = 1, Name = "one"},
                new DataObjects.Producer(){ ProducerID = 2, Name = "two"},
                new DataObjects.Producer(){ ProducerID = 3, Name = "three"}
            };

            _cars = new List<ICar>()
            {
                new DataObjects.Car() { CarID = 6, Name = "Polo6", Producer = _producers[1], Price = 45, Color = "Red"},
                new DataObjects.Car() { CarID = 1, Name = "Polo1", Producer = _producers[0], Price = 260, Color = "Black"},
                new DataObjects.Car() { CarID = 2, Name = "Polo2", Producer = _producers[0], Price = 270, Color = "Red"},
                new DataObjects.Car() { CarID = 3, Name = "Polo2", Producer = _producers[1], Price = 260, Color = "White"},
                new DataObjects.Car() { CarID = 4, Name = "Polo4", Producer = _producers[1], Price = 45, Color = "Red"},
                new DataObjects.Car() { CarID = 5, Name = "Polo5", Producer = _producers[1], Price = 45, Color = "Red"}
                
            };
            
            _questions = new List<IQuestion>();/*
            {
                new DataObjects.Question() 
                { 
                    Id = 0, Content = "Yellow electric mouse?", Points = 1, Answer = new List<Tuple<string,bool>>
                    { 
                        new Tuple<string, bool>("Electabuzz", false), 
                        new Tuple<string, bool>("Zapdos", false), 
                        new Tuple<string, bool>("Pikachu", true)
                    }
                },
                
                new DataObjects.Question() 
                { 
                    Id = 1, Content = "Name of main character?", Points = 1, Answer = new List<Tuple<string,bool>>
                    { 
                        new Tuple<string, bool>("Ash", true), 
                        new Tuple<string, bool>("Brock", false),
                        new Tuple<string, bool>("Giovanni", false), 
                        new Tuple<string, bool>("Misty", false)
                    }
                },
                
                new DataObjects.Question() 
                { 
                    Id = 2, Content = "Is it possible to turn right on red?", Points = 1, Answer = new List<Tuple<string,bool>>
                    { 
                        new Tuple<string, bool>("Yes, always", false), 
                        new Tuple<string, bool>("Never", false),
                        new Tuple<string, bool>("Yes, but only if there's a green arrow", false)
                    }
                }

            };*/
            InitSQLite();

            _tests = new List<ITest>()
            {
                new DataObjects.Test() { Id = 0, Name = "Pokemon Test", Length = new TimeSpan(1, 0, 0), MaximumPoints = 10, 
                    QuestionsIds = new List<int>(){ 0 }, 
                    Question = new List<IQuestion>()
                    {
                        _questions[0] //, _questions[1]
                    }

                }, 
                new DataObjects.Test() { Id = 1, Name = "Driver's Test", Length = new TimeSpan(1, 15, 0), MaximumPoints = 18, 
                    QuestionsIds = new List<int>(){ 1 }, 
                    Question = new List<IQuestion>()
                    {
                        _questions[1]
                    }

                }
            };




        }

        private void InitSQLite()
        {
            //string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            bool newDB = false;
            if (!File.Exists("Tests.sqlite"))
            {
                newDB = true;
                SQLiteConnection.CreateFile("./Tests.sqlite");
            }

            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();
            

            if (connection.State == ConnectionState.Open)
            {
                if (newDB)
                {
                    string createString = "CREATE TABLE QUESTIONS " +
                        "(" +
                        "ID INT PRIMARY KEY, " +
                        "POINTS INT, " +
                        "CONTENT VARCHAR(100), " +
                        "ANSWER1 VARCHAR(100), " +
                        "ANSWER2 VARCHAR(100), " +
                        "ANSWER3 VARCHAR(100), " +
                        "ANSWER4 VARCHAR(100), " +
                        "ANSWER5 VARCHAR(100))";
                    SQLiteCommand createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();


                    string insertString;
                    SQLiteCommand insertCommand;

                    insertString = "INSERT INTO QUESTIONS (ID, POINTS, CONTENT, ANSWER1, ANSWER2, ANSWER3, " +
                        "ANSWER4, ANSWER5) VALUES (0, 10, 'What is the first question?', 'abc_1', 'def_0', '', '', '')";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();


                    insertString = "INSERT INTO QUESTIONS (ID, POINTS, CONTENT, ANSWER1, ANSWER2, ANSWER3, " +
                        "ANSWER4, ANSWER5) VALUES (1, 5, 'Yellow electric mouse?', 'Pika_0', 'Chu_1', 'Zapdos_0', '', '')";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();
                }

                string selectString = "select * from QUESTIONS order by ID desc";
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    //przypisuje do zmiennych dane z bazy
                    Console.WriteLine("ID: " + reader["ID"] + "\tContent: " + reader["CONTENT"]);

                    _questions.Add(
                    new DataObjects.Question()
                    {
                        Id = Int32.Parse(reader["ID"].ToString()),
                        Content = reader["CONTENT"].ToString(),
                        Points = Int32.Parse(reader["POINTS"].ToString()),
                        Answer = new List<Tuple<string, bool>>(ReadAnswers(reader))
                    });

                }

            }
                        
            connection.Close();
        }

        private List<Tuple<string, bool>> ReadAnswers(SQLiteDataReader reader)
        {
            List<Tuple<string, bool>> Answers = new List<Tuple<string,bool>>();

            if (reader["ANSWER1"].ToString().Length != 0)
                Answers.Add(ParseTuple(reader["ANSWER1"].ToString()));            
            else
                return Answers;

            if (reader["ANSWER2"].ToString().Length != 0)
                Answers.Add(ParseTuple(reader["ANSWER2"].ToString()));
            else 
                return Answers;

            if (reader["ANSWER3"].ToString().Length != 0)
                Answers.Add(ParseTuple(reader["ANSWER3"].ToString()));
            else
                return Answers;

            if (reader["ANSWER4"].ToString().Length != 0)
                Answers.Add(ParseTuple(reader["ANSWER4"].ToString()));
            else
                return Answers;

            if (reader["ANSWER5"].ToString().Length != 0)
                Answers.Add(ParseTuple(reader["ANSWER5"].ToString()));
            else
                return Answers;

            return Answers;
        }

        private Tuple<string, bool> ParseTuple(string answer)
        {            
            char[] delimiterChar = {'_'};
            string[] splitAnswer = answer.Split(delimiterChar);
            bool isTrue = (splitAnswer[1] == "1") ? true : false;
            Tuple<string, bool> tupleAnswer = new Tuple<string, bool>(splitAnswer[0], isTrue);
            return tupleAnswer;            
        }
    
    


      //  private void InsertToDatabase(string tableName, )

        public void transaction(string cs)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                using (SQLiteTransaction tr = con.BeginTransaction())
                {
                    using (SQLiteCommand cmd = con.CreateCommand())
                    {

                        cmd.Transaction = tr;
                        cmd.CommandText = "DROP TABLE IF EXISTS Friends";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"CREATE TABLE Friends(Id INTEGER PRIMARY KEY, 
                                        Name TEXT)";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Friends(Name) VALUES ('Tom')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Friends(Name) VALUES ('Rebecca')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Friends(Name) VALUES ('Jim')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Friends(Name) VALUES ('Robert')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Friends(Name) VALUES ('Julian')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO Friends(Name) VALUES ('Jane')";
                        cmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                }

                con.Close();
            }
        }
        
        
    



        public IEnumerable<IProducer> GetAllProducers()
        {
            return _producers;
        }

        public IEnumerable<ICar> GettAllCars()
        {
            return _cars;
        }


        public ICar CreateNewCar()
        {
            return new DataObjects.Car();
        }

        public IQuestion CreateNewQuestion(List<string> questionString)
        {
            string id = GetAllQuestions().Count().ToString();
            questionString.Add(id.ToString());
            IQuestion question = new DataObjects.Question(questionString);
            AddQuestion(question);
            return question;
        }

        public void AddCar(ICar car)
        {
            _cars.Add(car);
        }


        public IEnumerable<IAnsweredQuestion> GetAllAnsweredQuestions()
        {
            return _answeredQuestions;
        }

        public IEnumerable<IHistory> GetAllHistories()
        {
            return _histories;
        }
        
        public IEnumerable<IQuestion> GetAllQuestions()
        {
            return _questions;
        }

        public IEnumerable<ITest> GetAllTests()
        {
            return _tests;
        }

        public IQuestion GetQuestion(int questionId)
        {
            return _questions.Find(x => x.Id == questionId);
        }

        
        public ITest GetTest(int id)
        {
            return _tests.Find(test => test.Id == id);
        }
        

        public IEnumerable<IUser> GetAllUsers()
        {
            return _users;
        }


        public IQuestion CreateNewQuestion()
        {
            return new DataObjects.Question();
        }

        public ITest CreateNewTest()
        {
            return new DataObjects.Test();
        }

        public void AddTest(ITest test)
        {
            _tests.Add(test);
        }

        public void AddQuestion(IQuestion question)
        {
            _questions.Add(question);
        }

        public void AddQuestionToTest(ITest test, IQuestion question)
        {
            
        }



        IEnumerable<IProducer> IDAO.GetAllProducers()
        {
            throw new NotImplementedException();
        }

        IEnumerable<ICar> IDAO.GettAllCars()
        {
            throw new NotImplementedException();
        }

        IEnumerable<ITest> IDAO.GetAllTests()
        {
            return GetAllTests();
        }

        IEnumerable<IQuestion> IDAO.GetAllQuestions()
        {
            return GetAllQuestions();
        }

        ITest IDAO.CreateNewTest()
        {
            throw new NotImplementedException();
        }

        void IDAO.AddTest(ITest test)
        {
            throw new NotImplementedException();
        }

        

        void IDAO.AddCar(ICar car)
        {
            throw new NotImplementedException();
        }
    }
}
