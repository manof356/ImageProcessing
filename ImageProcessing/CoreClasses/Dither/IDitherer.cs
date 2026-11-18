using System;
using System.Threading;

namespace ImageProcessing.CoreClasses
{
    // Общий контракт любого алгоритма дизеринга
    public interface IDitherer
    {
        // Название для списка в окне
        string Name { get; }

        // Обрабатывает яркость НА МЕСТЕ.
        // На входе значения 0..255, на выходе только значения из levels уровней
        // (при levels = 2 это 0 и 255).
        void Apply(byte[] gray, int width, int height, int levels,
                   IProgress<int> progress, CancellationToken cancel);
    }
}