using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageProcessing.wpfCommonParts
{
    /// <summary>
    /// Логика взаимодействия для BeforeAfterImages.xaml
    /// </summary>
    public partial class BeforeAfterImages : UserControl
    {
        public BeforeAfterImages()
        {
            InitializeComponent();
        }

        // Картинка слева ("До")
        public ImageSource OriginalImage
        {
            get => (ImageSource)GetValue(OriginalImageProperty);
            set => SetValue(OriginalImageProperty, value);
        }
        public static readonly DependencyProperty OriginalImageProperty =
            DependencyProperty.Register(nameof(OriginalImage), typeof(ImageSource),
                                        typeof(BeforeAfterImages), new PropertyMetadata(null));

        // Картинка справа ("После")
        public ImageSource ResultImage
        {
            get => (ImageSource)GetValue(ResultImageProperty);
            set => SetValue(ResultImageProperty, value);
        }
        public static readonly DependencyProperty ResultImageProperty =
            DependencyProperty.Register(nameof(ResultImage), typeof(ImageSource),
                                        typeof(BeforeAfterImages), new PropertyMetadata(null));

        // Как масштабировать при показе (для дизеринга нужен NearestNeighbor)
        public BitmapScalingMode ScalingMode
        {
            get => (BitmapScalingMode)GetValue(ScalingModeProperty);
            set => SetValue(ScalingModeProperty, value);
        }
        public static readonly DependencyProperty ScalingModeProperty =
            DependencyProperty.Register(nameof(ScalingMode), typeof(BitmapScalingMode),
                                        typeof(BeforeAfterImages),
                                        new PropertyMetadata(BitmapScalingMode.Unspecified));
    }
}