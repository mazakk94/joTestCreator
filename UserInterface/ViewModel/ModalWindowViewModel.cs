using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System.Collections.Generic;

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
                UnpackQuestionString();

                RaisePropertyChanged(() => QuestionString);
            }
        }

        private void UnpackQuestionString()
        { // 0 - content, 1 .. 5 - answer, 6 - points, 7 - id
            //int id = 
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