using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UserInterface.ViewModel
{
    public class ModalWindowViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private IDAO _dao = new DataAccessObject.DAO();

        private int _questionCount;

        public int QuestionsCount
        {
            get
            {
                return _questionCount;
            }

            set
            {
                if (_questionCount == value)
                {
                    return;
                }

                _questionCount = value;
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

        private List<string> _questionString;

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

        
        public ModalWindowViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });
            _maxPoints = 0;
            MaxPoints = 0;
            _questionsIds = new ObservableCollection<int>();
            QuestionsIds = new ObservableCollection<int>();
            _answerList = new ObservableCollection<string>();
            AnswerList = new ObservableCollection<string>();
            _questions = new ObservableCollection<IQuestion>();
            Questions = new ObservableCollection<IQuestion>();
            _questionString = new List<string>();
            QuestionString = new List<string>();

            AddNewQuestionCommand =
              new GalaSoft.MvvmLight.Command.RelayCommand(
                () =>
                Messenger.Default.Send<Helpers.OpenWindowMessage>(
                  new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kNewQuestion, Argument = QuestionsCount.ToString() }));

            Messenger.Default.Register<List<string>>(this, "question", s => QuestionString = s);
            //OpenModalDialog
        }

        public RelayCommand AddNewQuestionCommand { get; private set; }

    }
}