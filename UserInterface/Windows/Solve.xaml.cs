﻿using System;
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
                      #region solve test
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
                      #endregion

                      #region solve single test
                      case WindowType.kSolveSingleTest:

                          var solveSingleTestVM = SimpleIoc.Default.GetInstance<SolveTestViewModel>();
                          var solveSingleTestWindow = new SolveSingleTest()
                          {
                              DataContext = solveSingleTestVM
                          };
                          parsedMessage = message.Argument.Split('+');
                          solveSingleTestVM.ClearWindow();
                          solveSingleTestVM.Test.Id = Int32.Parse(parsedMessage[0]);
                          solveSingleTestVM.UserName = parsedMessage[1];
                          solveSingleTestVM.RefreshDAO();
                          solveSingleTestVM.LoadData();

                          if (solveSingleTestVM.FillWindow())
                          {
                              var result = solveSingleTestWindow.ShowDialog();

                              if (result.HasValue && result.Value)
                                  result = true;
                              else
                                  solveSingleTestVM.Timer.Stop();
                          }
                          else
                          {
                              solveSingleTestVM.Timer.Stop();
                              MessageBox.Show("This test does not have any question!");
                          }

                          break;
                      #endregion
                  }

              });
        }

        #region methods
        
        #endregion


    }
}
