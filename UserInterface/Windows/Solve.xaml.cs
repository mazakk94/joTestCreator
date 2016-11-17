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
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Wojtasik.UserInterface.Helpers;
using Wojtasik.UserInterface.ViewModel;
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
                          string[] parsedMessage = message.Argument.Split('+');
                          solveTestVM.ClearWindow();
                          solveTestVM.Test.Id = Int32.Parse(parsedMessage[0]);
                          solveTestVM.UserName = parsedMessage[1];
                          solveTestVM.RefreshDAO();
                          solveTestVM.LoadData();

                          if(solveTestVM.FillWindow())
                          {
                              var result = solveTestWindow.ShowDialog();

                              if (result.HasValue && result.Value)
                                  result = true;
                              else
                                  solveTestVM.Timer.Stop();
                          }
                          else
                          {
                              solveTestVM.Timer.Stop();
                              MessageBox.Show("This test does not have any question!");
                          }
                          
                          break;
                  }

              });
        }

        #region methods
        
        #endregion


    }
}
