using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wojtasik.Interfaces
{
    public interface IDAO
    {
        IEnumerable<ITest> GetAllTests();
        IEnumerable<IQuestion> GetAllQuestions();
        IEnumerable<IUser> GetAllUsers();
        IEnumerable<IHistory> GetAllHistories();        
        IEnumerable<int> SelectQuestionsIds(int testId);
        IEnumerable<string> GetTestData(int testId);
        IEnumerable<IQuestion> GetQuestionsByIds(List<int> questionsIds);        
        IQuestion GetQuestion(int questionId);        
        IQuestion CreateNewQuestion(List<string> questionString);
        IQuestion CreateTempQuestion(List<string> _questionString);        
        ITest CreateNewTest(List<string> TestData, List<int> NewTestQuestionsIds);
        ITest GetTest(int p);
        ITest CreateNewTest();
        IHistory CreateNewHistory();
        IHistory CreateNewHistory(int testId);
        IUser GetCurrentUser();        
        IUser CreateNewUser(string name, bool type);
        IUser InitUser(string name, bool type);
        IAnsweredQuestion CreateNewAnsweredQuestion();
        List<List<int>> SelectCheckedAnswers(int historyId);
        string CreateQuestionString(IQuestion question);
        void DeleteQuestion(int id);
        void DeleteQuestionId(int id);
        void InsertQuestionId(int testId, int questionId);        
        void UpdateTest(int testId, List<string> TestData, List<int> NewTestQuestionsIds);
        void InitDAO();
        void AddQuestion(IQuestion question);
        void AddTest(ITest test);
        void DeleteTest(int testId);                
        void CreateNewHistory(IHistory BeingSolved);        
        void SetCurrentUser(string UserName);
        int GetNextHistoryId();
        int GetNewQuestionId();
        bool InsertQuestion(IQuestion question);
        bool? IsEditor(string UserName);
    }
}
