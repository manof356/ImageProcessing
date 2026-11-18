using System;
using System.Threading.Tasks;

namespace ImageProcessing.CoreClasses
{
    // Обесцвечивание со степенью 0..100
    public class GrayscaleProcessor
    {
        private readonly BgraImage _source;  // оригинал, никогда не меняется
        private readonly byte[] _gray;       // серый каждого пикселя, считается один раз
        private readonly BgraImage _result;  // буфер результата, переиспользуется

        public GrayscaleProcessor(BgraImage source)
        {
            _source = source;
            // Яркость считает общий класс, прозрачность не трогаем
            _gray = Luminance.FromBgra(source, flattenOnWhite: false);
            _result = new BgraImage(source.Width, source.Height,
                                    new byte[source.Pixels.Length]);
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
            return _result;   // один и тот же объект, пиксели перезаписываются при следующем вызове
        }
    }
}