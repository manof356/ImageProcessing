using ImageProcessing.CoreClasses;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace ImageProcessing.Services
{
    public static class ImageService
    {
        // Читает файл (тот же код, что был во ViewModel)
        public static BitmapSource LoadBitmap(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(path);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        // Любой формат приводим к Bgra32 и забираем байты
        public static BgraImage ToBgra(BitmapSource source)
        {
            var converted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            int width = converted.PixelWidth;
            int height = converted.PixelHeight;
            int stride = width * 4;                    // байтов в одной строке
            var pixels = new byte[stride * height];
            converted.CopyPixels(pixels, stride, 0);
            return new BgraImage(width, height, pixels);
        }

        // Байты обратно в картинку для экрана
        public static BitmapSource ToBitmapSource(BgraImage image)
        {
            var bmp = BitmapSource.Create(image.Width, image.Height, 96, 96,
                                          PixelFormats.Bgra32, null,
                                          image.Pixels, image.Width * 4);
            bmp.Freeze();
            return bmp;
        }

        // Формат хранит прозрачность? Только PNG и TIFF
        public static bool SupportsTransparency(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".png" || ext == ".tif" || ext == ".tiff";
        }

        // Формат определяем по расширению выбранного файла
        public static void Save(BgraImage image, string path, bool hasTransparency)
        {
            bool keepAlpha = hasTransparency && SupportsTransparency(path);

            // Прозрачность есть, а формат её не держит: накладываем на белый
            if (hasTransparency && !keepAlpha)
                image = AlphaFlattener.OnWhite(image);

            BitmapSource bitmap = ToBitmapSource(image);

            // Альфа не нужна: убираем 4-й канал (32 бита → 24)
            if (!keepAlpha)
                bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr24, null, 0);

            BitmapEncoder encoder;
            switch (Path.GetExtension(path).ToLowerInvariant())
            {
                case ".png": encoder = new PngBitmapEncoder(); break;
                case ".jpg":
                case ".jpeg": encoder = new JpegBitmapEncoder { QualityLevel = 95 }; break;
                case ".tif":
                case ".tiff": encoder = new TiffBitmapEncoder(); break;
                case ".bmp": encoder = new BmpBitmapEncoder(); break;
                default: throw new NotSupportedException("Формат не поддерживается");
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
    }
}