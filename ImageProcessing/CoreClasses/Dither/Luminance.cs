using System.Threading.Tasks;

namespace ImageProcessing.CoreClasses
{
    // Общий расчёт яркости: им пользуются и ЧБ-обработка, и дизеринг
    public static class Luminance
    {
        // Веса Rec.709, умноженные на 65536 (сумма ровно 65536)
        private const int WeightR = 13933;
        private const int WeightG = 46871;
        private const int WeightB = 4732;

        // Возвращает по одному байту яркости на пиксель.
        // flattenOnWhite: прозрачные пиксели смешиваются с белым фоном.
        public static byte[] FromBgra(BgraImage source, bool flattenOnWhite)
        {
            byte[] p = source.Pixels;
            int width = source.Width;
            var gray = new byte[width * source.Height];

            // Строки картинки делим между ядрами процессора
            Parallel.For(0, source.Height, y =>
            {
                int pixel = y * width;   // номер пикселя в начале строки
                int index = pixel * 4;   // позиция его первого байта
                for (int x = 0; x < width; x++, pixel++, index += 4)
                {
                    int b = p[index], g = p[index + 1], r = p[index + 2];
                    // +32768 для округления, >> 16 это деление на 65536
                    int lum = (r * WeightR + g * WeightG + b * WeightB + 32768) >> 16;

                    if (flattenOnWhite)
                    {
                        int a = p[index + 3];
                        // Смешиваем с белым (255) пропорционально прозрачности
                        if (a < 255)
                            lum = (lum * a + 255 * (255 - a) + 127) / 255;
                    }
                    gray[pixel] = (byte)lum;
                }
            });
            return gray;
        }
    }
}