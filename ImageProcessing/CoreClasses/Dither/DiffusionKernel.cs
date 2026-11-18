using System;

namespace ImageProcessing.CoreClasses
{
    // Таблица раздачи ошибки: кому из соседей и какую часть
    public class DiffusionKernel
    {
        // (dx, dy, weight): на сколько пикселей сосед правее и ниже, и его вес
        public (int dx, int dy, int weight)[] Taps { get; }
        public int Divisor { get; }   // веса делятся на это число
        public int MaxDx { get; }     // как далеко по горизонтали (нужно для запаса по краям)
        public int MaxDy { get; }     // как далеко вниз (нужно для числа строк ошибки)

        public DiffusionKernel(int divisor, params (int dx, int dy, int weight)[] taps)
        {
            if (divisor <= 0)
                throw new ArgumentException("Делитель должен быть больше нуля.");

            foreach (var t in taps)
            {
                // Ошибку можно отдавать только вперёд: вправо в этой строке или вниз
                if (t.dy < 0 || (t.dy == 0 && t.dx <= 0))
                    throw new ArgumentException("Ошибку можно раздавать только вправо или вниз.");

                MaxDx = Math.Max(MaxDx, Math.Abs(t.dx));
                MaxDy = Math.Max(MaxDy, t.dy);
            }
            Taps = taps;
            Divisor = divisor;
        }
    }
}