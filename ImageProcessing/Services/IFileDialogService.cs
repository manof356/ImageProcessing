public interface IFileDialogService
{
    // Путь к файлу или null, если пользователь нажал "Отмена"
    string OpenImage();
    // Путь для сохранения или null, если нажали "Отмена"
    string SaveImage(string suggestedName);
}