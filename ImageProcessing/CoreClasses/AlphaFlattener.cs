using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.CoreClasses
{
    internal class AlphaFlattener
    {
        // Накладывает картинку на белый фон, прозрачность исчезает
        public static BgraImage OnWhite(BgraImage source)
        {
            byte[] src = source.Pixels;
            var dst = new byte[src.Length];

            for (int i = 0; i < src.Length; i += 4)
            {
                int a = src[i + 3];
                // Цвет смешивается с белым (255) пропорционально прозрачности
                dst[i] = (byte)((src[i] * a + 255 * (255 - a) + 127) / 255);
                dst[i + 1] = (byte)((src[i + 1] * a + 255 * (255 - a) + 127) / 255);
                dst[i + 2] = (byte)((src[i + 2] * a + 255 * (255 - a) + 127) / 255);
                dst[i + 3] = 255;   // теперь всё непрозрачное
            }
            return new BgraImage(source.Width, source.Height, dst);
        }
    }
}
