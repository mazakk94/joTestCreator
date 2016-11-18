using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wojtasik.Interfaces;

namespace Wojtasik.UserInterface.ViewModel
{
    public class CreateTestViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new Wojtasik.DataAccessObject.DAO();

        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                if (_result == value)
                    return;
                _result = value;
                RaisePropertyChanged(() => Result);
            }
        }

        private string _multiCheckVisible;
        public string MultiCheckVisible
        {
            get
            {
                return _multiCheckVisible;
            }

            set
            {
                if (_multiCheckVisible == value)
                    return;

                _multiCheckVisible = value;
                RaisePropertyChanged(() => MultiCheckVisible);
            }
        }

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
                RaisePropertyChanged("Name");
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


        private List<int> _questionsIds;
        public List<int> QuestionsIds
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
                AnswerList.Clear();
                //pobieram id pytania po indeksie
                //pobieram z listy pytań listę odpowiedzi szukając po id 
                List<string> list = new List<string>();
                if (_questions.Count > 0)
                {
                    int id = _questionsIds[_selectedIndex];
                    foreach (var answer in _questions[_selectedIndex].Answer)
                    {
                        list.Add(answer.Item1);
                    }
                }
                AnswerList = new List<String>(list);

                //GetQuestions(_selectedIndex);
                //GetAllQuestions();
                //tutaj robie reset pytań i dodaje z aktualnego indeksu

                //Items[_selectedIndex] = (Convert.ToInt32(Items[_selectedIndex]) + 1).ToString();
                RaisePropertyChanged(() => Index);
                RaisePropertyChanged("AnswerList");
            }
        }

        private List<string> _answerList;
        public List<string> AnswerList
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
                if (_questionString.Count > 7 && !_questionString[_questionString.Count].Contains("-")) // edit
                    UnpackQuestionString(true);
                else if(_questionString.Count > 6 && !_questionString[_questionString.Count].Contains("-")) // new
                    UnpackQuestionString(false);
                
                RaisePropertyChanged(() => Index);
                RaisePropertyChanged(() => Questions);
            }
        }

        public RelayCommand AddNewQuestionCommand { get; private set; }
        public RelayCommand EditQuestionCommand { get; private set; }
        public RelayCommand DeleteQuestionCommand { get; private set; }

        #endregion
        
        public CreateTestViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });

            ClearWindow();

            #endregion

            AddNewQuestionCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => CreateAndSaveQuestion());

            EditQuestionCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => EditAndSaveQuestion());

            DeleteQuestionCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => DeleteQuestion());

            Messenger.Default.Register<List<string>>(this, "question", 
                s => { QuestionString.Clear(); foreach (var item in s) QuestionString.Add(item); });
            
            Messenger.Default.Register<string>(this, "result", s => Result = s);
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
            _questionsIds = new List<int>();
            QuestionsIds = new List<int>();
            _answerList = new List<string>();
            AnswerList = new List<string>();
            _questions = new ObservableCollection<IQuestion>();
            Questions = new ObservableCollection<IQuestion>();
            _questionString = new List<string>();
            QuestionString = new List<string>();
            _result = "";
            Result = "";
            MultiCheckVisible = "Hidden";
        }        

        private void SetMaxPoints(int points)
        {
            MaxPoints += points;
        }

        private void UnpackQuestionString(bool isEdit)
        {
            if (isEdit == false)
            {
                IQuestion question = _dao.CreateNewQuestion(_questionString);
                _questions.Add(question);
                _questionsIds.Add(question.Id);
                SetMaxPoints(question.Points);
            }
            else // question Edit           
            {   //  no new question in dao, no new id, replacement of question.points 
                int selectedIndex = _selectedIndex;
                _questionString.Add(_questionsIds[selectedIndex].ToString());
                SetMaxPoints(-_questions[selectedIndex].Points);
                IQuestion question = _dao.CreateTempQuestion(_questionString);
                _questions[selectedIndex] = question;
                SetMaxPoints(_questions[selectedIndex].Points);
            }
        }

        internal void RefreshDAO()
        {
            //pobieram na nowo z bazy wszystkie testy, pytania i ids
            _dao.InitDAO();
        }

        internal void LoadData()
        {            
            List<string> testData = _dao.GetTestData(this.TestId).ToList();
            this.Name = testData[0];
            this.Length = ParseTimeSpan(testData[1]);
            this.QuestionsIds = new List<int>(_dao.SelectQuestionsIds(this.TestId));
            List<IQuestion> tmpQuestions = _dao.GetQuestionsByIds(this.QuestionsIds).ToList();
            foreach (var question in tmpQuestions) Questions.Add(question);
            this.MaxPoints = CalculateMaxPoints(Questions);
        }

        internal void SetAnswers(bool notDeleted)
        {
            if (Questions.Count > 0)
            {
                int index = 0;
                if (_selectedIndex >= 0 && notDeleted)
                    index = _selectedIndex;

                List<string> answers = new List<string>();

                foreach (var answer in Questions[index].Answer)
                    if (answer.Item1 != null)
                        answers.Add(answer.Item1);
                    

                AnswerList = new List<string>(answers);
                RaisePropertyChanged(() => AnswerList);
                
            }
                
        }

        private int CalculateMaxPoints(ObservableCollection<IQuestion> Questions)
        {
            int maxpoints = 0;
            foreach(var question in Questions)
            {
                maxpoints += question.Points;
            }
            return maxpoints;
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
            List<int> questionsIds = this._dao.SelectQuestionsIds(TestId).ToList();
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
        
        private void CreateAndSaveQuestion()
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kNewQuestion, Argument = QuestionsCount.ToString() });
            if (QuestionString.Count > 6)
                UnpackQuestionString(false);
            else
                ;

            SetAnswers(false);
        }

        private void EditAndSaveQuestion()
        {
            if (_selectedIndex >= 0)
            {
                PrepareQuestionString();
                string arg = ParseQuestionString(QuestionString);
                Messenger.Default.Send<Helpers.OpenWindowMessage>(
                      new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kEditQuestion, Argument = arg });
                if (QuestionString.Count > 6 && Result == "Accepted")
                    UnpackQuestionString(true);
                else
                    ;
            }
            SetAnswers(true);
            //trzeba nadpisać istniejące pytanie -> info siedzi w questionstring, powinien sam sie odpakować            
        }

        private void DeleteQuestion()
        {
            if(_selectedIndex >= 0)
            {
                SetMaxPoints(-_questions[_selectedIndex].Points);
                QuestionsIds.RemoveAt(_selectedIndex);
                Questions.RemoveAt(_selectedIndex);
                ObservableCollection<IQuestion> questions = new ObservableCollection<IQuestion>();
                foreach (var question in Questions) questions.Add(question); ;
                Questions.Clear();
                Questions = new ObservableCollection<IQuestion>(questions);
                RaisePropertyChanged(() => Questions);
            }
            SetAnswers(false);
        }

        private void PrepareQuestionString()
        {
            /* * 0 - content, 1 .. 5 - answer, 6 - points, 7 - id*/

            QuestionString.Clear();
            QuestionString.Add(_questions[_selectedIndex].Content);

            for (int i = 0; i < 5; i++)
            {
                if (_questions[_selectedIndex].Answer.Count > i)
                    QuestionString.Add(UnParseAnswer(_questions[_selectedIndex].Answer[i]));
                else
                    QuestionString.Add("");
            }

            QuestionString.Add(_questions[_selectedIndex].Points.ToString());
            QuestionString.Add("");
        }

        private string UnParseAnswer(Tuple<string, bool> answer)
        {
            return answer.Item1 + ((answer.Item2 == true) ? "_1" : "_0");
        }

        private string ParseQuestionString(List<string> questionString)
        {
            string parsed = "";
            for (int i = 0; i < questionString.Count - 1; i++)
            {
                parsed += (i == questionString.Count - 2) ? questionString[i] : (questionString[i] + ";");
            }
            return parsed;
        }

        internal void ShowMultiCheck()
        {
            MultiCheckVisible = "Visible";
        }

        internal void HideMultiCheck()
        {
            MultiCheckVisible = "Hidden";
        }

        #endregion        
    
        
    
    
        
    }
}