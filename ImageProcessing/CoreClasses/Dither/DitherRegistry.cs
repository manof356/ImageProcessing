using System.Collections.Generic;

namespace ImageProcessing.CoreClasses
{
    // Список доступных алгоритмов. Новый алгоритм = новая строка здесь.
    public static class DitherRegistry
    {
        public static IReadOnlyList<IDitherer> All { get; } = new List<IDitherer>
        {
            // Соседи: справа 7/16, снизу-слева 3/16, снизу 5/16, снизу-справа 1/16
            new ErrorDiffusionDitherer("Флойда–Стейнберга",
                new DiffusionKernel(16, (1, 0, 7), (-1, 1, 3), (0, 1, 5), (1, 1, 1))),
        };
    }
}