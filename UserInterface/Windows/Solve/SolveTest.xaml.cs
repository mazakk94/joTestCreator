using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
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
using UserInterface.Helpers;
using UserInterface.ViewModel;

namespace UserInterface.Windows.Solve
{
    public partial class SolveTest : Window
    {
        public SolveTest()
        {
            InitializeComponent();     
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                switch (message.Type)
                {                
                    case WindowType.kSubmitTest:
                        var submitTestWindowVM = SimpleIoc.Default.GetInstance<SolveTestViewModel>();
                        //submitTestWindowVM.MyText = message.Argument;
                        var createNewTestWindow = new SubmitTest()
                        {
                            DataContext = submitTestWindowVM
                            
                        };
                        //submitTestWindowVM.ClearWindow();
                        //submitTestWindowVM.TestId = Int32.Parse(message.Argument);
                        
                        bool? result = createNewTestWindow.ShowDialog();
                        /*if (result.HasValue && result.Value)
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
                        */
                        break;                        
                }
                 
              });
        }
        

        #region methods

        void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}
