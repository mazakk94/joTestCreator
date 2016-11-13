using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UserInterface.ViewModel
{
    public class SolveTestViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new DataAccessObject.DAO();

        private string _testName;
        public string TestName
        {
            get
            {
                return _testName;
            }
            set
            {
                _testName = value;
                RaisePropertyChanged(() => TestName);
            }
        }

        #endregion

        public SolveTestViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });
            
            _testName = "";
            TestName = "";

            #endregion

            ClearWindow();
        } 
       
        
        #region methods

        public void ClearWindow()
        {
            //_testName = "";
            TestName = "Test Name";
        }        

        internal void RefreshDAO()
        {
            //pobieram na nowo z bazy wszystkie testy, pytania i ids
            _dao.InitDAO();
        }
        
        #endregion

    }
}