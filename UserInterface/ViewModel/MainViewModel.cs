using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;


namespace UserInterface.ViewModel
{

    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {

        #region variables

        private IDAO _dao;
        private ListCollectionView _view;
        private readonly IDataService _dataService;

        //  public event PropertyChangedEventHandler PropertyChanged;         ALE NAROBIŁ INBY

        private IUser _user;
        public IUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged("User");
            }
        }

        private string _isEditorVisible;
        public string IsEditorVisible
        {
            get { return _isEditorVisible; }
            set
            {
                _isEditorVisible = value;
                RaisePropertyChanged("IsEditorVisible");
            }
        }

        private ObservableCollection<TestViewModel> _tests;
        public ObservableCollection<TestViewModel> Tests
        {
            get { return _tests; }
            set
            {
                _tests = value;
                RaisePropertyChanged("Tests");
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

        private ObservableCollection<IQuestion> _resultQuestionList;
        public ObservableCollection<IQuestion> ResultQuestionList
        {
            get { return _resultQuestionList; }
            set
            {
                _resultQuestionList = value;
                RaisePropertyChanged("Questions");
            }
        }
        
        private List<int> _newTestQuestionsIds;
        public List<int> NewTestQuestionsIds //needed to know which questions view in listbox
        {
            get
            {
                return _newTestQuestionsIds;
            }
            set
            {
                _newTestQuestionsIds = value;
                RaisePropertyChanged("NewTestQuestionsIds");
            }
        }

        private List<string> _testData;
        public List<string> TestData //contains data about test - [0]maxpoints, [1]length, [2]name
        {
            get
            {
                return _testData;
            }
            set
            {
                _testData = value;
                RaisePropertyChanged("TestData");
            }
        }

        private string _maxPoints;
        public string MaxPoints 
        {
            get
            {
                return _maxPoints;
            }
            set
            {
                _maxPoints = value;
                RaisePropertyChanged("MaxPoints");
            }
        }

        private string _length;
        public string Length 
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
                RaisePropertyChanged("Length");
            }
        }

        private int _selectedIndex;
        public int Index //selected index to know which answers show in listbox next to questions list
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                if (_selectedIndex == value)
                {
                    return;
                }

                _selectedIndex = value;
                _questions.Clear();
                //pobieram z testu liste numerów pytań i pobieram te pytania z DAO
                //GetTestId(_selectedIndex);
                List<int> questionsIds = GetQuestionsIds(_selectedIndex);
                _length = (Tests[_selectedIndex].Length.Hours*60 + Tests[_selectedIndex].Length.Minutes).ToString();
                _maxPoints = Tests[_selectedIndex].MaximumPoints.ToString();
                GetQuestions(questionsIds);

                RaisePropertyChanged(() => Index);
                RaisePropertyChanged(() => Questions);
                RaisePropertyChanged(() => MaxPoints);
                RaisePropertyChanged(() => Length);
            }
        }        

        private TestViewModel _editedTest;
        public TestViewModel EditedTest
        {
            get { return _editedTest; }
            set
            {
                _editedTest = value;
                RaisePropertyChanged("EditedTest");
            }
        }

        private string _someString; //string that is passed to dialog window
        public string SomeString
        {
            get
            {
                return _someString;
            }

            set
            {
                if (_someString == value)
                {
                    return;
                }

                _someString = value;
                RaisePropertyChanged(() => SomeString);
            }
        }

        private string _result;
        public string Result //accepted or rejected
        {
            get
            {
                return _result;
            }

            set
            {
                if (_result == value)
                {
                    return;
                }

                _result = value;
                RaisePropertyChanged(() => Result);
            }
        }

        #endregion

        public MainViewModel(IDataService dataService)
        {
            #region variables initializations

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });

            SomeString = "Some Placeholder Text - modify if you want";
            Result = "Output Placeholder Result";

            _tests = new ObservableCollection<TestViewModel>();
            _questions = new ObservableCollection<IQuestion>();
            _resultQuestionList = new ObservableCollection<IQuestion>();
            _newTestQuestionsIds = new List<int>();
            _testData = new List<string>();
            _dao = new DataAccessObject.DAO();
            _view = (ListCollectionView)CollectionViewSource.GetDefaultView(_tests);            
            GetAllTests();
            GetAllQuestions();
            //_userName = _dao.GetCurrentUser().Name;
            //_isEditorVisible = _dao.GetCurrentUser().Type ? "Visible" : "Hidden";

            NewTestQuestionsIds = new List<int>();
            TestData = new List<string>();

            #endregion 

            #region commands initializations

            SignInRegisterCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => SignInRegister());

            OpenSolveCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => OpenSolve());

            OpenHistoryCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => OpenHistory());

            OpenEditorCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => OpenEditor());

            CreateNewTestCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => CreateAndSaveTest());

            EditTestCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => EditTest(GetSelectedTestId()));

            DeleteTestCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => DeleteTest(GetSelectedTestId()));

            SolveTestCommand = new GalaSoft.MvvmLight.Command.RelayCommand(
                () => SolveTest());
            
            Messenger.Default.Register<string>(this, s => Result = s);
            Messenger.Default.Register<List<int>>(this, "questionsIds", s => NewTestQuestionsIds = s);
            Messenger.Default.Register<List<string>>(this, "testData", s => TestData = s);
            
            _addTestCommand = new MyRelayCommand(param => this.AddTestToList());
            _saveNewTestCommand = new MyRelayCommand(param => this.SaveTest(),
                                                  param => this.CanSaveTest());
            #endregion
        }
                
        #region methods

        public bool LoginUser(string name, bool type)
        {
            bool login = false;
            User = _dao.InitUser(name, type);

            foreach (var user in _dao.GetAllUsers())
                if (User.Name == user.Name)
                    login = true;

            if (login)
                _dao.SetCurrentUser(User.Name);
            else
            {
                User = _dao.CreateNewUser(name, type);
                _dao.SetCurrentUser(User.Name);
            }
            User = _dao.GetCurrentUser();
            return User.Type;
            
        }

        private void OpenEditor()
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kEditor, Argument = "" });
        }

        private void OpenHistory()
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                  new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kHistory, Argument = User.Name });
        }

        private void OpenSolve()
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kSolve, Argument = User.Name });
        }

        private void SignInRegister()
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kMenu, Argument = "" });
        }

        private void SolveTest()
        {
            if (Index > 0)
                Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kSolveTest, Argument = _selectedIndex.ToString()+"+"+User.Name });
        }

        private void DeleteTest(int testId)
        {
            _dao.DeleteTest(testId);
            _dao.InitDAO();
            GetAllTests();
            //tu mozna dodac jeszcze działania po zamknieciu okna dodawania testu

        }
               
        private int GetSelectedTestId()
        {
            return _selectedIndex;
        }

        private void GetAllTests()
        {
            _tests.Clear();     //czy na pewno?
            foreach (var c in _dao.GetAllTests())
            {
                _tests.Add(new TestViewModel(c));
            }
        }
        
        private void GetAllQuestions()
        {
            foreach (var c in _dao.GetAllQuestions())
            {
                _questions.Add(c);
            }
        }

        private void GetQuestions(List<int> questionsIds)
        {
            foreach (var id in questionsIds)
            {
                _questions.Add(_dao.GetQuestion(id));
            }
        }

        private List<int> GetQuestionsIds(int testId)
        {
            List<int> ids = new List<int>();
            foreach (var test in _dao.GetAllTests())
            {
                if (test.Id == testId)
                {
                    ids = test.QuestionsIds;
                }
            }
            return ids;
        }

        private void AddTestToList()
        {
            ITest test = _dao.CreateNewTest();
            TestViewModel qvm = new TestViewModel(test);
            _dao.AddTest(test);
            Tests.Add(qvm);
            EditedTest = qvm;
        }

        private void SaveTest()
        {
            _tests.Add(_editedTest);
        }

        private bool CanSaveTest()
        {
            if (EditedTest == null)
                return false;

            if (EditedTest.HasErrors)
                return false;

            return true;
        }

        private void EditTest(int testId)
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kEditTest, Argument = testId.ToString() });

            if (Result == "Accepted")
            {
                _dao.UpdateTest(testId, TestData, NewTestQuestionsIds);
                _dao.InitDAO();
            }
                
            GetAllTests();
            //tu mozna dodac jeszcze działania po zamknieciu okna dodawania testu

        }

        private void CreateAndSaveTest()
        {
            Messenger.Default.Send<Helpers.OpenWindowMessage>(
                   new Helpers.OpenWindowMessage() { Type = Helpers.WindowType.kNewTest, Argument = _tests.Count.ToString() });

            if(Result == "Accepted")
                _dao.CreateNewTest(TestData, NewTestQuestionsIds);
            GetAllTests();
            //tu mozna dodac jeszcze działania po zamknieciu okna dodawania testu

        }
        #endregion

        #region commands
        
        public RelayCommand SignInRegisterCommand { get; private set; }
        public RelayCommand OpenSolveCommand { get; private set; }
        public RelayCommand OpenHistoryCommand { get; private set; }
        public RelayCommand OpenEditorCommand { get; private set; }
        public RelayCommand SolveTestCommand { get; private set; }
        public RelayCommand CreateNewTestCommand { get; private set; }
        public RelayCommand EditTestCommand { get; private set; }
        public RelayCommand DeleteTestCommand { get; private set; }
        public RelayCommand OpenModalDialog { get; private set; }
        public RelayCommand OpenNonModalDialog { get; private set; }

        private MyRelayCommand _addTestCommand;
        public ICommand AddTestCommand
        {
            get { return _addTestCommand; }
        }

        private ICommand _saveNewTestCommand;
        public ICommand SaveNewTestCommand
        {
            get { return _saveNewTestCommand; }
        }

        private MyRelayCommand _filterDataCommand;
        public MyRelayCommand FilterDataCommand
        {
            get { return _filterDataCommand; }
        }

        private MyRelayCommand _selectedItemChangedCommand;
        public MyRelayCommand SelectedItemChangedCommand
        {
            get { return _selectedItemChangedCommand; }
        }

        #endregion

    }
}