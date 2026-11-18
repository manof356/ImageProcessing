using System;
using System.Threading;

namespace ImageProcessing.CoreClasses
{
    // Универсальный движок диффузии ошибки.
    // Floyd–Steinberg, Atkinson, Jarvis и другие отличаются только таблицей (DiffusionKernel).
    public class ErrorDiffusionDitherer : IDitherer
    {
        private readonly DiffusionKernel _kernel;
        private readonly bool _serpentine;   // змейка: чётные строки вправо, нечётные влево

        public string Name { get; }

        public ErrorDiffusionDitherer(string name, DiffusionKernel kernel, bool serpentine = true)
        {
            Name = name;
            _kernel = kernel;
            _serpentine = serpentine;
        }

        public void Apply(byte[] gray, int width, int height, int levels,
                          IProgress<int> progress, CancellationToken cancel)
        {
            if (levels < 2) levels = 2;

            // Готовая таблица: "для яркости v ближайший допустимый уровень"
            byte[] quant = BuildQuantTable(levels);

            // Раскладываем таблицу соседей по простым массивам (так быстрее в цикле)
            var taps = _kernel.Taps;
            int n = taps.Length;
            var tapDx = new int[n];
            var tapDy = new int[n];
            var tapW = new float[n];
            for (int k = 0; k < n; k++)
            {
                tapDx[k] = taps[k].dx;
                tapDy[k] = taps[k].dy;
                tapW[k] = (float)taps[k].weight / _kernel.Divisor;   // доля ошибки соседу
            }

            // Буфер накопленной ошибки: по одной строке на каждый уровень "вниз" + текущая
            int rows = _kernel.MaxDy + 1;
            int pad = _kernel.MaxDx;            // запас слева и справа, чтобы не проверять границы
            int stride = width + 2 * pad;
            var err = new float[rows][];
            for (int r = 0; r < rows; r++) err[r] = new float[stride];

            var targets = new float[n][];       // в какую строку ошибки пишет каждый сосед
            int lastPercent = -1;

            for (int y = 0; y < height; y++)
            {
                cancel.ThrowIfCancellationRequested();   // пользователь нажал "Отмена"

                // Направление строки: 1 это вправо, -1 это влево (змейка)
                int dir = (_serpentine && (y & 1) == 1) ? -1 : 1;
                int xStart = dir == 1 ? 0 : width - 1;

                float[] cur = err[y % rows];             // ошибка, пришедшая в эту строку
                for (int k = 0; k < n; k++)
                    targets[k] = err[(y + tapDy[k]) % rows];

                int rowStart = y * width;                // индекс первого пикселя строки
                for (int i = 0, x = xStart; i < width; i++, x += dir)
                {
                    int idx = rowStart + x;              // индекс = y * ширина + x

                    // Яркость пикселя плюс ошибка, которую раздали предыдущие
                    float value = gray[idx] + cur[x + pad];

                    // Ограничиваем 0..255 и округляем
                    int clamped = value <= 0f ? 0 : value >= 255f ? 255 : (int)(value + 0.5f);

                    byte q = quant[clamped];             // ближайший допустимый уровень
                    gray[idx] = q;                       // результат пишем на место исходного

                    int e = clamped - q;                 // ошибка округления
                    if (e != 0)
                    {
                        // Раздаём ошибку соседям. По горизонтали смещение зеркалим (dir)
                        for (int k = 0; k < n; k++)
                            targets[k][x + tapDx[k] * dir + pad] += e * tapW[k];
                    }
                }

                // Строка закончена, её буфер очищаем и он пойдёт под строку y + rows
                Array.Clear(cur, 0, stride);

                // Прогресс сообщаем только когда процент изменился (не чаще 100 раз)
                int percent = (y + 1) * 100 / height;
                if (percent != lastPercent)
                {
                    lastPercent = percent;
                    progress?.Report(percent);
                }
            }
        }

        // Для каждой яркости 0..255 находит ближайшую из levels равномерных ступеней
        private static byte[] BuildQuantTable(int levels)
        {
            var table = new byte[256];
            double step = 255.0 / (levels - 1);
            for (int v = 0; v < 256; v++)
            {
                int level = (int)Math.Round(v / step);       // номер ближайшего уровня
                table[v] = (byte)Math.Round(level * step);   // его яркость
            }
            return table;
        }
    }
}