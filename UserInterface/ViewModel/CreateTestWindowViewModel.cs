using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UserInterface.ViewModel
{
    public class CreateTestWindowViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new DataAccessObject.DAO();

        private int _testId;
        public int TestId
        {
            get
            {
                return _testId;
            }

            set
            {
                if (_testId == value)
                {
                    return;
                }

                _testId = value;
                RaisePropertyChanged(() => TestId);
            }
        }

        private int _length;
        public int Length 
        {
            get
            {
                return _length;
            }

            set
            {
                if (_length == value)
                {
                    return;
                }

                _length = value;
                RaisePropertyChanged(() => Length);
            }
        }

        private string _name;
        public string Name 
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private int _questionsCount;
        public int QuestionsCount
        {
            get
            {
                return _questionsCount;
            }

            set
            {
                if (_questionsCount == value)
                {
                    return;
                }

                _questionsCount = value;
                RaisePropertyChanged(() => QuestionsCount);
            }
        }

        private ObservableCollection<IQuestion> _questions;
        public ObservableCollection<IQuestion> Questions
        {
            get { return _questions; }
            set
            {
                _questions = value;
                RaisePropertyChanged("Questions");
            }
        }


        private ObservableCollection<int> _questionsIds;
        public ObservableCollection<int> QuestionsIds
        {
            get
            {
                return _questionsIds;
            }

            set
            {
                if (_questionsIds == value)
                {
                    return;
                }

                _questionsIds = value;
                RaisePropertyChanged(() => QuestionsIds);
            }
        }   
        
        private int _selectedIndex;
        public int Index
        {
            get
            {                    
                return _selectedIndex;
            }

            set
            {
                if (_selectedIndex == value)
                {
                    //RaisePropertyChanged(() => Index);
                    return;                    
                }

                _selectedIndex = value;
                _answerList.Clear();
                //pobieram id pytania po indeksie
                //pobieram z listy pytań listę odpowiedzi szukając po id 
                if (_questions.Count > 0)
                {
                    int id = _questionsIds[_selectedIndex];
                    foreach (var answer in _questions[_selectedIndex].Answer)
                    {
                        _answerList.Add(answer.Item1);
                    }
                }
                

                //GetQuestions(_selectedIndex);
                //GetAllQuestions();
                //tutaj robie reset pytań i dodaje z aktualnego indeksu

                //Items[_selectedIndex] = (Convert.ToInt32(Items[_selectedIndex]) + 1).ToString();
                RaisePropertyChanged(() => Index);
            }
        }
        
        private ObservableCollection<string> _answerList;
        public ObservableCollection<string> AnswerList
        {
            get { return _answerList; }
            set
            {
                _answerList = value;
                RaisePropertyChanged("AnswerList");
            }
        }

        private int _maxPoints;
        public int MaxPoints
        {
            get
            {
                return _maxPoints;
            }

            set
            {
                if (_maxPoints == value)
                {
                    return;
                }

                _maxPoints = value;
                RaisePropertyChanged(() => MaxPoints);
            }
        }

        
        private string _myText;
        public string MyText
        {
            get
            {
                return _myText;
            }

            set
            {
                if (_myText == value)
                {
                    return;
                }

                _myText = value;
                RaisePropertyChanged(() => MyText);
            }
        }
        
        private List<string> _questionString;  //contains question content, points, answers to unpack
        public List<string> QuestionString
        {
            get
            {
                return _questionString;
            }

            set
            {
                if (_questionString == value)
                {
                    return;
                }

                _questionString = value;

                //rozpakowanie i wpisanie do list
                if(_questionString.Count > 6)
                    UnpackQuestionString();
                
                RaisePropertyChanged(() => Index);
                RaisePropertyChanged(() => Questions);
            }
        }

        public RelayCommand AddNewQuestionCommand { get; private set; }

        #endregion
        
        public CreateTestWindowViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });

            ClearWindow();

            #endregion

            AddNewQuestionCommand =
              new GalaSoft.MvvmLight.Command.RelayCommand(
                () =>
                Messenger.Default.Send<Helpers.OpenWindowMessage>(
                  new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kNewQuestion, Argument = QuestionsCount.ToString() }));

            Messenger.Default.Register<List<string>>(this, "question", s => QuestionString = s);
        }
        
        #region methods

        public void ClearWindow()
        {
            _maxPoints = 0;
            MaxPoints = 0;
            _length = 1;
            Length = 1;
            _name = "";
            Name = "";
            _questionsIds = new ObservableCollection<int>();
            QuestionsIds = new ObservableCollection<int>();
            _answerList = new ObservableCollection<string>();
            AnswerList = new ObservableCollection<string>();
            _questions = new ObservableCollection<IQuestion>();
            Questions = new ObservableCollection<IQuestion>();
            _questionString = new List<string>();
            QuestionString = new List<string>();
        }        

        private void SetMaxPoints(int points)
        {
            MaxPoints += points;
        }

        private void UnpackQuestionString()
        {
            IQuestion question = _dao.CreateNewQuestion(_questionString);
            _questions.Add(question);
            _questionsIds.Add(question.Id);
            SetMaxPoints(question.Points);
        }

        internal void RefreshDAO()
        {
            //pobieram na nowo z bazy wszystkie testy, pytania i ids
            _dao.InitDAO();

        }

        internal void LoadData()
        {            
            List<string> testData = _dao.GetTestData(this.TestId);
            this.Name = testData[0];
            this.Length = ParseTimeSpan(testData[1]);
            this.MaxPoints = Int32.Parse(testData[2]);
            this.QuestionsIds = new ObservableCollection<int>(_dao.SelectQuestionsIds(this.TestId));
            this.Questions = _dao.GetQuestionsByIds(this.QuestionsIds);
        }

        private int ParseTimeSpan(string timeSpan)
        {
            int minutes = 0;
            string[] splitted = timeSpan.Split(':');
            minutes += splitted[0] == "01" ? 60 : 0;
            minutes += Int32.Parse(splitted[1]);
            return minutes;
        }

        public void UpdateQuestions()
        {
            //here i'm deleting old questions and putting new ones in DataBase !
            //pobieram z bazy questionsids i usuwam stare pytania oraz te questionsids
            List<int> questionsIds = this._dao.SelectQuestionsIds(TestId);
            foreach (var id in questionsIds)
            {
                this._dao.DeleteQuestion(id);
                this._dao.DeleteQuestionId(id);
            }
            foreach (var question in Questions)
            {
                this._dao.InsertQuestion(question);
                this._dao.InsertQuestionId(TestId, question.Id);
            }

        }

        #endregion           
    }
}