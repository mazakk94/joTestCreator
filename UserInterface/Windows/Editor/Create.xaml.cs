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

namespace UserInterface.Windows.Editor
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Window
    {
        public Create()
        {
            InitializeComponent();
            Closing += (sender, args) => DialogResult = chkBox.IsChecked;

            /*
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
                        var result = modalWindow.ShowDialog() ?? false;
                        Messenger.Default.Send(result ? "Accepted" : "Rejected");                           
                        break;
                
                    case WindowType.kNewTest:
                        modalWindowVM = SimpleIoc.Default.GetInstance<ModalWindowViewModel>();
                        modalWindowVM.MyText = message.Argument;
                        var createNewTestWindow = new Create()
                        {
                            DataContext = modalWindowVM
                        };
                        
                        result = createNewTestWindow.ShowDialog() ?? false;
                        Messenger.Default.Send(result ? "Accepted" : "Rejected");
                        var resultv2 = createNewTestWindow.maxPoints.Content.ToString();

                        //List<List<string>> questionList = GetQuestionDataFromDialog(createNewTestWindow);
                        
                        List<Tuple<string, string>> list = GetTestDataFromDialog(createNewTestWindow);


                        List<string> resultList = new List<string>();

                        resultList.Add(result ? "Accepted" : "Rejected");
                        resultList.Add(resultv2);
                        Messenger.Default.Send(resultv2, "token");
                        //Messenger.Default.Send(resultList, "list");
                        Messenger.Default.Send(list, "tuplelist");
                        //Messenger.Default.
                        break;


                    default:
                        var uniqueKey = System.Guid.NewGuid().ToString();
                        var nonModalWindowVM = SimpleIoc.Default.GetInstance<NonModalWindowViewModel>(uniqueKey);
                        nonModalWindowVM.MyText = message.Argument;
                        var nonModalWindow = new NonModalWindow()
                        {
                            DataContext = nonModalWindowVM
                        };
                        nonModalWindow.Closed += (sender, args) => SimpleIoc.Default.Unregister(uniqueKey);
                        nonModalWindow.Show();
                        break;
                }


              
                 
              });
             */





        }
    }
}
