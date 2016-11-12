using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IDAO
    {
        IEnumerable<IProducer> GetAllProducers();
        IEnumerable<ICar> GettAllCars();

        //IEnumerable<IQuestion> GetAllQuestions();
        //IQuestion CreateNewQuestion();
        //void AddQuestion(IQuestion question);

        IEnumerable<ITest> GetAllTests();
        IEnumerable<IQuestion> GetAllQuestions();
        ITest CreateNewTest();
        IQuestion GetQuestion(int questionId);
        void AddTest(ITest test);
        //IEnumerable<IUser> GetAllUsers();
        //IUser CreateNewUser();

        IQuestion CreateNewQuestion(List<string> questionString);
        IQuestion CreateTempQuestion(List<string> _questionString);
        void AddCar(ICar car);

        
        ITest CreateNewTest(List<string> TestData, List<int> NewTestQuestionsIds);
        List<int> SelectQuestionsIds(int testId);
        List<string> GetTestData(int testId);
        ObservableCollection<IQuestion> GetQuestionsByIds(ObservableCollection<int> observableCollection);
        void DeleteQuestion(int id);
        void DeleteQuestionId(int id);
        void InsertQuestionId(int testId, int questionId);
        bool InsertQuestion(IQuestion question);

        void UpdateTest(int testId, List<string> TestData, List<int> NewTestQuestionsIds);
        void InitDAO();


        void DeleteTest(int testId);
    }
}
