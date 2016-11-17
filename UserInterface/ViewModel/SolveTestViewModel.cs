using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System.Linq;
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

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

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

        private TimeSpan _testDuration;
        public TimeSpan TestDuration 
        {
            get
            {
                return _testDuration;
            }
            set
            {
                _testDuration = value;
                RaisePropertyChanged(() => TestDuration);
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

        private DateTime _timeStarted;
        public DateTime TimeStarted
        {
            get
            {
                return _timeStarted;
            }
            set
            {
                _timeStarted = value;
                RaisePropertyChanged(() => TimeStarted);
            }
        }

        private string _scoreinfo;
        public string ScoreInfo
        {
            get
            {
                return _scoreinfo;
            }
            set
            {
                _scoreinfo = value;
                RaisePropertyChanged(() => ScoreInfo);
            }
        }

        public DispatcherTimer Timer;

        #endregion

        #region commands definitions

        public RelayCommand PreviousQuestionCommand { get; private set; }
        public RelayCommand NextQuestionCommand { get; private set; }
        public RelayCommand SubmitTestCommand { get; private set; }

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

            SubmitTestCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => SubmitTest());  

            #endregion

            //ClearWindow();
        }

        #region methods

        private void SubmitTest()
        {
            SaveCheckedAnswers(Id);
            Timer.Stop();
            BeingSolved.Score = CalculateScore();
            ScoreInfo = BeingSolved.Score.ToString() + " / " + Test.MaximumPoints.ToString();
            CreateHistoryOfTest();
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kSubmitTest, Argument = TestDuration.ToString() });
        }

        private int CalculateScore()
        {
            int score = 0;

            for (int i = 0; i < Test.Question.Count; i++)
            {
                //iterate all 
                //if checkbox is checked and test.question[i].answer
                bool correct = true;
                for (int j = 0; j < Test.Question[i].Answer.Count; j++)
                {
                    if (Test.Question[i].Answer[j].Item2 && !BeingSolved.ChosenAnswers[i].ChosenAnswers.Contains(j)) //true
                        correct = false;
                    else if (!Test.Question[i].Answer[j].Item2 && BeingSolved.ChosenAnswers[i].ChosenAnswers.Contains(j)) //false
                        correct = false;
                }
            
                if (correct)
                    score += Test.Question[i].Points;
                
            }

            return score;
        }

        private TimeSpan RoundDuration(TimeSpan variable)
        {
            return new TimeSpan(variable.Hours, variable.Minutes, variable.Seconds);
        }

        private void CreateHistoryOfTest()
        {
            BeingSolved.Id = _dao.GetNextHistoryId();
            BeingSolved.When = DateTime.Now;
            TestDuration = RoundDuration(DateTime.Now - TimeStarted); //nedded to be tested a little more
            BeingSolved.Duration = TestDuration;
            _dao.SetCurrentUser(UserName);
            BeingSolved.User = _dao.GetCurrentUser();
            _dao.CreateNewHistory(BeingSolved);
        }                

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Timeleft = Timeleft - new TimeSpan(0, 0, 1);
            if (Timeleft == new TimeSpan(0, 0, 0))
            {
                SubmitTest();
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
            TimeStarted = DateTime.Now;
        }

        internal void RefreshDAO()
        {
            //pobieram na nowo z bazy wszystkie testy, pytania i ids
            _dao.InitDAO();
        }

        internal void LoadData()
        {
            this.Test = _dao.GetTest(Test.Id);
            this.Test.Question = _dao.GetQuestionsByIds(this.Test.QuestionsIds).ToList();
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
            if (isInRange(Id + direction))
            {
                SaveCheckedAnswers(Id);

                Id += direction;
                Content = Test.Question[Id].Content;
                QuestionInfo = (Id + 1).ToString() + " of " + Test.Question.Count.ToString();

                Answer.Clear();
                Answer = new List<string>();
                for (int i = 0; i < 5; i++)
                    Answer.Add((Test.Question[Id].Answer.Count > i) ? Test.Question[Id].Answer[i].Item1 : "");
                RaisePropertyChanged(() => Answer);


                CheckBox.Clear();
                for (int i = 0; i < 5; i++)
                    CheckBox.Add(BeingSolved.ChosenAnswers[Id].ChosenAnswers.Contains(i) ? true : false);
                RaisePropertyChanged(() => CheckBox);            
            }            
        }

        private bool isInRange(int nextId)
        {
            if (nextId < 0)
                return false;
            else if (nextId >= Test.Question.Count)
                return false;
            return true;
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