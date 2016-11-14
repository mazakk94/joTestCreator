﻿using GalaSoft.MvvmLight.Ioc;
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

namespace UserInterface
{
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                  switch (message.Type)
                  {
                      case WindowType.kMenu:

                          var menuVM = SimpleIoc.Default.GetInstance<MainViewModel>();
                          var menuWindow = new Menu()
                          {
                              DataContext = menuVM
                          };

                          this.Close();
                          var result = menuWindow.ShowDialog();
                          
                          //if (result.HasValue && result.Value)
                          //{
                           //   result = true;
                              /*List<int> questionsIds = FillQuestionsIds(createEditTestWindow);
                              List<string> resultList = GetTestDataFromDialog(createEditTestWindow);

                              Messenger.Default.Send(questionsIds, "questionsIds");
                              Messenger.Default.Send(resultList, "testData");
                              modalWindowVM.UpdateQuestions();*/ 
                              //insert and delete from DB questions and Ids !
                       //   } 
                       //   else 
                      //    {
                       //      solveTestVM.Timer.Stop();
                        //  }

                          //string resultString;
                          //if (result == true) resultString = "Accepted";
                          //else resultString = "Rejected";
                          //Messenger.Default.Send(resultString);
                          
                          break;

                  }

              });
        
        }

    }
}
