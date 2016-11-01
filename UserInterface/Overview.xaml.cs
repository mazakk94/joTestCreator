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
        }

        private List<List<string>> GetQuestionDataFromDialog(Create window)
        {
            throw new NotImplementedException();
        }

        List<Tuple<string, string>> GetTestDataFromDialog(Create window)
        {       // name, value
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            list.Add(new Tuple<string, string>("MaxPoints", window.maxPoints.Content.ToString()));
            list.Add(new Tuple<string, string>("Length", window.Length.Content.ToString()));
            
            return list;
        }



    }

}