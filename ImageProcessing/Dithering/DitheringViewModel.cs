using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageProcessing.Common;
using ImageProcessing.CoreClasses;
using ImageProcessing.Services;

namespace ImageProcessing.Dithering
{
    public class DitheringViewModel : ImageToolViewModel
    {
        private byte[] _sourceGray;              // яркость оригинала, не меняется
        private int _width, _height;
        private bool _hasResult;                 // дизеринг уже применён?
        private CancellationTokenSource _cts;    // для кнопки "Отмена"

        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        // Список алгоритмов для комбобокса
        public IReadOnlyList<IDitherer> Algorithms => DitherRegistry.All;

        private IDitherer _selectedAlgorithm;
        public IDitherer SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set { _selectedAlgorithm = value; OnPropertyChanged(); }
        }

        // Идёт расчёт
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
                // Кнопки сами не заметят смену состояния без действий пользователя
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Прогресс 0..100
        private int _progress;
        public int Progress
        {
            get => _progress;
            private set { _progress = value; OnPropertyChanged(); }
        }

        public DitheringViewModel(IFileDialogService dialogs, IMessageService messages)
            : base(dialogs, messages)
        {
            ApplyCommand = new RelayCommand(Apply, () => _sourceGray != null && !IsBusy);
            CancelCommand = new RelayCommand(() => _cts?.Cancel(), () => IsBusy);
            SelectedAlgorithm = Algorithms.Count > 0 ? Algorithms[0] : null;
        }

        // Во время расчёта нельзя загружать другую картинку, а экспорт только после результата
        protected override bool CanLoad => !IsBusy;
        protected override bool CanExport => _hasResult && !IsBusy;

        // Картинка загружена: готовим яркость
        protected override void OnImageLoaded(BitmapSource bitmap)
        {
            _sourceGray = null;        // старое освобождаем до создания нового
            _hasResult = false;

            // Временная BGRA-копия, нужна только чтобы посчитать яркость
            BgraImage bgra = ImageService.ToBgra(bitmap);
            _width = bgra.Width;
            _height = bgra.Height;
            // Прозрачное заливаем белым: в сером результате прозрачности нет
            _sourceGray = Luminance.FromBgra(bgra, flattenOnWhite: true);

            // Пока дизеринг не применён, "После" показывает ту же картинку, что и "До"
            ResultImage = bitmap;
        }

        // async void допустим для команд: всё, что может упасть, ловим внутри
        private async void Apply()
        {
            IDitherer algorithm = SelectedAlgorithm;
            if (_sourceGray == null || algorithm == null) return;

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            int w = _width, h = _height;

            Progress = 0;
            IsBusy = true;

            // Progress<T> создан в потоке окна, поэтому и вызывается в нём
            var progress = new Progress<int>(p => Progress = p);

            try
            {
                // Алгоритм меняет массив на месте, а оригинал нужен нетронутым
                byte[] work = (byte[])_sourceGray.Clone();

                // Тяжёлый расчёт уходит в фон, окно не зависает
                await Task.Run(() => algorithm.Apply(work, w, h, 2, progress, token), token);

                // Мы снова в потоке окна: показываем результат
                ResultImage = ImageService.FromGray(work, w, h);
                _hasResult = true;
            }
            catch (OperationCanceledException)
            {
                // Отмена: прежний результат остаётся как есть
            }
            finally
            {
                IsBusy = false;
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}