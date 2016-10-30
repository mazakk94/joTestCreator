using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MvvmLight5.Model;

namespace MvvmLight5.ViewModel {
  public class NonModalWindowViewModel:ViewModelBase {
    private readonly IDataService _dataService;


    private string _myText;

    public string MyText {
      get {
        return _myText;
      }

      set {
        if(_myText == value) {
          return;
        }

        _myText = value;
        RaisePropertyChanged(() => MyText);
      }
    }

    private bool _checkMe;

    public bool CheckMe {
      get {
        return _checkMe;
      }

      set {
        if(_checkMe == value) {
          return;
        }

        _checkMe = value;
        Messenger.Default.Send(_checkMe ? "Accepted" : "Rejected");
        RaisePropertyChanged(() => CheckMe);
      }
    }

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public NonModalWindowViewModel(IDataService dataService) {
      _dataService = dataService;
      _dataService.GetData(
          (item,error) => {
            if(error != null) {
              // Report error here
              return;
            }
          });


    }


  }
}