﻿using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MvvmLight5.Helpers;
using MvvmLight5.ViewModel;

namespace MvvmLight5 {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow:Window {
    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow() {
      InitializeComponent();
      Closing += (s,e) => ViewModelLocator.Cleanup();
      Messenger.Default.Register<OpenWindowMessage>(
        this,
        message => {
          if (message.Type == WindowType.kModal) {
            var modalWindowVM = SimpleIoc.Default.GetInstance<ModalWindowViewModel>();
            modalWindowVM.MyText = message.Argument;
            var modalWindow = new ModalWindow() {
              DataContext = modalWindowVM
            };
            var result = modalWindow.ShowDialog() ?? false;
            Messenger.Default.Send(result ? "Accepted" : "Rejected");
          } else {
            var uniqueKey = System.Guid.NewGuid().ToString();
            var nonModalWindowVM = SimpleIoc.Default.GetInstance<NonModalWindowViewModel>(uniqueKey);
            nonModalWindowVM.MyText = message.Argument;
            var nonModalWindow = new NonModalWindow() {
              DataContext = nonModalWindowVM
            };
            nonModalWindow.Closed += (sender, args) => SimpleIoc.Default.Unregister(uniqueKey);
            nonModalWindow.Show();
          }
        });
    }
  }
}