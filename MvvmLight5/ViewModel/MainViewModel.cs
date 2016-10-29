using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using MvvmLight5.Helpers;
using MvvmLight5.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MvvmLight5.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {

        private ObservableCollection<TestViewModel> _tests;
        private ObservableCollection<IQuestion> _questions;
        private IDAO _dao;

        private RelayCommand _addTestCommand;


        /*

        public ObservableCollection<TestViewModel> Tests
        {
            get { return _tests; }
            set
            {
                _tests = value;
                RaisePropertyChanged("Tests");
            }
        }

        public ObservableCollection<IQuestion> Questions
        {
            get { return _questions; }
            set
            {
                _questions = value;
                RaisePropertyChanged("Questions");
            }
        }

        private String _testText;
        public String TestText
        {
            get { return _testText; }
            set
            {
                _testText = value;
                NotifyPropertyChanged("TestText");
                RaisePropertyChanged("TestText");
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void FillTestText()
        {
            _testText = "Init";
        }

        private void Print()
        {
            _testText += "String";
            NotifyPropertyChanged("TestText");
        }

        private void GetAllTests()
        {
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
                    return;
                }

                _selectedIndex = value;
                _questions.Clear();
                List<int> questionsIds = GetQuestionsIds(_selectedIndex);

                GetQuestions(questionsIds);
                //GetQuestions(_selectedIndex);
                //GetAllQuestions();
                //tutaj robie reset pytań i dodaje z aktualnego indeksu

                //Items[_selectedIndex] = (Convert.ToInt32(Items[_selectedIndex]) + 1).ToString();
                //RaisePropertyChanged(() => Index);
            }
        }



        private void GetQuestions(List<int> questionsIds)
        {
            foreach (var id in questionsIds)
            {
                _questions.Add(_dao.GetQuestion(id));
            }
        }

        private List<int> GetQuestionsIds(int _selectedIndex)
        {
            List<int> ids = new List<int>();
            foreach (var test in _dao.GetAllTests())
            {
                if (test.Id == _selectedIndex)
                {
                    ids = test.QuestionsIds;
                }
            }
            return ids;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand AddTestCommand
        {
            get { return _addTestCommand; }
        }

        private void AddTestToList()
        {
            ITest test = _dao.CreateNewTest();
            TestViewModel qvm = new TestViewModel(test);
            _dao.AddTest(test);
            Tests.Add(qvm);
            EditedTest = qvm;
        }

        private ICommand _saveNewTestCommand;

        public ICommand SaveNewTestCommand
        {
            get { return _saveNewTestCommand; }
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

        private RelayCommand _printTextCommand;

        public RelayCommand PrintTextCommand
        {
            get { return _printTextCommand; }
        }


        private RelayCommand _filterDataCommand;

        public RelayCommand FilterDataCommand
        {
            get { return _filterDataCommand; }
        }

        private RelayCommand _selectedItemChangedCommand;

        public RelayCommand SelectedItemChangedCommand
        {
            get { return _selectedItemChangedCommand; }
        }

        private ListCollectionView _view;

        private string _filterData;

        public string FilterData
        {
            get { return _filterData; }
            set
            {
                _filterData = value;
                RaisePropertyChanged("FilterData");
            }
        }
        */
        /*
        private void DoFilterData()
        {
            if (FilterData.Length > 0)
            {
                _view.Filter = (c) => ((testViewModel)c).Name.Contains(FilterData);
            }
            else
            {
                _view.Filter = null;
            }
        }
         * */

        private RelayCommand _grouptestsCommand;

        public RelayCommand GrouptestsCommand
        {
            get { return _grouptestsCommand; }
        }

        /*
        private void GroupByPrice()
        {
            _view.SortDescriptions.Add(new SortDescription("Price", 
                ListSortDirection.Ascending));
            _view.GroupDescriptions.Add(new PropertyGroupDescription("Price"));
        }
         * */



        private readonly IDataService _dataService;

        private string _someString;

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

        public string Result
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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MainViewModel(IDataService dataService)
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

            SomeString = "Some Placeholder Text - modify if you want";
            Result = "Output Placeholder Result";

            OpenModalDialog =
              new RelayCommand(
                () =>
                Messenger.Default.Send<OpenWindowMessage>(
                  new OpenWindowMessage() { Type = WindowType.kModal, Argument = SomeString }));
            OpenNonModalDialog =
              new RelayCommand(
                () =>
                Messenger.Default.Send<OpenWindowMessage>(
                  new OpenWindowMessage() { Type = WindowType.kNonModal, Argument = SomeString }));

            Messenger.Default.Register<string>(this, s => Result = s);
        }

        public RelayCommand OpenModalDialog { get; private set; }
        public RelayCommand OpenNonModalDialog { get; private set; }
    }
}