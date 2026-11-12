using ImageProcessing.BlackAndWhite;
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

namespace ImageProcessing.Dithering
{
    /// <summary>
    /// Логика взаимодействия для DitheringWindow.xaml
    /// </summary>
    public partial class DitheringWindow : Window
    {
        public DitheringWindow()
        {
            InitializeComponent();
            DataContext = new BlackAndWhiteViewModel(new FileDialogService(), new MessageService());
        }
    }
}
