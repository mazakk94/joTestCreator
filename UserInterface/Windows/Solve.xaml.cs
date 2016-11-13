using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Interfaces;


using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using UserInterface.Helpers;
using UserInterface.ViewModel;
using UserInterface.Windows.Solve;
using System.Collections.ObjectModel;

namespace UserInterface
{
    public partial class Solve : Window
    {
        public Solve()
        {
            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                  switch (message.Type)
                  {
                      case WindowType.kSolveTest:

                          var solveTestVM = SimpleIoc.Default.GetInstance<SolveTestViewModel>();
                          var solveTestWindow = new SolveTest()
                          {
                              DataContext = solveTestVM
                          };
                          solveTestVM.ClearWindow();
                          solveTestVM.Test.Id = Int32.Parse(message.Argument);                          
                          solveTestVM.RefreshDAO();
                          solveTestVM.LoadData();

                          var result = solveTestWindow.ShowDialog();
                          
                          if (result.HasValue && result.Value)
                          {
                              result = true;
                              /*List<int> questionsIds = FillQuestionsIds(createEditTestWindow);
                              List<string> resultList = GetTestDataFromDialog(createEditTestWindow);

                              Messenger.Default.Send(questionsIds, "questionsIds");
                              Messenger.Default.Send(resultList, "testData");
                              modalWindowVM.UpdateQuestions();*/ 
                              //insert and delete from DB questions and Ids !
                          } 
                          else 
                          {
                              solveTestVM.Timer.Stop();
                          }

                          //string resultString;
                          //if (result == true) resultString = "Accepted";
                          //else resultString = "Rejected";
                          //Messenger.Default.Send(resultString);
                          
                          break;

                  }

              });
        }

        #region methods
        
        #endregion


    }
}
