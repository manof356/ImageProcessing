using ImageProcessing.Services;
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

namespace ImageProcessing.BlackAndWhite
{
    /// <summary>
    /// Логика взаимодействия для BlackAndWhiteWindow.xaml
    /// </summary>
    public partial class BlackAndWhiteWindow : Window
    {
        public BlackAndWhiteWindow()
        {
            InitializeComponent();
            DataContext = new BlackAndWhiteViewModel(new FileDialogService(), new MessageService());
        }
    }
}
