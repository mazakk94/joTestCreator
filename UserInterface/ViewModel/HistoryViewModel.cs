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
    public class HistoryViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new DataAccessObject.DAO();

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        private List<IHistory> _solvedTests;
        public List<IHistory> SolvedTests
        {
            get { return _solvedTests; }
            set
            {
                _solvedTests = value;
                RaisePropertyChanged("SolvedTests");
            }
        }

        private int _testIndex;
        public int TestIndex
        {
            get { return _testIndex; }
            set
            {
                _testIndex = value;
                RaisePropertyChanged("TestIndex");
            }
        }

        #endregion

        public HistoryViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });
            

            UserName = "";
            SolvedTests = new List<IHistory>();
            

            #endregion

        }

        #region methods

        public void RefreshDAO()
        {
            _dao.InitDAO();
        }

        public void FillSolvedTests()
        {
            SolvedTests.Clear();
            foreach (var history in _dao.GetAllHistories())
            {
                if (history.User.Name == UserName)
                    SolvedTests.Add(history);
            }
            //SolvedTests = _dao.GetAllHistories();
        }

        #endregion
    }
}

