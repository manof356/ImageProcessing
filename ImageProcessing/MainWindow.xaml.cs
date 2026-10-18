using ImageProcessing.BlackAndWhite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // функция кнопки "показать окно черно-белой обработки"
        private void BlackAndWhiteButton_Click(object sender, RoutedEventArgs e)
        {
            // Owner: дочернее окно закроется вместе с главным
            var window = new BlackAndWhiteWindow { Owner = this };
            window.Show(); // главное окно остаётся доступным
        }

        // показать окно справки "О программе"
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }
    }
}
