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
            InitSQLite();
            
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
            
            _questions = new List<IQuestion>()
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



            };
            
            _tests = new List<ITest>()
            {
                new DataObjects.Test() { Id = 0, Name = "Pokemon Test", Length = new TimeSpan(1, 0, 0), MaximumPoints = 10, 
                    QuestionsIds = new List<int>(){ 0, 1 }, 
                    Question = new List<IQuestion>()
                    {
                        _questions[0], _questions[1]
                    }

                }, 
                new DataObjects.Test() { Id = 1, Name = "Driver's Test", Length = new TimeSpan(1, 15, 0), MaximumPoints = 18, 
                    QuestionsIds = new List<int>(){ 2 }, 
                    Question = new List<IQuestion>()
                    {
                        _questions[2]
                    }

                }
            };




        }

        private void InitSQLite()
        {
            //string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);


            if (!File.Exists("Tests.sqlite"))
            {
                SQLiteConnection.CreateFile("./Tests.sqlite");
            }
            

            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");

            if(connection.State == ConnectionState.Open)
            {
                ;
            }
            connection.Open();

            /*
            //if not exists
            string createString = "CREATE TABLE QUESTIONS "+
                "(ID INT, "+
                "CONTENT VARCHAR(20))";
            SQLiteCommand createCommand = new SQLiteCommand(createString, connection);
            createCommand.ExecuteNonQuery();

            string insertString;
            SQLiteCommand insertCommand;

            insertString = "INSERT INTO QUESTIONS (ID, CONTENT) VALUES (0, 'What is the first question?')";
            insertCommand = new SQLiteCommand(insertString, connection);
            insertCommand.ExecuteNonQuery();

            insertString = "INSERT INTO QUESTIONS (ID, CONTENT) VALUES (2, 'Name of main character?')";
            insertCommand = new SQLiteCommand(insertString, connection);
            insertCommand.ExecuteNonQuery();

            insertString = "INSERT INTO QUESTIONS (ID, CONTENT) VALUES (1, 'Yellow electric mouse?')";            
            insertCommand = new SQLiteCommand(insertString, connection);
            insertCommand.ExecuteNonQuery();
            */

            string selectString = "select * from QUESTIONS order by ID desc";
            SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
            SQLiteDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
                Console.WriteLine("ID: " + reader["id"] + "\tContent: " + reader["content"]);

            if (connection.State == ConnectionState.Open)
            {
                ;
            }

            connection.Close();

            if (connection.State == ConnectionState.Open)
            {
                ;
            }
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

        ICar IDAO.CreateNewCar()
        {
            throw new NotImplementedException();
        }

        void IDAO.AddCar(ICar car)
        {
            throw new NotImplementedException();
        }
    }
}
