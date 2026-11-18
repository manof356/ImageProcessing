using System;
using System.Threading.Tasks;

namespace ImageProcessing.CoreClasses
{
    // Картинка как набор байтов: B, G, R, A подряд (4 байта на пиксель)
    public class BgraImage
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] Pixels { get; }

        public BgraImage(int width, int height, byte[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        // Есть ли хоть один не полностью непрозрачный пиксель
        public bool HasTransparency()
        {
            for (int i = 3; i < Pixels.Length; i += 4)   // альфа это каждый 4-й байт
                if (Pixels[i] < 255) return true;
            return false;
        }
    }
}