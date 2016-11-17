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
using Wojtasik.UserInterface.Helpers;
using Wojtasik.UserInterface.ViewModel;

namespace UserInterface.Windows.Editor
{
    public partial class CreateTest : Window
    {
        public CreateTest()
        {
            InitializeComponent();
            //Closing += (s, e) => ViewModelLocator.Cleanup();
            //Closing += (sender, args) => DialogResult = chkBox.IsChecked;

            
             Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                switch (message.Type)
                {
                    case WindowType.kNewQuestion:
                        //var modalWindowVM = SimpleIoc.Default.GetInstance<CreateTestViewModel>();
                        var modalWindowVM = SimpleIoc.Default.GetInstance<CreateQuestionViewModel>();
                        //modalWindowVM.MyText = message.Argument;
                        var modalWindow = new CreateQuestion()
                        {
                            DataContext = modalWindowVM
                        };
                        modalWindowVM.ClearWindow();
                        bool? result = modalWindow.ShowDialog();// ?? false;

                        if (result.HasValue && result.Value)
                        {
                            List<string> resultList = GetQuestionDataFromDialog(modalWindow);
                            Messenger.Default.Send(resultList, "question");
                        }

                        string resultString;
                        if (result == true) resultString = "Accepted";
                        else resultString = "Rejected";

                        Messenger.Default.Send(resultString);                        
                        break;

                    case WindowType.kEditQuestion:
                        
                        var EditQuestionWindowVM = SimpleIoc.Default.GetInstance<CreateQuestionViewModel>();
                        List<string> unparsed = UnParseQuestionString(message.Argument);
                        
                        EditQuestionWindowVM.QuestionString = new List<string>();
                        foreach(var item in unparsed)
                        {
                            EditQuestionWindowVM.QuestionString.Add(item);
                        }
                        //EditQuestionWindowVM.QuestionString = unparsed; //questionString
                        
                        var EditQuestionWindow = new CreateQuestion()
                        {
                            DataContext = EditQuestionWindowVM
                        };
                        
                        EditQuestionWindowVM.FillDialog(); //fill dialog with questionString
                        result = EditQuestionWindow.ShowDialog();

                        if (result.HasValue && result.Value)
                        {
                            List<string> resultList = GetQuestionDataFromDialog(EditQuestionWindow);
                            Messenger.Default.Send(resultList, "question"); 
                        }

                        if (result == true) resultString = "Accepted";
                        else resultString = "Rejected";

                        Messenger.Default.Send(resultString);
                        
                        break;                   
                }             
              });
             
        }

        private List<string> UnParseQuestionString(string parsed)
        {
            string[] unParsed = parsed.Split(';');
            List<string> list = new List<string>(unParsed);
            list[list.Count-1] += "-";
            return list;
        }

        private List<string> GetQuestionDataFromDialog(CreateQuestion window)
        {
            List<string> result = new List<string>();
            // 0 - content, 1 - answer1, 2 - answer2 .. 5 - answer5, 6 - points, 7 - id
            //string correct;            
            result.Add(window.content.Text);
            result.Add(window.answer1.Text + ((bool)(window.answer1.Text.Length > 0) ? ((bool)window.correct1.IsChecked ? "_1" : "_0") : ""));
            result.Add(window.answer2.Text + ((bool)(window.answer2.Text.Length > 0) ? ((bool)window.correct2.IsChecked ? "_1" : "_0") : ""));
            result.Add(window.answer3.Text + ((bool)(window.answer3.Text.Length > 0) ? ((bool)window.correct3.IsChecked ? "_1" : "_0") : ""));
            result.Add(window.answer4.Text + ((bool)(window.answer4.Text.Length > 0) ? ((bool)window.correct4.IsChecked ? "_1" : "_0") : ""));
            result.Add(window.answer5.Text + ((bool)(window.answer5.Text.Length > 0) ? ((bool)window.correct5.IsChecked ? "_1" : "_0") : ""));
            result.Add(window.points.Content.ToString());

            return result;
        }

        void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


    }
}
