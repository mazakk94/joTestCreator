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
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                  switch (message.Type)
                  {
                      case WindowType.kSolve:

                          var solveVM = SimpleIoc.Default.GetInstance<MainViewModel>();
                          var solveWindow = new Solve()
                          {
                              DataContext = solveVM
                          };

                          var result = solveWindow.ShowDialog();

                          if (result.HasValue && result.Value)
                              result = true;

                          break;

                      case WindowType.kHistory:

                          var historyVM = SimpleIoc.Default.GetInstance<HistoryViewModel>();
                          var historyWindow = new History()
                          {
                              DataContext = historyVM
                          };
                          historyVM.RefreshDAO();
                          historyVM.UserName = message.Argument;
                          historyVM.FillSolvedTests();

                          result = historyWindow.ShowDialog();

                          if (result.HasValue && result.Value)
                              result = true;

                          break;

                      case WindowType.kEditor:

                          var editorVM = SimpleIoc.Default.GetInstance<MainViewModel>();
                          var editorWindow = new Editor()
                          {
                              DataContext = editorVM
                          };

                          result = editorWindow.ShowDialog();

                          if (result.HasValue && result.Value)
                              result = true;

                          break;
                  }

              });
        }

        #region methods
        
        #endregion


    }
}
