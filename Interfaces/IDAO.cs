using System;
using System.Collections.Generic;
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

        ICar CreateNewCar();
        void AddCar(ICar car);

    }
}
