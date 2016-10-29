using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MvvmLight5.Helpers;
using MvvmLight5.Model;

namespace MvvmLight5.ViewModel {
  /// <summary>
  /// This class contains properties that the main View can data bind to.
  /// <para>
  /// See http://www.galasoft.ch/mvvm
  /// </para>
  /// </summary>
  public class MainViewModel : ViewModelBase {
      
    private readonly IDataService _dataService;

    private string _someString;

    public string SomeString {
      get {
        return _someString;
      }

      set {
        if (_someString == value) {
          return;
        }

        _someString = value;
        RaisePropertyChanged(() => SomeString);
      }
    }

    private string _result;

    public string Result {
      get {
        return _result;
      }

      set {
        if (_result == value) {
          return;
        }

        _result = value;
        RaisePropertyChanged(() => Result);
      }
    }

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public MainViewModel(IDataService dataService) {
      _dataService = dataService;
      _dataService.GetData(
        (item, error) => {
          if (error != null) {
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
            new OpenWindowMessage() {Type = WindowType.kModal, Argument = SomeString}));
      OpenNonModalDialog =
        new RelayCommand(
          () =>
          Messenger.Default.Send<OpenWindowMessage>(
            new OpenWindowMessage() {Type = WindowType.kNonModal, Argument = SomeString}));

      Messenger.Default.Register<string>(this, s => Result = s);
    }

    public RelayCommand OpenModalDialog { get; private set; }
    public RelayCommand OpenNonModalDialog { get; private set; }
  }
}
