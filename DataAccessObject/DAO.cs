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
        
        #region variable definitions

        private List<IProducer> _producers;
        private List<ICar> _cars;     
        private List<IAnsweredQuestion> _answeredQuestions;
        private List<IHistory> _histories;
        private List<IQuestion> _questions;
        private List<ITest> _tests;
        private List<IUser> _users;
        SQLiteConnection connection;

        #endregion
        
        public DAO()
        {
            #region old
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
            #endregion

            _questions = new List<IQuestion>();
            #region comment
            /*
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
            #endregion

            _tests = new List<ITest>();
            #region comment
            /*  {
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
            };*/
            #endregion

            InitSQLite();


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

        #region getters

        public IEnumerable<ICar> GettAllCars()
        {
            return _cars;
        }

        private List<IQuestion> GetQuestionsByIds(List<int> questionsIds)
        {
            List<IQuestion> questions = new List<IQuestion>();
            foreach (var id in questionsIds)
            {
                questions.Add(GetQuestion(id));
            }
            return questions;
        }

        public IEnumerable<IProducer> GetAllProducers()
        {
            return _producers;
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
            _questions.Clear();
            _questions = SelectAllQuestions();
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

        #endregion

        #region sql methods
        
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
                    string createString = "CREATE TABLE QUESTIONS (" +
                        "ID INT PRIMARY KEY, POINTS INT, CONTENT VARCHAR(100), " +
                        "ANSWER1 VARCHAR(100), " +
                        "ANSWER2 VARCHAR(100), " +
                        "ANSWER3 VARCHAR(100), " +
                        "ANSWER4 VARCHAR(100), " +
                        "ANSWER5 VARCHAR(100))";
                    SQLiteCommand createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE TESTS (ID INT PRIMARY KEY, NAME VARCHAR(100), LENGTH INT, MAXPOINTS INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE QUESTIONSIDS (TESTID INT, QUESTIONID INT)";
                    createCommand = new SQLiteCommand(createString, connection);
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
                    
                    insertString = "INSERT INTO QUESTIONS (ID, POINTS, CONTENT, ANSWER1, ANSWER2, ANSWER3, " +
                        "ANSWER4, ANSWER5) VALUES (2, 15, 'Name of main character?', 'Ash_1', 'Brock_0', 'Giovanni_0', 'Misty_0', '')";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();

                    insertString = "INSERT INTO TESTS (ID, NAME, LENGTH, MAXPOINTS) VALUES (" +
                        "0, 'Pokemon Test', 60, 10)";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();

                    insertString = "INSERT INTO TESTS (ID, NAME, LENGTH, MAXPOINTS) VALUES (" +
                        "1, 'Drivers test', 75, 18)";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();

                    insertString = "INSERT INTO QUESTIONSIDS (TESTID, QUESTIONID) VALUES (1, 1)";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();

                    insertString = "INSERT INTO QUESTIONSIDS (TESTID, QUESTIONID) VALUES (0, 0)";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();

                    insertString = "INSERT INTO QUESTIONSIDS (TESTID, QUESTIONID) VALUES (1, 2)";
                    insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();
                    

                }

                connection.Close();

                List<IQuestion> questions = SelectAllQuestions();
                _questions = questions;

                List<ITest> tests = SelectAllTests();
                _tests = tests;

                
                                

            }

            
        }

        private List<ITest> SelectAllTests()
        {
            List<ITest> tests = new List<ITest>();
            string selectString = "select * from TESTS order by id";

            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    //przypisuje do zmiennych dane z bazy
                    //Console.WriteLine("ID: " + reader["ID"] + "\tContent: " + reader["CONTENT"]);
                    int id = Int32.Parse(reader["ID"].ToString());
                    List<int> questionsIds = SelectQuestionsIds(id);

                    tests.Add(
                    new DataObjects.Test()
                    {
                        Id = id,
                        Name = reader["NAME"].ToString(),                        
                        Length = new TimeSpan(Int32.Parse(reader["LENGTH"].ToString())/60, Int32.Parse(reader["LENGTH"].ToString()) % 60, 0),
                        MaximumPoints = Int32.Parse(reader["MAXPOINTS"].ToString()), 
                        QuestionsIds = questionsIds
                    });
                }
                connection.Close();
            }
            //connection.Close();
            return tests;
        }

        private List<int> SelectQuestionsIds(int id)
        {
            List<int> ids = new List<int>();
            string selectString = "select * from QUESTIONSIDS where TESTID = "+id.ToString();

            //connection.Open();
            if (connection.State == ConnectionState.Open)
            {

                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    //przypisuje do zmiennych dane z bazy
                    //Console.WriteLine("ID: " + reader["ID"] + "\tContent: " + reader["CONTENT"]);
                    int questionId = Int32.Parse(reader["QUESTIONID"].ToString());        
                    ids.Add(questionId);
                }
            }
            //connection.Close();
            return ids;
        }

        private List<IQuestion> SelectAllQuestions()
        {
            List<IQuestion> questions = new List<IQuestion>();
            string selectString = "select * from QUESTIONS order by ID desc";

            
            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    //przypisuje do zmiennych dane z bazy
                    Console.WriteLine("ID: " + reader["ID"] + "\tContent: " + reader["CONTENT"]);

                    questions.Add(
                    new DataObjects.Question()
                    {
                        Id = Int32.Parse(reader["ID"].ToString()),
                        Content = reader["CONTENT"].ToString(),
                        Points = Int32.Parse(reader["POINTS"].ToString()),
                        Answer = new List<Tuple<string, bool>>(ReadAnswers(reader))
                    });

                }
                connection.Close();
            }
            
            return questions;
        }
        
        private List<Tuple<string, bool>> ReadAnswers(SQLiteDataReader reader)
        {
            List<Tuple<string, bool>> Answers = new List<Tuple<string, bool>>();

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

        private bool InsertTest(ITest test)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string insertString = CreateTestString(test);
                SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool InsertQuestion(IQuestion question)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string insertString = CreateQuestionString(question);
                SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();

                connection.Close();
                return true;
            } 
            else
            {
                return false;
            }
        }

        private bool InsertQuestionsIds(int id, List<int> NewTestQuestionsIds)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                List<string> insertStringList = CreateQuestionsIdsString(id, NewTestQuestionsIds);

                using (var command = new SQLiteCommand(connection))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        for (int i = 0; i < insertStringList.Count; i++)
                        {
                            command.CommandText = insertStringList[i];                                
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }

                connection.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<string> CreateQuestionsIdsString(int id, List<int> NewTestQuestionsIds)
        {
            List<string> result = new List<string>();
            string init = "INSERT INTO QUESTIONSIDS (TESTID, QUESTIONID) VALUES (";  
            for (int i = 0; i < NewTestQuestionsIds.Count; i++)
            {
                result.Add(init+id.ToString()+", "+NewTestQuestionsIds[i]+")");
            }
            return result;
        }    

        private string CreateQuestionString(IQuestion question)
        {
            string result = "";
            string init = "INSERT INTO QUESTIONS (ID, POINTS, CONTENT, ANSWER1, ANSWER2, ANSWER3, ANSWER4, ANSWER5) VALUES (";            
            result += init;
            result += question.Id.ToString() + ", ";
            result += question.Points.ToString() + ", ";
            result += "'" + question.Content + "', ";

            for (int item = 0; item < 5; item++) 
            {
                if (question.Answer[item].Item1.Length == 0)
                    result += (item == 4) ? "'')" : "'', ";
                else
                {
                    result += "'" + question.Answer[item].Item1 + ((question.Answer[item].Item2 == true) ? "_1'" : "_0'");                    
                    result += (item == 4) ? ")" : ", ";
                }
            }
            return result;
        }

        private string CreateTestString(ITest test)
        {
            string result = "";
            result += "INSERT INTO TESTS (ID, NAME, LENGTH, MAXPOINTS) VALUES (";
            result += test.Id.ToString() + ", ";
            result += "'" + test.Name.ToString() + "', ";
            result += (test.Length.Minutes + test.Length.Hours * 60).ToString() + ", ";
            result += test.MaximumPoints.ToString() + ")";

            return result;
        }

        #endregion
                       
        #region dao methods

        private Tuple<string, bool> ParseTuple(string answer)
        {
            char[] delimiterChar = { '_' };
            string[] splitAnswer = answer.Split(delimiterChar);
            bool isTrue = (splitAnswer[1] == "1") ? true : false;
            Tuple<string, bool> tupleAnswer = new Tuple<string, bool>(splitAnswer[0], isTrue);
            return tupleAnswer;
        }

        public ICar CreateNewCar()
        {
            return new DataObjects.Car();
        }

        public IQuestion CreateNewQuestion(List<string> questionString)
        {
            questionString.Add(GetAllQuestions().Count().ToString());
            IQuestion question = new DataObjects.Question(questionString);
            AddQuestion(question);
            InsertQuestion(question);
            return question;
        }                

        public void AddCar(ICar car)
        {
            _cars.Add(car);
        }

        public IQuestion CreateNewQuestion()
        {
            return new DataObjects.Question();
        }

        public ITest CreateNewTest()
        {
            return new DataObjects.Test();
        }

        public ITest CreateNewTest(List<string> TestData, List<int> NewTestQuestionsIds)
        {
            int id = _tests.Count;
            ITest test = new DataObjects.Test(TestData, GetQuestionsByIds(NewTestQuestionsIds), NewTestQuestionsIds, id);
            AddTest(test); //to tylko dodaje do DAO
            if (!InsertTest(test))
            {
                //raise error
            }
            else //insert successfull
            {
                InsertQuestionsIds(id, NewTestQuestionsIds);
            }   

            return test;
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
            //TODO OR NOT
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

        #endregion

    }
}
