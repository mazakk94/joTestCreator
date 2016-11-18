using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wojtasik.Interfaces;

namespace Wojtasik.UserInterface.ViewModel
{
    class CreateQuestionViewModel : ViewModelBase
    {
        #region variables definitions

        private readonly IDataService _dataService;
        private IDAO _dao = new Wojtasik.DataAccessObject.DAO();

        private bool _isMultiCheck;
        public bool IsMultiCheck
        {
            get
            {
                return _isMultiCheck;
            }
            set
            {
                if (_isMultiCheck == value)
                {
                    return;
                }
                _isMultiCheck = value;
                RaisePropertyChanged(() => IsMultiCheck);
            }
        }

        private List<string> _answers;
        public List<string> Answers
        {
            get { return _answers; }
            set
            {
                _answers = value;
                RaisePropertyChanged("Answers");
            }
        }

        private List<bool> _correctAnswer;
        public List<bool> CorrectAnswer
        {
            get { return _correctAnswer; }
            set
            {
                _correctAnswer = value;
                RaisePropertyChanged("CorrectAnswer");
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
                /*
                //rozpakowanie i wpisanie do list
                if (_questionString.Count > 7 && !_questionString[_questionString.Count].Contains("-")) // edit
                    UnpackQuestionString(true);
                else if (_questionString.Count > 6 && !_questionString[_questionString.Count].Contains("-")) // new
                    UnpackQuestionString(false);
                */
                RaisePropertyChanged(() => QuestionString);
            }
        }

        #endregion

        public CreateQuestionViewModel(IDataService dataService)
        {
            #region variables initialization

            _dataService = dataService;
            _dataService.GetData((item, error) => { if (error != null) return; });

            ClearWindow();

            #endregion
        }

        #region methods

        public void ClearWindow()
        {
            _length = 1;
            Length = 1;
            _name = "";
            Name = "";
            _questionString = new List<string>();
            QuestionString = new List<string>();
            _answers = new List<string>();
            Answers = new List<string>();
            _correctAnswer = new List<bool>();
            CorrectAnswer = new List<bool>();
            _isMultiCheck = false;
            IsMultiCheck = false;
        }

        internal void RefreshDAO()
        {
            //pobieram na nowo z bazy wszystkie testy, pytania i ids
            _dao.InitDAO();
        }

        private int ParseTimeSpan(string timeSpan)
        {
            int minutes = 0;
            string[] splitted = timeSpan.Split(':');
            minutes += splitted[0] == "01" ? 60 : 0;
            minutes += Int32.Parse(splitted[1]);
            return minutes;
        }
        
        internal void FillDialog()
        {
            /* * 0 - content, 1 .. 5 - answer, 6 - points, 7 - id*/
            Name = QuestionString[0];
            for (int i = 0; i < 5; i++)
            {
                string[] splitted = QuestionString[i + 1].Split('_');
                if (splitted.Length == 2)
                {
                    Answers.Add(splitted[0]);
                    CorrectAnswer.Add(splitted[1] == "1" ? true : false);
                }
                else
                {
                    Answers.Add("");
                    CorrectAnswer.Add(false);
                }
            }

            Length = Int32.Parse(QuestionString[6].Replace("-", ""));

        }

        #endregion
    }
}
