using DataAccessObject.DataObjects;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<int> _temporaryIds;
        SQLiteConnection connection;
        private string _userName;

        #endregion

        public void InitDAO()
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

            _temporaryIds = new List<int>();
            _userName = "";
            _questions = new List<IQuestion>();
            _tests = new List<ITest>();
            _users = new List<IUser>();
            _histories = new List<IHistory>();

            InitSQLite();
        }

        public DAO()
        {
            InitDAO();
        }
        
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

        public List<string> GetTestData(int testId)
        {
            List<string> list = new List<string>();
            ITest test = GetTest(testId);
            list.Add(test.Name);
            list.Add(test.Length.ToString());
            list.Add(test.MaximumPoints.ToString());

            return list;
        }

        public IEnumerable<ICar> GettAllCars()
        {
            return _cars;
        }

        public ObservableCollection<IQuestion> GetQuestionsByIds(ObservableCollection<int> questionsIds)
        {
            // todo, check if it is working instead of list
            ObservableCollection<IQuestion> questions = new ObservableCollection<IQuestion>();
            foreach (var id in questionsIds)
            {
                questions.Add(GetQuestion(id));
            }
            return questions;
        }
        
        public List<IQuestion> GetQuestionsByIds(List<int> questionsIds)
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
                #region newDB
                if (newDB)
                {
                    string createString;
                    SQLiteCommand createCommand;

                    createString = "CREATE TABLE QUESTIONS (ID INT PRIMARY KEY, POINTS INT, CONTENT VARCHAR(100), " +
                     "ANSWER1 VARCHAR(100), ANSWER2 VARCHAR(100), ANSWER3 VARCHAR(100), ANSWER4 VARCHAR(100), ANSWER5 VARCHAR(100))";                    
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE TESTS (ID INT PRIMARY KEY, NAME VARCHAR(100), LENGTH INT, MAXPOINTS INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE QUESTIONSIDS (TESTID INT, QUESTIONID INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE USERS (NAME VARCHAR(20), PASSWORD VARCHAR(30), TYPE INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE HISTORY (ID INT, TESTID INT, USERNAME VARCHAR(20), TIME DATETIME, DURATION INT, SCORE INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();
                }
                #endregion

                #region create table
                /*
                string createS = "CREATE TABLE HISTORY (ID INT, TESTID INT, USERNAME VARCHAR(20), TIME DATETIME, DURATION INT, SCORE INT)";
                SQLiteCommand createC = new SQLiteCommand(createS, connection);
                createC.ExecuteNonQuery();
                 */
                #endregion

                connection.Close();
                
                List<IQuestion> questions = SelectAllQuestions();
                List<ITest> tests = SelectAllTests();
                List<IUser> users = SelectAllUsers();
                List<IHistory> histories = SelectAllHistories(); //todo

                _questions = questions;
                _tests = tests;
                _users = users;
                _histories = histories;

            }
        }

        private List<IHistory> SelectAllHistories()
        {
            List<IHistory> histories = new List<IHistory>();

            string selectString = "select * from HISTORY ORDER BY ID asc";

            List<IUser> users = SelectAllUsers();
            List<ITest> tests = SelectAllTests();     

            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    int durationSeconds = Int32.Parse(reader["DURATION"].ToString());
                    string userName = reader["USERNAME"].ToString();
                    int testId = Int32.Parse(reader["TESTID"].ToString());
                                   
                    IUser user = users.Find(x => x.Name == userName);
                    ITest test = tests.Find(x => x.Id == testId);
                    List<IQuestion> questions = GetQuestionsByIds(test.QuestionsIds);

                    History history = new DataObjects.History();
                    history.Name = test.Name;
                    history.Duration = new TimeSpan(durationSeconds/3600, durationSeconds/60, durationSeconds%60);
                    history.Id = Int32.Parse(reader["ID"].ToString());
                    history.TestId = testId;
                    history.Score = Int32.Parse(reader["SCORE"].ToString());
                    history.When = DateTime.ParseExact(reader["TIME"].ToString(), "yyyy-MM-dd HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture);
                    history.User = user;
                    history.Length = test.Length;
                    history.MaximumPoints = test.MaximumPoints;
                    history.QuestionsIds = test.QuestionsIds;
                    history.Question = questions;

                    histories.Add(history);                    
                }
                connection.Close();
            }
             
            return histories;
        }

        private List<IUser> SelectAllUsers()
        {
            List<IUser> users = new List<IUser>();
            string selectString = "select * from USERS";

            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(
                    new DataObjects.User()
                    {
                        Name = reader["NAME"].ToString(),
                        Type = Int32.Parse(reader["TYPE"].ToString()) == 1
                    });
                }
                connection.Close();
            }
            return users;
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

        public List<int> SelectQuestionsIds(int testId)
        {
            List<int> ids = new List<int>();
            string selectString = "select * from QUESTIONSIDS where TESTID = " + testId.ToString();

            SQLiteConnection tmpConnection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            tmpConnection.Open();

            if (tmpConnection.State == ConnectionState.Open)
            {

                SQLiteCommand selectCommand = new SQLiteCommand(selectString, tmpConnection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    //przypisuje do zmiennych dane z bazy
                    //Console.WriteLine("ID: " + reader["ID"] + "\tContent: " + reader["CONTENT"]);
                    int questionId = Int32.Parse(reader["QUESTIONID"].ToString());        
                    ids.Add(questionId);
                }
            }
            tmpConnection.Close();
            return ids;
        }

        private List<IQuestion> SelectAllQuestions()
        {
            List<IQuestion> questions = new List<IQuestion>();
            string selectString = "select * from QUESTIONS order by ID desc";

            SQLiteConnection tmpconnection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            tmpconnection.Open();
            if (tmpconnection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, tmpconnection);
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
                tmpconnection.Close();
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

        public bool InsertQuestion(IQuestion question)
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

        public void DeleteQuestion(int questionId)
        {
            //TODO - delete from database question selected by id
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string result = "DELETE FROM QUESTIONS WHERE ID = " + questionId.ToString();
                SQLiteCommand deleteCommand = new SQLiteCommand(result, connection);
                deleteCommand.ExecuteNonQuery();
                connection.Close();
                //return true;
            }
            else
            {
                //return false;
            }
        }


        public void DeleteQuestionId(int questionId)
        {
            /*
             DELETE FROM table_name
             WHERE [condition];
            */
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string result = "DELETE FROM QUESTIONSIDS WHERE QUESTIONID = " + questionId.ToString();
                SQLiteCommand deleteCommand = new SQLiteCommand(result, connection);
                deleteCommand.ExecuteNonQuery();
                connection.Close();
                //return true;
            }
            else
            {
                //return false;
            }
            
        }

        public void DeleteTest(int testId)
        {
            List<int> ids = SelectQuestionsIds(testId);
            foreach (var id in ids)
            {
                DeleteQuestion(id);
                DeleteQuestionId(id);
            }

            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string result = "DELETE FROM TESTS WHERE ID = " + testId.ToString();
                SQLiteCommand deleteCommand = new SQLiteCommand(result, connection);
                deleteCommand.ExecuteNonQuery();
                connection.Close();
                //return true;
            }
            else
            {
                //return false;
            }

            if (testId != _tests.Count-1)
                FixTestsIds(testId);

            InitDAO();
        }

        private void FixTestsIds(int testId)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();
            int oldId = _tests.Count - 1;
            if (connection.State == ConnectionState.Open)
            {
                string insertString = "UPDATE TESTS SET ID = " + testId.ToString() + " WHERE ID = " + oldId.ToString();
                SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();
                connection.Close();
            }

            //pobieram QIds te, które mają oldId w testId i zamieniam ich testId z oldId na testId

            List<int> questionsIds = SelectQuestionsIds(oldId);
            foreach (var id in questionsIds)
            {
                connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string insertString = "UPDATE QUESTIONSIDS SET TESTID = " + testId.ToString() + " WHERE TESTID = " + oldId.ToString();
                    SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                    insertCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }

        public void InsertQuestionId(int testId, int questionId)
        {          
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string result = "INSERT INTO QUESTIONSIDS (TESTID, QUESTIONID) VALUES (" +
                    testId.ToString() + ", " + questionId.ToString() + ")";
                SQLiteCommand insertCommand = new SQLiteCommand(result, connection);
                insertCommand.ExecuteNonQuery();
                connection.Close();
                //return true;
            }
            else
            {
                //return false;
            }
        }


        public void UpdateTest(int testId, List<string> TestData, List<int> NewTestQuestionsIds)
        {
            //i have to update here test data and questionsids
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string insertString = CreateUpdateTestString(testId, TestData, NewTestQuestionsIds);
                SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();
                connection.Close();

                //update data
                int index = _tests.IndexOf(_tests.Find(x => x.Id == testId));
                _tests[index].Name = TestData[2];
                _tests[index].Length = new TimeSpan(Int32.Parse(TestData[1]) / 60, Int32.Parse(TestData[1]) % 60, 0);
                _tests[index].MaximumPoints = Int32.Parse(TestData[0]);

                //return true;
            }
            else
            {
                //return false;
            }
        }

        private string CreateUpdateTestString(int testId, List<string> TestData, List<int> NewTestQuestionsIds)
        {
            /*
            UPDATE table_name
            SET column1 = value1, column2 = value2...., columnN = valueN
            WHERE [condition];
             // TestData - [0]maxpoints, [1]length, [2]name
             */

            string result = "";
            for (var i = 0; i < TestData.Count; i++)
            {
                TestData[i] = TestData[i].Replace("\'", "");
            }
            
            result += "UPDATE TESTS SET NAME = '" + TestData[2] + "', LENGTH = " + Int32.Parse(TestData[1]) + ", " +
                "MAXPOINTS = " + Int32.Parse(TestData[0]) + " WHERE ID = " + testId.ToString();
            return result;
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
            int count = question.Answer.Count;
            int item;

            for (item = 0; item < count; item++) 
            {
                if (question.Answer[item].Item1.Length == 0)
                    result += (item == 4) ? "'')" : "'', ";
                else
                {
                    result += "'" + question.Answer[item].Item1 + ((question.Answer[item].Item2 == true) ? "_1'" : "_0'");                    
                    result += (item == 4) ? ")" : ", ";
                }
            }

            if (item < 4) 
                for(int i = item; i < 5; i++)
                    result += (i == 4) ? "'')" : "'', "; 

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

        public IQuestion CreateNewQuestion()
        {
            return new DataObjects.Question();
        }

        public IQuestion CreateTempQuestion(List<string> questionString)
        {
            // no new id added
            IQuestion question = new DataObjects.Question(questionString);
            //no adding question to dao
            return question;
        }

        public IQuestion CreateNewQuestion(List<string> questionString)
        {
            //questionString.Add(GetAllQuestions().Count().ToString()); //add ID to question
            questionString.Add(GetNewQuestionId().ToString()); //add ID to question
            IQuestion question = new DataObjects.Question(questionString);
            AddQuestion(question);
            //InsertQuestion(question);     //no insert until saving changes
            return question;
        }

        private int GetNewQuestionId()
        {
            //szukam w bazie najniższy wolny id question który nie jest zajety
            string questionString = "SELECT ID FROM QUESTIONS ORDER BY ID";
            List<int> ids = new List<int>(_temporaryIds);

            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(questionString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    //przypisuje do zmiennych dane z bazy
                    //Console.WriteLine("ID: " + reader["ID"] + "\tContent: " + reader["CONTENT"]);

                    ids.Add(Int32.Parse(reader["ID"].ToString()));

                }
                connection.Close();

                int i = 0;
                int nextId = 0;
                do
                {
                    if (!ids.Contains(i))
                        nextId = i;
                    i++;
                } while (ids.Contains(nextId));

                _temporaryIds.Add(nextId); //add to avoid returning same IDs

                return nextId;
            }


            return -1;
        }                

        public void AddCar(ICar car)
        {
            _cars.Add(car);
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
                { /*raise error*/ }
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
            return new DataObjects.Test();
        }

        void IDAO.AddTest(ITest test)
        {
            throw new NotImplementedException();
        }        

        void IDAO.AddCar(ICar car)
        {
            throw new NotImplementedException();
        }
        
        public IHistory CreateNewHistory()
        {
            return new DataAccessObject.DataObjects.History();
        }

        public IHistory CreateNewHistory(int testId)
        {
            ITest test = GetTest(testId);
            return new DataAccessObject.DataObjects.History(test);
        }

        #endregion


        public int GetNextHistoryId()
        {
            int minId = 0;
            foreach(var history in _histories)
            {
                if (history.Id == minId)
                    minId++;                
            }
            return minId;
        }

        public void CreateNewHistory(IHistory history)
        {
            AddHistory(history);
            if (!InsertHistory(history))
            { /*raise error*/ }
        }

        private bool InsertHistory(IHistory history)
        {
            //"INSERT INTO HISTORY (ID, TESTID, USERNAME, TIME, DURATION, SCORE) VALUES (0, 0, 'MAZAK', '2016-11-13 10:00:00', 5, 2);"

            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string insertString = "INSERT INTO HISTORY (ID, TESTID, USERNAME, TIME, DURATION, SCORE) " +
                "VALUES (" + history.Id.ToString() + ", " + history.TestId.ToString() + ", '" + history.User.Name + 
                "', '"+ history.When.ToString("yyyy-MM-dd HH:mm:ss") +"', "+ history.Duration.TotalSeconds.ToString() +
                ", "+ history.Score.ToString() +");";
                //'2016-11-13 10:00:00'
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

        private void AddHistory(IHistory history)
        {
            _histories.Add(history);
        }
        
        public IUser GetCurrentUser()
        {
            return _users.Find(x => x.Name == _userName);
        }

        public void SetCurrentUser(string UserName)
        {
            _userName = UserName;
        }
        

        public IUser CreateNewUser(string username, bool type)
        {
            IUser user = InitUser(username, type);
            AddUser(user);
            InsertUser(user);
            return user;
        }

        private void AddUser(IUser User)
        {
            _users.Add(User);
        }

        private void InsertUser(IUser User)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string insertString = "INSERT INTO USERS (NAME, TYPE) VALUES ('" + User.Name.ToString() + 
                    "', " + (User.Type == true ? "1" : "0") + ");";
                SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();
                connection.Close();
            }
        }


        public IUser InitUser(string name, bool type)
        {
            return new User(name, type);
        }




        List<IHistory> IDAO.GetAllHistories()
        {
            return _histories;
        }
    }
}
