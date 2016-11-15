using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UserInterface.Helpers;
using UserInterface.ViewModel;

namespace UserInterface
{
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<OpenWindowMessage>(
              this,
              message =>
              {
                  switch (message.Type)
                  {
                      case WindowType.kMenu:
                          var menuVM = SimpleIoc.Default.GetInstance<MainViewModel>();
                          var menuWindow = new Menu()
                          {
                              DataContext = menuVM
                          };
                          menuVM.IsEditorVisible = menuVM.LoginUser(this.userName.Text, UserOrEditor()) ? "Visible" : "Hidden";                          
                          this.Close();
                          var result = menuWindow.ShowDialog();                         
                          
                          break;
                  }

              });
        
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            this.Title = textBox.Text +
            "[Length = " + textBox.Text.Length.ToString() + "]";
        }



        bool UserOrEditor()
        {
            return (this.RadioEditor.IsChecked ?? false) && (!this.RadioUser.IsChecked ?? false);
        }

    }
}
