using Wojtasik.DataAccessObject.DataObjects;
using Wojtasik.Interfaces;
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

namespace Wojtasik.DataAccessObject
{
    public class DAO : IDAO
    {

        #region variable definitions

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


        #region getters

        public IEnumerable<string> GetTestData(int testId)
        {
            List<string> list = new List<string>();
            ITest test = GetTest(testId);
            list.Add(test.Name);
            list.Add(test.Length.ToString());
            list.Add(test.MaximumPoints.ToString());
            list.Add(test.MultiCheck.ToString());

            return list;
        }
        
        public IEnumerable<IQuestion> GetQuestionsByIds(List<int> questionsIds)
        {
            List<IQuestion> questions = new List<IQuestion>();
            foreach (var id in questionsIds)
                questions.Add(GetQuestion(id));
            return questions;
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
            _questions = SelectAllQuestions().ToList();
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

        #endregion

        #region sql methods

        private void InitSQLite()
        {
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

                    createString = "CREATE TABLE TESTS (ID INT PRIMARY KEY, NAME VARCHAR(100), LENGTH INT, MAXPOINTS INT, MULTICHECK BOOLEAN)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE QUESTIONSIDS (TESTID INT, QUESTIONID INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE USERS (NAME VARCHAR(20), TYPE INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE HISTORY (ID INT, TESTNAME VARCHAR(20), USERNAME VARCHAR(20), TIME DATETIME, DURATION INT, SCORE INT)";
                    createCommand = new SQLiteCommand(createString, connection);
                    createCommand.ExecuteNonQuery();

                    createString = "CREATE TABLE CHECKEDANSWERS (HISTORYID INT, QUESTIONID INT, ANSWER INT)";
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

                List<IQuestion> questions = SelectAllQuestions().ToList();
                List<ITest> tests = SelectAllTests().ToList();
                List<IUser> users = SelectAllUsers().ToList();
                List<IHistory> histories = SelectAllHistories().ToList();

                _questions = questions;
                _tests = tests;
                _users = users;
                _histories = histories;

            }
        }

        private IEnumerable<IHistory> SelectAllHistories()
        {
            List<IHistory> histories = new List<IHistory>();

            string selectString = "select * from HISTORY ORDER BY ID asc";

            List<IUser> users = SelectAllUsers().ToList();

            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    int durationSeconds = Int32.Parse(reader["DURATION"].ToString());
                    string userName = reader["USERNAME"].ToString();
                    string testName  = reader["TESTNAME"].ToString();

                    IUser user = users.Find(x => x.Name == userName);
                    //ITest test = tests.Find(x => x.Id == testId);
                    

                    History history = new DataObjects.History();
                    history.Name = testName;
                    history.Duration = new TimeSpan(durationSeconds / 3600, durationSeconds / 60, durationSeconds % 60);
                    history.Id = Int32.Parse(reader["ID"].ToString());
                    
                    history.Score = Int32.Parse(reader["SCORE"].ToString());
                    history.When = DateTime.ParseExact(reader["TIME"].ToString(), "yyyy-MM-dd HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture);
                    history.User = user;
                    List<int> questionIds = SelectQuestionsIds(history.Id).ToList();
                    List<IQuestion> questions = GetQuestionsByIds(questionIds).ToList();
                    history.QuestionsIds = questionIds;
                    history.Question = questions;

                    histories.Add(history);
                }
                connection.Close();
            }

            return histories;
        }

        private IEnumerable<IUser> SelectAllUsers()
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

