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
using UserInterface.Windows.Editor;
using System.Collections.ObjectModel;

namespace UserInterface
{
    public partial class Overview : Window
    {
        public Overview()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                switch (message.Type)
                {
                    case WindowType.kModal:                          
                        var modalWindowVM = SimpleIoc.Default.GetInstance<CreateTestWindowViewModel>();
                        modalWindowVM.MyText = message.Argument;
                        var modalWindow = new ModalWindow()
                        {
                            DataContext = modalWindowVM
                        };
                        var modalResult = modalWindow.ShowDialog() ?? false;
                        Messenger.Default.Send(modalResult ? "Accepted" : "Rejected");                           
                        break;
                
                    case WindowType.kNewTest:
                        modalWindowVM = SimpleIoc.Default.GetInstance<CreateTestWindowViewModel>();
                        modalWindowVM.MyText = message.Argument;
                        var createNewTestWindow = new CreateTest()
                        {
                            DataContext = modalWindowVM
                            
                        };
                        modalWindowVM.ClearWindow();
                        
                        bool? result = createNewTestWindow.ShowDialog();
                        if (result.HasValue && result.Value)
                        {
                            result = true;
                            List<int> questionsIds = FillQuestionsIds(createNewTestWindow);      
                            List<string> resultList = GetTestDataFromDialog(createNewTestWindow);

                            Messenger.Default.Send(questionsIds, "questionsIds");
                            Messenger.Default.Send(resultList, "testData");
                            modalWindowVM.UpdateQuestions();
                        }
                        
                        string resultString;
                        if (result == true) resultString = "Accepted";
                        else resultString = "Rejected";
                        Messenger.Default.Send(resultString);
                        
                        break;

                    case WindowType.kEditTest:

                        //TODO
                        modalWindowVM = SimpleIoc.Default.GetInstance<CreateTestWindowViewModel>();                        
                        var createEditTestWindow = new CreateTest()
                        {
                            DataContext = modalWindowVM
                        };
                        modalWindowVM.ClearWindow();
                        modalWindowVM.TestId = Int32.Parse(message.Argument);
                        modalWindowVM.RefreshDAO();
                        modalWindowVM.LoadData();

                        result = createEditTestWindow.ShowDialog();
                        if (result.HasValue && result.Value)
                        {
                            result = true;
                            List<int> questionsIds = FillQuestionsIds(createEditTestWindow);
                            List<string> resultList = GetTestDataFromDialog(createEditTestWindow);

                            Messenger.Default.Send(questionsIds, "questionsIds");
                            Messenger.Default.Send(resultList, "testData");
                            modalWindowVM.UpdateQuestions(); //insert and delete from DB questions and Ids !
                        }

                        //resultString;
                        if (result == true) resultString = "Accepted";
                        else resultString = "Rejected";
                        Messenger.Default.Send(resultString);

                        break;      

                }
                 
              });
        }

        #region methods

        private List<int> FillQuestionsIds(CreateTest window )
        {
            List<int> list = new List<int>();
            foreach (var item in window.questionsIds.Items)
            {
                list.Add(Int32.Parse(item.ToString()));
            }
            return list;            
        }
        
        List<string> GetTestDataFromDialog(CreateTest window)
        {       // name, value
            List<string> list = new List<string>();
            list.Add(window.maxPoints.Content.ToString());
            list.Add(window.Length.Content.ToString());
            list.Add(window.Name.Text.ToString());
            
            return list;
        }

        #endregion
    }
}