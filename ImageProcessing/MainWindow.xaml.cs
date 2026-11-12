using ImageProcessing.BlackAndWhite;
using ImageProcessing.Dithering;
using System.Windows;

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

        // функция кнопки "показать окно Dithering обработки"
        private void DitheringButton_Click(object sender, RoutedEventArgs e)
        {
            // Owner: дочернее окно закроется вместе с главным
            var window = new DitheringWindow { Owner = this };
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