        private IEnumerable<ITest> SelectAllTests()
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
                    int id = Int32.Parse(reader["ID"].ToString());
                    List<int> questionsIds = SelectQuestionsIds(id).ToList();
                    string multiCheck = reader["MULTICHECK"].ToString();
                    tests.Add(
                    new DataObjects.Test()
                    {
                        Id = id,
                        Name = reader["NAME"].ToString(),
                        Length = new TimeSpan(Int32.Parse(reader["LENGTH"].ToString()) / 60, Int32.Parse(reader["LENGTH"].ToString()) % 60, 0),
                        MaximumPoints = Int32.Parse(reader["MAXPOINTS"].ToString()),
                        QuestionsIds = questionsIds,
                        MultiCheck = (reader["MULTICHECK"].ToString() == "True" || reader["MULTICHECK"].ToString() == "1" ) ? true : false
                    });
                }
                connection.Close();
            }
            //connection.Close();
            return tests;
        }

        public IEnumerable<int> SelectQuestionsIds(int testId)
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
                    int questionId = Int32.Parse(reader["QUESTIONID"].ToString());
                    ids.Add(questionId);
                }
            }
            tmpConnection.Close();
            return ids;
        }

        private IEnumerable<IQuestion> SelectAllQuestions()
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

        public List<List<int>> SelectCheckedAnswers(int historyId)
        {
            List<List<int>> answersList = new List<List<int>>();
            List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();

            string selectString = "select * from CHECKEDANSWERS WHERE HISTORYID = "
                + historyId.ToString() + " ORDER BY QUESTIONID ASC";

            SQLiteConnection tmpconnection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            tmpconnection.Open();
            if (tmpconnection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectString, tmpconnection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    int Id = Int32.Parse(reader["QUESTIONID"].ToString());
                    int answer = Int32.Parse(reader["ANSWER"].ToString());
                    pairs.Add(new Tuple<int, int>(Id, answer));

                    if (Id > answersList.Count - 1)
                        answersList.Add(new List<int>());
                    answersList[Id].Add(answer);
                }

                tmpconnection.Close();
            }

            return answersList;
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
                return false;

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
                return false;
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
                return false;
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

        public void DeleteQuestion(int questionId)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string result = "DELETE FROM QUESTIONS WHERE ID = " + questionId.ToString();
                SQLiteCommand deleteCommand = new SQLiteCommand(result, connection);
                deleteCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteQuestionId(int questionId)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string result = "DELETE FROM QUESTIONSIDS WHERE QUESTIONID = " + questionId.ToString();
                SQLiteCommand deleteCommand = new SQLiteCommand(result, connection);
                deleteCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteTest(int testId)
        {
            List<int> ids = SelectQuestionsIds(testId).ToList();
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
            }

            if (testId != _tests.Count - 1)
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

            List<int> questionsIds = SelectQuestionsIds(oldId).ToList();
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
            }
        }

        private string CreateUpdateTestString(int testId, List<string> TestData, List<int> NewTestQuestionsIds)
        {
            string result = "";
            for (var i = 0; i < TestData.Count; i++)
                TestData[i] = TestData[i].Replace("\'", "");

            result += "UPDATE TESTS SET NAME = '" + TestData[2] + "', LENGTH = " + Int32.Parse(TestData[1]) + ", " +
                "MAXPOINTS = " + Int32.Parse(TestData[0]) + " WHERE ID = " + testId.ToString();

            return result;
        }

        private List<string> CreateQuestionsIdsString(int id, List<int> NewTestQuestionsIds)
        {
            List<string> result = new List<string>();
            string init = "INSERT INTO QUESTIONSIDS (TESTID, QUESTIONID) VALUES (";
            for (int i = 0; i < NewTestQuestionsIds.Count; i++)
                result.Add(init + id.ToString() + ", " + NewTestQuestionsIds[i] + ")");

            return result;
        }

        public string CreateQuestionString(IQuestion question)
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
                for (int i = item; i < 5; i++)
                    result += (i == 4) ? "'')" : "'', ";

            return result;
        }

        private string CreateTestString(ITest test)
        {
            string result = "";
            result += "INSERT INTO TESTS (ID, NAME, LENGTH, MAXPOINTS, MULTICHECK) VALUES (";
            result += test.Id.ToString() + ", ";
            result += "'" + test.Name.ToString() + "', ";
            result += (test.Length.Minutes + test.Length.Hours * 60).ToString() + ", ";
            result += test.MaximumPoints.ToString() + ", ";
            result += (test.MultiCheck == true ? "1" : "0").ToString() + ")";

            return result;
        }
        
        private bool InsertHistory(IHistory history)
        {
            connection = new SQLiteConnection("Data Source=Tests.sqlite;Version=3;");
            connection.Open();

            if (connection.State == ConnectionState.Open)
            {
                string insertString = "INSERT INTO HISTORY (ID, TESTNAME, USERNAME, TIME, DURATION, SCORE) " +
                "VALUES (" + history.Id.ToString() + ", '" + history.Name.ToString() + "', '" + history.User.Name +
                "', '" + history.When.ToString("yyyy-MM-dd HH:mm:ss") + "', " + history.Duration.TotalSeconds.ToString() +
                ", " + history.Score.ToString() + ");";
                //'2016-11-13 10:00:00'

                SQLiteCommand insertCommand = new SQLiteCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();

                for (int i = 0; i < history.ChosenAnswers.Count; i++)
                {
                    for (int j = 0; j < history.ChosenAnswers[i].ChosenAnswers.Count; j++)
                    {
                        insertString =
                        "INSERT INTO CHECKEDANSWERS (HISTORYID, QUESTIONID, ANSWER) VALUES (" + history.Id.ToString() + ", " +
                        i.ToString() + ", " + history.ChosenAnswers[i].ChosenAnswers[j].ToString() + ");";

                        insertCommand = new SQLiteCommand(insertString, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                /*
                foreach (var question in history.Question)
                    InsertQuestion(question);

                InsertQuestionsIds(history.Id, history.QuestionsIds);
                */
                connection.Close();
                return true;
            }
            else
                return false;
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

        #endregion

        #region dao methods

        public void AddTest(ITest test)
        {
            _tests.Add(test);
        }

        public void AddQuestion(IQuestion question)
        {
            _questions.Add(question);
        }

        private Tuple<string, bool> ParseTuple(string answer)
        {
            char[] delimiterChar = { '_' };
            string[] splitAnswer = answer.Split(delimiterChar);
            bool isTrue = (splitAnswer[1] == "1") ? true : false;
            Tuple<string, bool> tupleAnswer = new Tuple<string, bool>(splitAnswer[0], isTrue);
            return tupleAnswer;
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
            questionString.Add(GetNewQuestionId().ToString()); //add ID to question
            IQuestion question = new DataObjects.Question(questionString);
            AddQuestion(question);
            //no insert until saving changes
            return question;
        }

        public int GetNewQuestionId()
        {
            //looking for lowest free id question 
            string questionString = "SELECT ID FROM QUESTIONS ORDER BY ID";
            List<int> ids = new List<int>(_temporaryIds);

            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(questionString, connection);
                SQLiteDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                    ids.Add(Int32.Parse(reader["ID"].ToString()));
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

        public ITest CreateNewTest()
        {
            return new DataObjects.Test();
        }

        public IAnsweredQuestion CreateNewAnsweredQuestion()
        {
            return new AnsweredQuestion();
        }

        public ITest CreateNewTest(List<string> TestData, List<int> NewTestQuestionsIds)
        {
            int id = GetNextHistoryId();
            ITest test = new DataObjects.Test(TestData, GetQuestionsByIds(NewTestQuestionsIds), NewTestQuestionsIds, id);
            AddTest(test); //only adds to DAO
            if (!InsertTest(test))
            { /*raise error*/ }
            return test;
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

        public void CreateNewHistory(IHistory history)
        {
            AddHistory(history);
            InsertHistory(history);
        }

        public int GetNextHistoryId()
        {
            int minId = 0;
            List<int> Ids = new List<int>();
            
            foreach (var test in _tests)
                Ids.Add(test.Id);

            foreach (var history in _histories)
                Ids.Add(history.Id);

            if (Ids.Count > 0)
            {
                Ids.Sort();
                minId = Ids.Max() + 1;
            }
           


            return minId;
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

        public IUser InitUser(string name, bool type)
        {
            return new User(name, type);
        }
        
        #endregion             
    

    

        public bool? IsEditor(string UserName)
        {
            IUser user = _users.Find(x => x.Name == UserName);

            if (user == null)
                return null;
            else
            {
                if (user.Type == true)
                    return true;
                else 
                    return false;
            }
            
        }
    }        
}
