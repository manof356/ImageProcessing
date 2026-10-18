using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ImageProcessing.Services
{
    internal class FileDialogService : IFileDialogService
    {
        public string OpenImage()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите фотографию",
                // Первый пункт показывает все форматы сразу
                Filter = "Все изображения|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|" +
                         "JPEG|*.jpg;*.jpeg|PNG|*.png|BMP|*.bmp|TIFF|*.tif;*.tiff",
                CheckFileExists = true
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string SaveImage(string suggestedName)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Экспорт результата",
                FileName = suggestedName,   // имя, которое появится в поле
                // Формат выбирается в выпадающем списке, расширение добавится само
                Filter = "PNG|*.png|JPEG|*.jpg|TIFF|*.tif|BMP|*.bmp",
                AddExtension = true,
                OverwritePrompt = true   // спросит, если файл уже существует
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
