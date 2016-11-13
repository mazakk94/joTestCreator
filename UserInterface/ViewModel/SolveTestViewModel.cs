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

        private IHistory _beingSolved;
        public IHistory BeingSolved
        {
            get
            {
                return _beingSolved;
            }
            set
            {
                _beingSolved = value;
                RaisePropertyChanged(() => BeingSolved);
            }
        }

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

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string _questionInfo;
        public string QuestionInfo
        {
            get
            {
                return _questionInfo;
            }
            set
            {
                _questionInfo = value;
                RaisePropertyChanged(() => QuestionInfo);
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

        public List<bool> _checkBox;
        public List<bool> CheckBox
        {
            get
            {
                return _checkBox;
            }
            set
            {
                _checkBox = value;
                RaisePropertyChanged(() => CheckBox);
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

        #region commands definitions

        public RelayCommand PreviousQuestionCommand { get; private set; }
        public RelayCommand NextQuestionCommand { get; private set; }

        #endregion

        public SolveTestViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });
                       
            #endregion

            #region methods initialization

            PreviousQuestionCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => UpdateWindow(-1));

            NextQuestionCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => UpdateWindow(1));            

            #endregion

            //ClearWindow();
        }
                       
        #region methods

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Timeleft = Timeleft - new TimeSpan(0, 0, 1);
            if (Timeleft == new TimeSpan(0, 0, 0))
            {
                Timer.Stop();
                Content = "TIME OVER";
                //here ends the test and it now has to count points for test, create and save history of test
            }
        }

        public void ClearWindow()
        {
            //TestName = "Test Name";
            Id = 0;            
            Test = _dao.CreateNewTest();
            Test.Name = "";
            BeingSolved = _dao.CreateNewHistory();
            Answer = new List<string>();
                for (var i = 0; i < 5; i++) Answer.Add(i.ToString());
            CheckBox = new List<bool>();            
                for (var i = 0; i < 5; i++) CheckBox.Add(false);
            Content = "";
            QuestionInfo = "";                        
            Timer = new DispatcherTimer();
            Timer.Tick += DispatcherTimer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
            Timeleft = new TimeSpan(0, 0, 10);            
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
            this.BeingSolved = _dao.CreateNewHistory(Test.Id);
        }     

        internal void FillWindow()
        {
            Content = Test.Question[0].Content;
            QuestionInfo = (Id+1).ToString() + " of " + Test.Question.Count.ToString();
            Timeleft = Test.Length;
            Answer.Clear();
            for(int i = 0; i < 5; i++)
            {
                if (Test.Question[Id].Answer.Count > i)
                    Answer.Add(Test.Question[Id].Answer[i].Item1);
                else
                    Answer.Add("");
            }
        }

        private void UpdateWindow(int direction)
        {

            SaveCheckedAnswers(Id);

            Id += direction;
            Content = Test.Question[Id].Content;
            QuestionInfo = (Id + 1).ToString() + " of " + Test.Question.Count.ToString();

            Answer.Clear();
            for (int i = 0; i < 5; i++)
                 Answer.Add((Test.Question[Id].Answer.Count > i) ? Test.Question[Id].Answer[i].Item1 : "");

            CheckBox.Clear();            
            //for (int i = 0; i < 5; i++)
             //   CheckBox.Add(BeingSolved.ChosenAnswers[Id].ChosenAnswers.Contains(i) ? true : false);
            
        }

        private void SaveCheckedAnswers(int Id)
        {
            BeingSolved.ChosenAnswers[Id].ChosenAnswers.Clear();
            for (int i = 0; i < 5; i++)
                if (CheckBox[i] == true) 
                    BeingSolved.ChosenAnswers[Id].ChosenAnswers.Add(i);
        }

        #endregion

    }
}