using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;

namespace UserInterface.ViewModel
{
    public class ModalWindowViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

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


            AddNewQuestionCommand =
              new GalaSoft.MvvmLight.Command.RelayCommand(
                () =>
                Messenger.Default.Send<Helpers.OpenWindowMessage>(
                  new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kNewQuestion, Argument = QuestionsCount.ToString() }));

            //OpenModalDialog
        }

        public RelayCommand AddNewQuestionCommand { get; private set; }

    }
}