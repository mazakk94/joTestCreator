using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace UserInterface.ViewModel
{
    public class HistoryViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new DataAccessObject.DAO();

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        private List<IHistory> _solvedTests;
        public List<IHistory> SolvedTests
        {
            get { return _solvedTests; }
            set
            {
                _solvedTests = value;
                RaisePropertyChanged("SolvedTests");
            }
        }

        private List<IQuestion> _questions;
        public List<IQuestion> Questions
        {
            get { return _questions; }
            set
            {
                _questions = value;
                RaisePropertyChanged("Questions");
            }
        }
        
        private int _testIndex;
        public int TestIndex
        {
            get { return _testIndex; }
            set
            {
                _testIndex = value;
                RaisePropertyChanged("TestIndex");
                UpdateQuestionsList();
            }
        }        

        private int _questionsIndex;
        public int QuestionIndex
        {
            get { return _questionsIndex; }
            set
            {
                _questionsIndex = value;
                RaisePropertyChanged("QuestionIndex");
            }
        }

        #endregion

        public HistoryViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });            

            UserName = "";
            SolvedTests = new List<IHistory>();
            Questions = new List<IQuestion>();            
            QuestionIndex = -1;
            TestIndex = -1;

            #endregion

        }

        #region methods

        public void RefreshDAO()
        {
            _dao.InitDAO();
        }

        public void FillSolvedTests()
        {
            SolvedTests.Clear();
            foreach (var history in _dao.GetAllHistories())
            {
                if (history.User.Name == UserName)
                    SolvedTests.Add(history);
            }
            //SolvedTests = _dao.GetAllHistories();
        }

        private void UpdateQuestionsList()
        {
            
            List<IQuestion> questions = new List<IQuestion>();
            if (TestIndex > -1 && SolvedTests.Count > 0)
            {
                bool changed = false;
                foreach (var question in SolvedTests[TestIndex].Question)
                {
                    questions.Add(question);
                    changed = true;
                }
                if (changed)
                    Questions = new List<IQuestion>(questions);
            }
        }

        #endregion
    }
}

