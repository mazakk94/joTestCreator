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
using UserInterface.Windows.Editor;
using System.Collections.ObjectModel;

namespace UserInterface
{
    public partial class Editor : Window
    {
        public Editor()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                switch (message.Type)
                {

                    #region new test
                    case WindowType.kNewTest:
                        var modalWindowVM = SimpleIoc.Default.GetInstance<CreateTestViewModel>();
                        modalWindowVM.MyText = message.Argument;
                        var createNewTestWindow = new CreateTest()
                        {
                            DataContext = modalWindowVM                            
                        };
                        
                        modalWindowVM.ClearWindow();
                        modalWindowVM.TestId = Int32.Parse(message.Argument);
                        modalWindowVM.IsMultiCheck = "true";

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
                    #endregion

                    #region new single test
                    case WindowType.kNewSingleTest:
                        var createNewSingleTestVM = SimpleIoc.Default.GetInstance<CreateTestViewModel>();
                        createNewSingleTestVM.MyText = message.Argument;
                        var createNewSingleTestWindow = new CreateTest()
                        {
                            DataContext = createNewSingleTestVM
                        };

                        createNewSingleTestVM.ClearWindow();
                        createNewSingleTestVM.TestId = Int32.Parse(message.Argument);
                        createNewSingleTestVM.IsMultiCheck = "false";

                        result = createNewSingleTestWindow.ShowDialog();

                        if (result.HasValue && result.Value)
                        {
                            result = true;
                            List<int> questionsIds = FillQuestionsIds(createNewSingleTestWindow);
                            List<string> resultList = GetTestDataFromDialog(createNewSingleTestWindow);

                            Messenger.Default.Send(questionsIds, "questionsIds");
                            Messenger.Default.Send(resultList, "testData");
                            createNewSingleTestVM.UpdateQuestions();
                        }

                        //string resultString;
                        if (result == true) resultString = "Accepted";
                        else resultString = "Rejected";
                        Messenger.Default.Send(resultString);

                        break;
                    #endregion

                    #region edit test
                    case WindowType.kEditTest:
                        
                        modalWindowVM = SimpleIoc.Default.GetInstance<CreateTestViewModel>();                        
                        var createEditTestWindow = new CreateTest()
                        {
                            DataContext = modalWindowVM
                        };
                        modalWindowVM.ClearWindow();
                        modalWindowVM.TestId = Int32.Parse(message.Argument);
                        modalWindowVM.RefreshDAO();
                        modalWindowVM.LoadData();
                        modalWindowVM.SetAnswers(false);
                        modalWindowVM.SetMultiCheck(modalWindowVM.TestId);

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
                    #endregion
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
            list.Add(window.Multi.IsChecked ?? false ? "1" : "0");
            
            return list;
        }

        #endregion
    }
}