using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.CoreClasses
{
    // Обесцвечивание со степенью 0..100
    public class GrayscaleProcessor
    {
        // Веса Rec.709, умноженные на 65536 (сумма ровно 65536)
        private const int WeightR = 13933;
        private const int WeightG = 46871;
        private const int WeightB = 4732;

        private readonly BgraImage _source;  // оригинал, никогда не меняется
        private readonly byte[] _gray;       // серый каждого пикселя, считается один раз
        private readonly BgraImage _result;  // буфер результата, переиспользуется

        public GrayscaleProcessor(BgraImage source)
        {
            _source = source;
            _gray = new byte[source.Width * source.Height];
            _result = new BgraImage(source.Width, source.Height,
                                    new byte[source.Pixels.Length]);

            byte[] p = source.Pixels;
            int width = source.Width;

            // Строки картинки делим между ядрами процессора
            Parallel.For(0, source.Height, y =>
            {
                int pixel = y * width;   // номер пикселя в начале строки
                int index = pixel * 4;   // позиция его первого байта
                for (int x = 0; x < width; x++, pixel++, index += 4)
                {
                    int b = p[index], g = p[index + 1], r = p[index + 2];
                    // +32768 нужно для округления, >> 16 это деление на 65536
                    _gray[pixel] = (byte)((r * WeightR + g * WeightG + b * WeightB + 32768) >> 16);
                }
            });
        }

        // amount: 0 это оригинал, 100 это полностью серая
        public BgraImage Apply(int amount)
        {
            amount = Math.Max(0, Math.Min(100, amount));
            int t = (amount * 256 + 50) / 100;   // 0..256, чтобы делить сдвигом

            byte[] src = _source.Pixels;
            byte[] dst = _result.Pixels;
            int width = _source.Width;

            Parallel.For(0, _source.Height, y =>
            {
                int pixel = y * width;
                int index = pixel * 4;
                for (int x = 0; x < width; x++, pixel++, index += 4)
                {
                    int gray = _gray[pixel];
                    // Смешиваем исходный канал с серым: c + (gray - c) * t
                    dst[index] = (byte)(src[index] + (((gray - src[index]) * t) >> 8)); // B
                    dst[index + 1] = (byte)(src[index + 1] + (((gray - src[index + 1]) * t) >> 8)); // G
                    dst[index + 2] = (byte)(src[index + 2] + (((gray - src[index + 2]) * t) >> 8)); // R
                    dst[index + 3] = src[index + 3];                                                // A без изменений
                }
            });
            return _result;   // один и тот же объект, при следующем вызове его пиксели перезапишутся
        }
    }
}
