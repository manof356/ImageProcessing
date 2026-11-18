using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageProcessing.Services;

namespace ImageProcessing.Common
{
    // Общая основа для всех окон-инструментов: загрузка, экспорт, две картинки
    public abstract class ImageToolViewModel : ViewModelBase
    {
        private readonly IFileDialogService _dialogs;
        private readonly IMessageService _messages;
        private string _sourcePath;   // путь к оригиналу, нужен для имени при экспорте

        public ICommand LoadCommand { get; }
        public ICommand ExportCommand { get; }

        private BitmapSource _originalImage;
        public BitmapSource OriginalImage
        {
            get => _originalImage;
            private set { _originalImage = value; OnPropertyChanged(); }
        }

        private BitmapSource _resultImage;
        public BitmapSource ResultImage
        {
            get => _resultImage;
            protected set { _resultImage = value; OnPropertyChanged(); }   // наследник кладёт результат сюда
        }

        protected ImageToolViewModel(IFileDialogService dialogs, IMessageService messages)
        {
            _dialogs = dialogs;
            _messages = messages;
            LoadCommand = new RelayCommand(Load);
            ExportCommand = new RelayCommand(Export, () => CanExport);
            // в конструкторе: у команды появилось условие
            LoadCommand = new RelayCommand(Load, () => CanLoad);
        }

        // ----- "крючки" для наследников -----

        // Картинка загружена: инструмент готовит свои данные
        protected abstract void OnImageLoaded(BitmapSource bitmap);

        // Есть ли в результате прозрачность (для предупреждения при экспорте)
        protected virtual bool ResultHasTransparency => false;

        // Можно ли сейчас экспортировать
        protected virtual bool CanExport => ResultImage != null;
        // рядом с CanExport
        protected virtual bool CanLoad => true;

        // ----- общая логика -----

        private void Load()
        {
            string path = _dialogs.OpenImage();
            if (path == null) return;   // отмена

            _sourcePath = path;
            ResultImage = null;                                // старый результат выбрасываем
            OriginalImage = ImageService.LoadBitmap(path);     // старая картинка уходит
            OnImageLoaded(OriginalImage);
        }

        private void Export()
        {
            // Имя оригинала без расширения + приписка
            string suggested = Path.GetFileNameWithoutExtension(_sourcePath) + "-копия";
            string path = _dialogs.SaveImage(suggested);
            if (path == null) return;   // отмена

            // Предупреждаем, если прозрачность пропадёт
            if (ResultHasTransparency && !ImageService.SupportsTransparency(path))
            {
                bool ok = _messages.Confirm(
                    "Этот формат не хранит прозрачность. Прозрачные области станут белыми. Продолжить?");
                if (!ok) return;
            }

            ImageService.Save(ResultImage, path, ResultHasTransparency);
        }
    }
}