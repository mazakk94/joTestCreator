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

namespace UserInterface.Windows.Editor
{
    public partial class CreateSingleQuestion : Window
    {
        public CreateSingleQuestion()
        {
            InitializeComponent();
        }

        void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            bool closeWindow = true;
            if ((this.correct1.IsChecked == true && this.answer1.Text.Length > 0)
                || (this.correct2.IsChecked == true && this.answer2.Text.Length > 0)
                || (this.correct3.IsChecked == true && this.answer3.Text.Length > 0)
                || (this.correct4.IsChecked == true && this.answer4.Text.Length > 0)
                || (this.correct5.IsChecked == true && this.answer5.Text.Length > 0))
            {
                closeWindow = true;
            }
            else
            {
                closeWindow = false;
                MessageBox.Show("Check a correct answer!");
            }

            if (this.content.Text.Length == 0)
            {
                closeWindow = false;
                MessageBox.Show("Set content of question!");
            }
                
            
            if(closeWindow)
            this.DialogResult = true;
            
        }
    }
}
