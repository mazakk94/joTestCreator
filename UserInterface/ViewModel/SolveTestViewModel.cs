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
    public class SolveTestViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new DataAccessObject.DAO();

        private ITest _test;
        public ITest Test
        {
            get
            {
                return _test;
            }
            set
            {
                _test = value;
                RaisePropertyChanged(() => Test);
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged(() => Content);
            }
        }

        private List<string> _answer;
        public List<string> Answer
        {
            get
            {
                return _answer;
            }
            set
            {
                _answer = value;
                RaisePropertyChanged(() => Answer);
            }
        }

        private TimeSpan _timeleft;
        public TimeSpan Timeleft
        {
            get
            {
                return _timeleft;
            }
            set
            {
                _timeleft = value;
                RaisePropertyChanged(() => Timeleft);
            }
        }

        public DispatcherTimer Timer;

        #endregion

        public SolveTestViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });
                       
            #endregion

            
            
            //ClearWindow();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Timeleft = Timeleft - new TimeSpan(0, 0, 1);
            if (Timeleft == new TimeSpan(0,0,0))
            {
                Timer.Stop();
                Content = "TIME OVER";
                //here ends the test and it now has to count points for test, create and save history of test
            }
        }

        
        #region methods

        public void ClearWindow()
        {
            //TestName = "Test Name";
            Answer = new List<string>();
            for(var i = 0; i < 5; i++) Answer.Add(i.ToString());
            Content = "Question content?";
                        
            Timer = new DispatcherTimer();
            Timer.Tick += DispatcherTimer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
            Timeleft = new TimeSpan(0, 0, 10);
            Test = _dao.CreateNewTest();
            Test.Name = "Test Name";
                     

        }

        internal void RefreshDAO()
        {
            //pobieram na nowo z bazy wszystkie testy, pytania i ids
            _dao.InitDAO();
        }

        internal void LoadData()
        {
            this.Test = _dao.GetTest(Test.Id);
            this.Test.Question = _dao.GetQuestionsByIds(this.Test.QuestionsIds);
        }
        
        #endregion

    }
}