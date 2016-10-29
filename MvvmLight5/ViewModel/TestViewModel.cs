using Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight5.ViewModel
{
    class TestViewModel
    {

        
        private ITest _test;
       // private List<IQuestion> _questions;

        public TestViewModel(ITest test)
        {
            _test = test;
        }

       /* public TestViewModel(IQuestion question)
        {
            
        }*/

        public int Id
        {
            get { return _test.Id; }
            set
            {
                _test.Id = value;
                //RaisePropertyChanged("testId");
            }
        }        
        
        public string Name
        {
            get { return _test.Name; }
            set
            {
                _test.Name = value;
                //RaisePropertyChanged("Coor");
            }
        }

        public TimeSpan Length
        {
            get { return _test.Length; }
            set
            {
                _test.Length = value;
                //RaisePropertyChanged("Coor");
            }
        }

        public int MaximumPoints
        {
            get { return _test.MaximumPoints; }
            set
            {
                _test.MaximumPoints = value;
                //RaisePropertyChanged("testId");
            }
        }        

        public List<IQuestion> Question
        {
            get { return _test.Question; }
            set
            {
                _test.Question = value;
                //RaisePropertyChanged("Producent");
            }
        }


        

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                //Validate();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasErrors
        {
            get { throw new NotImplementedException(); }
        }
    }
}