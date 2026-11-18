using ImageProcessing.Common;
using ImageProcessing.CoreClasses;
using ImageProcessing.Services;
using System.Windows.Media.Imaging;

namespace ImageProcessing.BlackAndWhite
{
    public class BlackAndWhiteViewModel : ImageToolViewModel
    {
        private BgraImage _source;
        private GrayscaleProcessor _processor;
        private bool _hasTransparency;

        // Степень обесцвечивания, сюда привязан слайдер
        private double _amount = 0;
        public double Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); Recalculate(); }
        }

        public BlackAndWhiteViewModel(IFileDialogService dialogs, IMessageService messages)
            : base(dialogs, messages) { }

        protected override bool ResultHasTransparency => _hasTransparency;

        protected override void OnImageLoaded(BitmapSource bitmap)
        {
            // Освобождаем старое до создания нового (экономим память)
            _processor = null;
            _source = null;

            _source = ImageService.ToBgra(bitmap);
            _processor = new GrayscaleProcessor(_source);
            _hasTransparency = _source.HasTransparency();
            Recalculate();
        }

        private void Recalculate()
        {
            if (_processor == null) return;
            ResultImage = ImageService.ToBitmapSource(_processor.Apply((int)Amount));
        }
    }
}