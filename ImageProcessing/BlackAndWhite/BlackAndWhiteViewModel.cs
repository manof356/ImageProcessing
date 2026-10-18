using ImageProcessing.Common;
using ImageProcessing.CoreClasses;
using ImageProcessing.Services;
using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageProcessing.BlackAndWhite
{
    internal class BlackAndWhiteViewModel : ViewModelBase
    {
        private readonly IFileDialogService _dialogs;
        private readonly IMessageService _messages;
        private bool _hasTransparency;
        private string _sourcePath;   // путь к загруженному оригиналу
        private BitmapSource _originalImage;
        public BitmapSource OriginalImage
        {
            get => _originalImage;
            set { _originalImage = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand ExportCommand { get; }

        public BlackAndWhiteViewModel(IFileDialogService dialogs, IMessageService messages)
        {
            _dialogs = dialogs;          // сервис приходит снаружи
            _messages = messages;
            LoadCommand = new RelayCommand(Load);
            // Второй параметр: кнопка неактивна, пока результата нет.
            ExportCommand = new RelayCommand(Export, () => ResultImage != null);

        }
        
        private BgraImage _source;                // оригинал в байтах
        private GrayscaleProcessor _processor;    // считает серый один раз при загрузке

        private BitmapSource _resultImage;
        public BitmapSource ResultImage
        {
            get => _resultImage;
            set { _resultImage = value; OnPropertyChanged(); }
        }

        // Степень обесцвечивания, сюда привязан слайдер
        private double _amount = 0;
        public double Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); Recalculate(); }
        }

        private void Recalculate()
        {
            if (_processor == null) return;               // фото ещё не загружено
            BgraImage result = _processor.Apply((int)Amount);
            ResultImage = ImageService.ToBitmapSource(result);
        }

        private void Load()
        {
            string path = _dialogs.OpenImage();
            if (path == null) return;                     // отмена
            _sourcePath = path;

            BitmapSource bitmap = ImageService.LoadBitmap(path);
            OriginalImage = bitmap;                       // слева "До"

            _source = ImageService.ToBgra(bitmap);        // картинка в байты
            _hasTransparency = _source.HasTransparency();
            _processor = new GrayscaleProcessor(_source); // серый считается один раз
            Recalculate();                                // справа "После"
        }

        private void Export()
        {
            // Имя оригинала без расширения + приписка
            string suggested = Path.GetFileNameWithoutExtension(_sourcePath) + "-копия";

            string path = _dialogs.SaveImage(suggested);
            if (path == null) return;           // отмена
                                                // Предупреждаем, если прозрачность пропадёт
            if (_hasTransparency && !ImageService.SupportsTransparency(path))
            {
                bool ok = _messages.Confirm(
                    "Этот формат не хранит прозрачность. Прозрачные области станут белыми. Продолжить?");
                if (!ok) return;
            }
            ImageService.Save(_processor.Apply((int)Amount), path, _hasTransparency);
        }
    }
}
