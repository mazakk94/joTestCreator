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
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
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
                        var modalWindowVM = SimpleIoc.Default.GetInstance<ModalWindowViewModel>();
                        modalWindowVM.MyText = message.Argument;
                        var modalWindow = new ModalWindow()
                        {
                            DataContext = modalWindowVM
                        };
                        var modalResult = modalWindow.ShowDialog() ?? false;
                        Messenger.Default.Send(modalResult ? "Accepted" : "Rejected");                           
                        break;
                
                    case WindowType.kNewTest:
                        modalWindowVM = SimpleIoc.Default.GetInstance<ModalWindowViewModel>();
                        modalWindowVM.MyText = message.Argument;
                        var createNewTestWindow = new CreateTest()
                        {
                            DataContext = modalWindowVM
                        };

                        #region comment
                        /*
                        result = createNewTestWindow.ShowDialog() ?? false;
                        Messenger.Default.Send(result ? "Accepted" : "Rejected");
                        var resultv2 = createNewTestWindow.maxPoints.Content.ToString();
                        List<List<string>> questionList = GetQuestionDataFromDialog(createNewTestWindow);                        
                        List<Tuple<string, string>> list = GetTestDataFromDialog(createNewTestWindow);
                        resultList.Add(result ? "Accepted" : "Rejected");
                        //resultList.Add(resultv2);
                        //Messenger.Default.Send(resultv2, "token");
                        //Messenger.Default.Send(resultList, "list");
                        //Messenger.Default.Send(list, "tuplelist");
                        
                        //Messenger.Default.
                        */
                        #endregion

                        //bool? result = createNewTestWindow.ShowDialog();// ?? false;
                        
                        //bool result = createNewTestWindow.ShowDialog() ?? true;
                        bool? result = createNewTestWindow.ShowDialog();
                        //if (result.HasValue && result.Value)
                        //if (result)
                        if (result.HasValue && result.Value)
                        //if(createNewTestWindow.DialogResult.HasValue && createNewTestWindow.DialogResult.Value)
                        {
                            result = true;
                            List<int> questionsIds = FillQuestionsIds(createNewTestWindow);      
                            List<string> resultList = GetTestDataFromDialog(createNewTestWindow);

                            Messenger.Default.Send(questionsIds, "questionsIds");
                            Messenger.Default.Send(resultList, "testData");
                        }
                        
                        string resultString;
                        if (result == true) resultString = "Accepted";
                        else resultString = "Rejected";
                        Messenger.Default.Send(resultString);
                        
                        break;      

                }
                 
              });
        }

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


    }

}