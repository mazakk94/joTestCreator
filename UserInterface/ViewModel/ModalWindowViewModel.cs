using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MvvmLight5.Model;

namespace MvvmLight5.ViewModel {
  public class ModalWindowViewModel:ViewModelBase {
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

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public ModalWindowViewModel(IDataService dataService) {
      _dataService = dataService;
      _dataService.GetData(
          (item,error) => {
            if(error != null) {
              // Report error here
              return;
            }
          });


      //OpenModalDialog
    }

  }
}